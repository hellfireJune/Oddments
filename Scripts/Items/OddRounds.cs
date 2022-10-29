using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using JuneLib.Items;

namespace Oddments
{
    public class OddRounds : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(OddRounds))
        {
            Name = "Odd Shells",
            Description = "Oddments",
            LongDescription = "Will mimic the effect of other bullet items while held",
            Quality = ItemQuality.B,
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/oddshells.png",
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

                    List<int> list = AlexandriaTags.GetAllItemsIdsWithTag("bullet_modifier");
                    list.Remove(this.PickupObjectId);
                    int id = BraveUtility.RandomElement(list);
                    PassiveItem prefabItem = PickupObjectDatabase.GetById(id) as PassiveItem;
                    //EncounterTrackable.SuppressNextNotification = true;
                    bulletItem = RealFakeItemHelper.CreateFakeItem(prefabItem, Owner, transform);
                    ETGModConsole.Log(Owner.passiveItems.Count);

                    foreach (var item in Owner.passiveItems)
                    {
                        ETGModConsole.Log(item.PickupObjectId);
                        if (item.GetComponent<FakeRealItemBehaviour>() != null)
                        {
                            ETGModConsole.Log("fake");
                        }
                    }
                }

                timer -= BraveTime.DeltaTime;
            }
        }
    }
}
