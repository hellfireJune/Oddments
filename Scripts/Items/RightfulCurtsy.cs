using Alexandria.ItemAPI;
using Alexandria.Misc;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class RightfulCurtsy : OddProjectileModifierItem
    {
        public static BulletModifierItemTemplate template = new BulletModifierItemTemplate(typeof(RightfulCurtsy))
        {
            Name = "rightful curtsy",
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.AmmoCapacityMultiplier, 0.1f);
                item.SetTag("lemegeton_non_summonable");
                item.RemovePickupFromLootTables();
            },

            ProcChance = 0.5f
        };

        public override bool ApplyBulletEffect(Projectile arg1)
        {
            arg1.baseData.force *= 2;
            arg1.RuntimeUpdateScale(1.1f);
            return true;
        }
    }
}
