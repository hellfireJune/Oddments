using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{

    public class SonarBullets : OddProjectileModifierItem
    {
        [SerializeField]
        public GameActorHomeEffect EffectToApply = new GameActorHomeEffect
        {
            AffectsPlayers = false,
            AffectsEnemies = true,
            AppliesTint = true,
            TintColor = new Color(0.25f, 0.25f, 0.25f),
            duration = 8f,
            effectIdentifier = "radar",
            DamagePerSecondToEnemies = 0,
            ignitesGoops = false,
        };



        [Serializable]
        public class GameActorHomeEffect : GameActorHealthEffect 
        {
            public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
            {
                
            }
        }
        public static BulletModifierItemTemplate template = new BulletModifierItemTemplate(typeof(SonarBullets))
        {
            Name = "Radar Bullets",
            Description = "Subma-bullets",
            LongDescription = "Bullets have a chance to mark enemies to be homed into by other bullets",
            SpriteResource = $"{Module.SPRITE_PATH}/sonarbullets.png",
            Quality = ItemQuality.A,
            PostInitAction = item =>
            {
                item.MakeBulletMod();
                item.AddToSubShop(ItemBuilder.ShopType.Goopton);
                /*
                (item as SonarBullets).EffectToApply = new GameActorHomeEffect
                {
                    AffectsPlayers = false,
                    AffectsEnemies = true,
                    AppliesTint = true,
                    TintColor = new Color(0.25f, 0.25f, 0.25f),
                    duration = 8f,
                    effectIdentifier = "radar",
                    DamagePerSecondToEnemies = 0,
                    ignitesGoops = false,
                };
                */
            },
            
            ProcChance = 0.1f,
            ProjectileTint = new Color(0.25f, 0.25f, 0.25f)
        };



        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += Player_PostProcessProjectile;
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.PostProcessProjectile -= Player_PostProcessProjectile;
            base.DisableEffect(player);
        }

        public override bool ApplyBulletEffect(Projectile arg1)
        {
            arg1.statusEffectsToApply.Add(EffectToApply);
            return true;
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            arg1.gameObject.AddComponent<RadarHomingMod>();
        }

        public class RadarHomingMod : BraveBehaviour
        {
            private void Start()
            {
                if (!m_projectile)
                {
                    m_projectile = base.GetComponent<Projectile>();
                }
                Projectile projectile = m_projectile;
                projectile.ModifyVelocity += ModifyVelocity;
            }

            public void AssignProjectile(Projectile source)
            {
                m_projectile = source;
            }

            private Vector2 ModifyVelocity(Vector2 inVel)
            {
                Vector2 vector = inVel;
                RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(m_projectile.LastPosition.IntXY(VectorConversions.Floor));
                List<AIActor> activeEnemies = absoluteRoomFromPosition.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies == null || activeEnemies.Count == 0)
                {
                    return inVel;
                }
                float num = float.MaxValue;
                Vector2 vector2 = Vector2.zero;
                AIActor x = null;
                Vector2 b = (!base.sprite) ? base.transform.position.XY() : base.sprite.WorldCenter;
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    AIActor aiactor = activeEnemies[i];
                    if (aiactor && aiactor.IsWorthShootingAt && !aiactor.IsGone
                        && aiactor.GetEffect("radar") != null)
                    {
                        Vector2 vector3 = aiactor.CenterPosition - b;
                        float sqrMagnitude = vector3.sqrMagnitude;
                        if (sqrMagnitude < num)
                        {
                            vector2 = vector3;
                            num = sqrMagnitude;
                            x = aiactor;
                        }
                    }
                }
                num = Mathf.Sqrt(num);
                if (num < HomingRadius && x != null)
                {
                    float num2 = 1f - num / HomingRadius;
                    float target = vector2.ToAngle();
                    float num3 = inVel.ToAngle();
                    float maxDelta = AngularVelocity * num2 * m_projectile.LocalDeltaTime;
                    float num4 = Mathf.MoveTowardsAngle(num3, target, maxDelta);
                    if (m_projectile is HelixProjectile)
                    {
                        float angleDiff = num4 - num3;
                        (m_projectile as HelixProjectile).AdjustRightVector(angleDiff);
                    }
                    else
                    {
                        if (m_projectile.shouldRotate)
                        {
                            base.transform.rotation = Quaternion.Euler(0f, 0f, num4);
                        }
                        vector = BraveMathCollege.DegreesToVector(num4, inVel.magnitude);
                    }
                    if (m_projectile.OverrideMotionModule != null)
                    {
                        m_projectile.OverrideMotionModule.AdjustRightVector(num4 - num3);
                    }
                }
                if (vector == Vector2.zero || float.IsNaN(vector.x) || float.IsNaN(vector.y))
                {
                    return inVel;
                }
                return vector;
            }
            public override void OnDestroy()
            {
                if (m_projectile)
                {
                    Projectile projectile = m_projectile;
                    projectile.ModifyVelocity -= ModifyVelocity;
                }
                base.OnDestroy();
            }

            public float HomingRadius = 6f;

            public float AngularVelocity = 180f;

            protected Projectile m_projectile;
        }
    }
}
