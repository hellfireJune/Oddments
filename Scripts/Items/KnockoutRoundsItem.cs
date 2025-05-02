using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class KnockoutRoundsItem : OddProjectileModifierItem
    {
        public static BulletModifierItemTemplate template = new BulletModifierItemTemplate(typeof(KnockoutRoundsItem))
        {
            ProcChance = 0.09f
        };

        public override bool ApplyBulletEffect(Projectile proj)
        {
            proj.OnHitEnemy += HitEnemy;
            return true;
        }

        private void HitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.knockbackDoer)
            {
                ActiveKnockbackData knockback = arg2.knockbackDoer.ApplyKnockback(arg1.LastVelocity, arg1.baseData.force * 3);
                HitEnemyHardHandler handler = arg2.gameObject.AddComponent<HitEnemyHardHandler>();
                handler.knockToCheck = knockback;
            }
        }
    }

    internal class HitEnemyHardHandler : BraveBehaviour
    {
        public ActiveKnockbackData knockToCheck;
    }
}
