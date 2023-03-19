using Alexandria.Misc;
using JuneLib.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Oddments
{
    public class BlendedHeartPickup : HealthPickup
    {
        public static PickupTemplate template = new PickupTemplate(typeof(BlendedHeartPickup))
        {
            Name = "Blended Heart",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                HealthPickup pickup = (HealthPickup)item;
                pickup.armorAmount = 1;
                OddItemIDs.BlendedHeart = item.PickupObjectId;
            }
        };

        public override void Pickup(PlayerController player)
        {
            if (player.healthHaver.GetCurrentHealth() < player.healthHaver.GetMaxHealth())
            {
                this.healAmount = 1f;
                this.armorAmount = 0;
            }
            base.Pickup(player);
        }
    }
}
