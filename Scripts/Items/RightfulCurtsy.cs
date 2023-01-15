using Alexandria.ItemAPI;
using Alexandria.Misc;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class RightfulCurtsy : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RightfulCurtsy))
        {
            Name = "rightful curtsy",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.AmmoCapacityMultiplier, 0.1f);
                item.SetTag("lemegeton_non_summonable");
                item.RemovePickupFromLootTables();
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += Player_PostProcessProjectile;
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            if (UnityEngine.Random.value < 0.5)
            {
                arg1.baseData.force *= 2;
                arg1.RuntimeUpdateScale(1.1f);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            player.PostProcessProjectile -= Player_PostProcessProjectile;
            base.DisableEffect(player);
        }
    }
}
