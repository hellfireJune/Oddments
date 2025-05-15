using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class OddProjectileModifierItem : PassiveItem
    {
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += Player_PostProcessProjectile;
            base.Pickup(player);
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            if (arg1)
            {
                float chance = ActivationChance;
                if (ChanceScalesWithDamageFired)
                {
                    chance *= m_damageFired;
                }

                if (UnityEngine.Random.value < chance && ApplyBulletEffect(arg1))
                {
                    if (BulletTint != null)
                    {
                        arg1.AdjustPlayerProjectileTint(BulletTint, TintPriority);
                    }
                    m_damageFired = 0f;
                }
            }

            m_damageFired += arg1.baseData.damage;
        }

        public override void DisableEffect(PlayerController player)
        {
            if(player)
                player.PostProcessProjectile -= Player_PostProcessProjectile;

            base.DisableEffect(player);
        }
        public virtual bool ApplyBulletEffect(Projectile projectile)
        {
            return true;
        }

        public Color BulletTint;
        public int TintPriority = 3;
        public float ActivationChance = 1f;

        public bool ChanceScalesWithDamageFired = false;
        private float m_damageFired = 0f;
    }
}
