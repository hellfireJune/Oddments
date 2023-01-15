using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using JuneLib.Items;
using UnityEngine;

namespace Oddments
{
    public class SonarBullets : PassiveItem
    {
		public class GameActorHomeEffect : GameActorEffect { }
		public static GameActorHomeEffect homeEffect;
		public static ItemTemplate template = new ItemTemplate(typeof(SonarBullets))
		{
			Name = "Radar Bullets",
			Description = "Subma-bullets",
			LongDescription = "Bullets have a chance to mark enemies to be homed into by other bullets",
			SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/sonarbullets.png",
			Quality = ItemQuality.A,
			PostInitAction = item =>
			{
                homeEffect = new GameActorHomeEffect
                {
                    AffectsPlayers = false,
                    AffectsEnemies = true,
                    AppliesTint = true,
                    TintColor = new Color(0.25f, 0.25f, 0.25f),
                    duration = 8f,
                    effectIdentifier = "radar"
                };
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

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            arg1.gameObject.AddComponent<RadarHomingMod>();
			if (UnityEngine.Random.value < 0.1f)
            {
				arg1.statusEffectsToApply.Add(homeEffect);
				arg1.AdjustPlayerProjectileTint(new Color(0.25f, 0.25f, 0.25f), 3);
			}
        }

        public class RadarHomingMod : BraveBehaviour
        {

			private void Start()
			{
				if (!this.m_projectile)
				{
					this.m_projectile = base.GetComponent<Projectile>();
				}
				Projectile projectile = this.m_projectile;
				projectile.ModifyVelocity += this.ModifyVelocity;
			}

			public void AssignProjectile(Projectile source)
			{
				this.m_projectile = source;
			}

			private Vector2 ModifyVelocity(Vector2 inVel)
			{
				Vector2 vector = inVel;
				RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.m_projectile.LastPosition.IntXY(VectorConversions.Floor));
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
				if (num < this.HomingRadius && x != null)
				{
					float num2 = 1f - num / this.HomingRadius;
					float target = vector2.ToAngle();
					float num3 = inVel.ToAngle();
					float maxDelta = this.AngularVelocity * num2 * this.m_projectile.LocalDeltaTime;
					float num4 = Mathf.MoveTowardsAngle(num3, target, maxDelta);
					if (this.m_projectile is HelixProjectile)
					{
						float angleDiff = num4 - num3;
						(this.m_projectile as HelixProjectile).AdjustRightVector(angleDiff);
					}
					else
					{
						if (this.m_projectile.shouldRotate)
						{
							base.transform.rotation = Quaternion.Euler(0f, 0f, num4);
						}
						vector = BraveMathCollege.DegreesToVector(num4, inVel.magnitude);
					}
					if (this.m_projectile.OverrideMotionModule != null)
					{
						this.m_projectile.OverrideMotionModule.AdjustRightVector(num4 - num3);
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
				if (this.m_projectile)
				{
					Projectile projectile = this.m_projectile;
					projectile.ModifyVelocity -= this.ModifyVelocity;
				}
				base.OnDestroy();
			}

			public float HomingRadius = 6f;

			public float AngularVelocity = 180f;

			protected Projectile m_projectile;
		}
	}
}
