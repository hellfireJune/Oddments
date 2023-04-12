using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using JuneLib.Items;
using SaveAPI;

namespace Oddments
{
    public class OddRounds : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(OddRounds))
        {
            Name = "Odd Shells",
            Description = "Oddments",
            LongDescription = "Will mimic the effect of other bullet items while held",
            Quality = ItemQuality.B,
            SpriteResource = $"{Module.SPRITE_PATH}/oddshells.png",
            PostInitAction = item =>
            {
                item.MakeBulletMod();
                item.SetupUnlockOnCustomFlag(CustomDungeonFlags.EVERY_VANILLA_BULLET_UNLOCKED, true);
                item.AddUnlockText("Unlock every vanilla bullet modifier (except Chance Bullets and Chaos Bullets)");
            }
        };

        protected float timer;
        protected float maxTimer = 10f;
        protected PassiveItem bulletItem;
        public override void Update()
        {
            base.Update();
            if (Owner)
            {
                if (timer < 0f)
                {
                    timer = maxTimer;
                    if (bulletItem)
                    {
                        RealFakeItemHelper.RemoveFakeItem(Owner, bulletItem);
                    }

                    List<int> list = AlexandriaTags.GetAllItemsIdsWithTag("bullet_modifier").Where(item => PickupObjectDatabase.GetById(item).CanBeDropped).ToList();
                    list.Remove(this.PickupObjectId);
                    int id = BraveUtility.RandomElement(list);
                    PassiveItem prefabItem = PickupObjectDatabase.GetById(id) as PassiveItem;
                    //EncounterTrackable.SuppressNextNotification = true;
                    bulletItem = RealFakeItemHelper.CreateFakeItem(prefabItem, Owner, transform);
                }

                timer -= BraveTime.DeltaTime;
            }
        }
    }
}
