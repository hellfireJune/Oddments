using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    internal class ShotsFiredToDamageUpItem : OddProjectileModifierItem
    {
        public static string RedoxSynergyName = "Redox Reload"; public static List<string> ids = new List<string>() { $"{Module.PREFIX}:catho-rounds", $"{Module.PREFIX}:ano-rounds" };

        public static BulletModifierItemTemplate template2 = new BulletModifierItemTemplate(typeof(ShotsFiredToDamageUpItem))
        {
            Name = "Catho-Rounds",
            Description = "Charge Loser",
            LongDescription = "All bullets fired gain damage, which is gradually lost in the last 6 bullets of the clip",
            Quality = ItemQuality.A,
            SpriteResource = $"{Module.SPRITE_PATH}/cathorounds.png",
        };
        public static BulletModifierItemTemplate template = new BulletModifierItemTemplate(typeof(ShotsFiredToDamageUpItem))
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

        public override bool ApplyBulletEffect(Projectile arg1)
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

            return true;
        }

        public bool ReverseDamageBuff = false;
        public float bulletAmount = 6;
        public float MaxMult = 1.5f;
    }
}
