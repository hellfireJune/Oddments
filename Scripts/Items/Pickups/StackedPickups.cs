using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PickupObject;

namespace Oddments
{
    public static class StackedPickups
    {
        public static PickupTemplate template = new PickupTemplate(typeof(HealthPickup))
        {
            Name = "Stacked Armor",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                HealthPickup pickup = (HealthPickup)item;
                pickup.armorAmount = 2;
                OddItemIDs.StackedArmor = item.PickupObjectId;
            }
        };
        public static PickupTemplate template2 = new PickupTemplate(typeof(StackedSilencerItem))
        {
            Name = "Stacked Blank",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                OddItemIDs.StackedBlanks = item.PickupObjectId;
            }
        };
        public static PickupTemplate template3 = new PickupTemplate(typeof(KeyBulletPickup))
        {
            Name = "Key Ring",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                KeyBulletPickup pickup = (KeyBulletPickup)item;
                pickup.numberKeyBullets = 2;
                OddItemIDs.KeyRingPickup = item.PickupObjectId;
            }
        };

        public class StackedSilencerItem : SilencerItem
        {
            public override void Pickup(PlayerController player)
            {
                if (!PickedUp)
                {
                    player.Blanks++;
                }
                base.Pickup(player);
            }
        }
    }
}
