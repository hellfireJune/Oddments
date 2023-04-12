using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    internal class ShotsFiredToDamageUpItem : PassiveItem
    {
        public static OddItemTemplate template2 = new OddItemTemplate(typeof(ShotsFiredToDamageUpItem))
        {
            Name = "Catho-Rounds",
            Description = "Charge Loser",
            LongDescription = "All bullets fired gain damage, which is gradually lost in the last 6 bullets of the clip",
            Quality = ItemQuality.A,
            SpriteResource = $"{Module.SPRITE_PATH}/cathorounds.png",
        };
        public static OddItemTemplate template = new OddItemTemplate(typeof(ShotsFiredToDamageUpItem))
        {
            Name = "Ano-Rounds",
            Description = "Charge Gainer",
            LongDescription = "All bullets fired gain damage that ramps up, increasing until the 6th bullet in the clip.",
            Quality = ItemQuality.A,
            SpriteResource = $"{Module.SPRITE_PATH}/anorounds.png",
            PostInitAction = item =>
            {
                ShotsFiredToDamageUpItem pickup = item as ShotsFiredToDamageUpItem;
                pickup.ReverseDamageBuff = true;
            }
        };

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += PostProcessProjectile;
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.PostProcessProjectile -= PostProcessProjectile;
            base.DisableEffect(player);
        }

        private void PostProcessProjectile(Projectile arg1, float arg2)
        {
            Gun gun = this.Owner.CurrentGun;
            int gunIndex = (gun?.LastShotIndex ?? 0);
            if (ReverseDamageBuff) 
            {
                gunIndex = -gunIndex-1 + (gun?.ClipCapacity ?? 0);
            }

            float mult = Mathf.Lerp(1, MaxMult, gunIndex / bulletAmount);
            mult = Mathf.Clamp(mult, 1, MaxMult);
            arg1.baseData.damage *= mult;
            arg1.RuntimeUpdateScale(mult);
        }

        public bool ReverseDamageBuff = false;
        public float bulletAmount = 6;
        public float MaxMult = 1.5f;
    }
}
