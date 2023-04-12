using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class HookWormItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(HookWormItem))
        {
            Name = "Cubic Shells",
            SpriteResource = $"{Module.SPRITE_PATH}/cubicshells.png",
            Description = "Power of 3",
            LongDescription = "Makes reloading easier, but bullets will take on a rectangular travel pattern",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.ReloadSpeed, 0.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                item.MakeBulletMod();
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += Player_PostProcessProjectile;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            player.PostProcessProjectile -= Player_PostProcessProjectile;
        }

        private void Player_PostProcessProjectile(Projectile obj, float arg2)
        {
            if (obj)
            {
                if (obj.OverrideMotionModule != null && obj.OverrideMotionModule is OrbitProjectileMotionModule)
                {
                    OrbitProjectileMotionModule orbitProjectileMotionModule = obj.OverrideMotionModule as OrbitProjectileMotionModule;
                    orbitProjectileMotionModule.StackHelix = true;
                    orbitProjectileMotionModule.ForceInvert = !this.shouldInvert;
                } else
                {
                    obj.OverrideMotionModule = new HookWormMovementModifier()
                    {
                        ShouldInvert = this.shouldInvert
                    };
                }
                shouldInvert = !shouldInvert;
            }
        }

        private bool shouldInvert = false;
    }

    public class HookWormMovementModifier : HelixProjectileMotionModule
    {
        public override void Move(Projectile source, Transform projectileTransform, tk2dBaseSprite projectileSprite, SpeculativeRigidbody specRigidbody, ref float m_timeElapsed, ref Vector2 m_currentDirection, bool Inverted, bool shouldRotate)
        {
            ProjectileData baseData = source.baseData;
            Vector2 vector = (!projectileSprite) ? projectileTransform.position.XY() : projectileSprite.WorldCenter;
            if (!this.m_wormInitialised)
            {
                this.m_wormInitialised = true;
                this.m_initialRightVector = ((!shouldRotate) ? m_currentDirection : projectileTransform.right.XY());
                this.m_initialUpVector = ((!shouldRotate) ? (Quaternion.Euler(0f, 0f, 90f) * m_currentDirection) : projectileTransform.up);
                this.m_privateLastPosition = vector;
                this.m_displacement = 0f;
                this.m_yDisplacement = 0f;
                m_timeElapsedPure = WaveLength + (CrissCrossDuration / 2f);
                m_lastWigglePos = 1f;
                m_mustWiggle = false;
            }
            m_timeElapsedPure += BraveTime.DeltaTime;
            float heresTheWiggler = Mathf.Clamp((((m_timeElapsedPure * baseData.speed) % (CrissCrossDuration + WaveLength + 1)) - WaveLength) / CrissCrossDuration, 0, 1);
            m_timeElapsed += BraveTime.DeltaTime * (heresTheWiggler == 0 ? 1 : 0);
            int num = (!(Inverted ^ this.ShouldInvert)) ? 1 : -1;
            float num2 = m_timeElapsed * baseData.speed;
            if (heresTheWiggler >= 1)
            {
                if (!m_mustWiggle)
                {
                    m_mustWiggle = true;
                }
            }
            else if (m_mustWiggle)
            {
                m_mustWiggle = false;
                m_lastWigglePos = Math.Abs(m_lastWigglePos - 1);
            }
            float num3 = num * Mathf.Lerp(-HookAmplitude, HookAmplitude, Mathf.Abs(m_lastWigglePos - heresTheWiggler));
            float d = num2 - this.m_displacement;
            float d2 = num3 - this.m_yDisplacement;
            Vector2 vector2 = this.m_privateLastPosition + this.m_initialRightVector * d + this.m_initialUpVector * d2;
            this.m_privateLastPosition = vector2;
            if (shouldRotate)
            {
                float num4 = (m_timeElapsed + 0.01f) * baseData.speed;
                float num5 = (float)num * this.HookAmplitude/* * Mathf.Round(Mathf.Sin((m_timeElapsed + 0.01f) * 3.1415927f * baseData.speed / this.WaveLength))*/;
                float num6 = BraveMathCollege.Atan2Degrees(num5 - num3, num4 - num2);
                projectileTransform.localRotation = Quaternion.Euler(0f, 0f, num6 + this.m_initialRightVector.ToAngle());
            }
            Vector2 vector3 = (vector2 - vector) / BraveTime.DeltaTime;
            float f = BraveMathCollege.Atan2Degrees(vector3);
            if (!float.IsNaN(f))
            {
                m_currentDirection = vector3.normalized;
            }
            this.m_displacement = num2;
            this.m_yDisplacement = num3;
            specRigidbody.Velocity = vector3;
        }

        private float m_timeElapsedPure = 0f;
        public bool ShouldInvert;
        private bool m_wormInitialised = false;
        public float HookAmplitude = 0.5f;
        public float WaveLength = 6f;
        public float CrissCrossDuration = 1f;
        private float m_lastWigglePos;
        private bool m_mustWiggle;
    }
}
