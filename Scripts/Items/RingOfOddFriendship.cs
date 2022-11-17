using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;
using Dungeonator;

namespace Oddments
{
    public class RingOfOddFriendship : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RingOfOddFriendship))
        {
            Name = "ring of odd friendship",
            Quality = ItemQuality.B,
            PostInitAction = item =>
            {
                item.SetTag("companion");
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

        public void StartFloor(Dungeon dungeon)
        {
            if (Owner)
            {
                MakeItem(Owner);
            }
        }
        protected PassiveItem bulletItem;
    }
}
