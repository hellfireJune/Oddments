using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;
using Dungeonator;
using SaveAPI;

namespace Oddments
{
    public class RingOfOddFriendship : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RingOfOddFriendship))
        {
            Name = "Ring of Odd Friendship",
            Description = "Odd friends",
            LongDescription = "Gives you a random companion that changes whenever you descend to a new floor in the Gungeon.",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/ringofoddfriendship.png",
            Quality = ItemQuality.B,
            PostInitAction = item =>
            {
                item.SetTag("companion");
                item.SetupUnlockOnCustomFlag(CustomDungeonFlags.EVERY_VANILLA_COMPANION_UNLOCKED, true);
                item.AddUnlockText("Unlock every vanilla companion");
            }
        };

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                MakeItem(player);
            }
            base.Pickup(player);
            CustomActions.PostDungeonTrueStart += StartFloor;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            CustomActions.PostDungeonTrueStart -= StartFloor;
        }

        public void MakeItem(PlayerController player)
        {
            if (bulletItem)
            {
                RealFakeItemHelper.RemoveFakeItem(player, bulletItem);
            }

            List<int> list = AlexandriaTags.GetAllItemsIdsWithTag("companion").Where(item => PickupObjectDatabase.GetById(item).CanBeDropped && PickupObjectDatabase.GetById(item) is PassiveItem
            && !PickupObjectDatabase.GetById(item).HasTag("lemegeton_non_summonable")).ToList();
            list.Remove(this.PickupObjectId);
            int id = BraveUtility.RandomElement(list);
            PassiveItem prefabItem = PickupObjectDatabase.GetById(id) as PassiveItem;
            bulletItem = RealFakeItemHelper.CreateFakeItem(prefabItem, player, transform);
        }

        public override void Update()
        {
            base.Update();
            if (Owner
                && !GameManager.Instance.IsLoadingLevel
                && queueMakeItem)
            {
                queueMakeItem = false;
                MakeItem(Owner);
            }
        }

        public void StartFloor(Dungeon dungeon)
        {
            if (Owner)
            {
                //MakeItem(Owner);
                queueMakeItem = true;
            }
        }
        private bool queueMakeItem = false;
        protected PassiveItem bulletItem;
    }
}
