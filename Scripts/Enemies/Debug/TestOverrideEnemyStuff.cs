using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.EnemyAPI;
using Brave.BulletScript;
using JuneLib.Status;

namespace Oddments
{
    internal class TestOverrideEnemyStuff
    {
        public class TestOverrideBehavior : OverrideBehavior
        {
            public override string OverrideAIActorGUID { get => "b1770e0f1c744d9d887cc16122882b4f"; }

            public override void DoOverride()
            {
                AttackBehaviorGroup group = (AttackBehaviorGroup)actor.behaviorSpeculator.AttackBehaviors[0];
                ShootGunBehavior shoot = group.AttackBehaviors[0].Behavior as ShootGunBehavior;
                shoot.WeaponType = WeaponType.BulletScript;
                shoot.BulletScript = new CustomBulletScriptSelector(typeof(TestBulletShootScript));
                ShootBehavior chains = group.AttackBehaviors[1].Behavior as ShootBehavior;
                chains.BulletScript = new CustomBulletScriptSelector(typeof(TestChainShootScript));
                actor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));

                //actor.gameObject.AddComponent<NewTestComponent>();
                actor.SetResistance(EffectResistanceType.Poison, 1f);
            }
        }

        public class NewTestComponent : BraveBehaviour
        {
            void Update()
            {
                if (!healthHaver)
                {
                    return;
                }
                //aiActor.EffectResistances.add
            }

        }

        public class TestBulletShootScript : Script // This BulletScript is just a modified version of the script BulletManShroomed, which you can find with dnSpy.
        {
            public float angleVariance = 10;
            public float baseDistance = 10f;
            public int baseAdditionalBullets = 2;
            public override IEnumerator Top()
            {
                for (int i = -3; i < 3; i++)
                {
                    float variance = UnityEngine.Random.Range(-angleVariance, angleVariance);
                    Fire(new Direction((i*baseDistance) + variance, DirectionType.Aim), new Speed(10f, SpeedType.Absolute));
                }
                for (int i = 0; i < baseAdditionalBullets; i++)
                {
                    float newVariance = UnityEngine.Random.Range(-baseDistance * 2, baseDistance * 2);
                    Fire(new Direction(newVariance), new Speed(7f, SpeedType.Absolute));
                }
                yield break;
            }
        }

        public class TestChainShootScript : Script
        {
            public override IEnumerator Top()
            {
                base.Fire(new Direction(0, DirectionType.Aim), new Speed(4f, SpeedType.Absolute), new BigBullet());
                return base.Top();
            }

            public class BigBullet : Bullet
            {
                public override IEnumerator Top()
                {
                    float speed = 2f;
                    float elapsed = 0f;
                    DeadlyDeadlyGoopManager gooper =
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.PoisonDef);
                    while (!this.IsEnded)
                    {
                        elapsed++;
                        if (elapsed == 10)
                        {
                            elapsed = 0f;
                            this.ChangeSpeed(new Speed(speed, SpeedType.Relative));
                        }
                        gooper.AddGoopCircle(Position, 1f);
                        yield return null;
                    }
                }
                public BigBullet() : base("bigBullet", false, false, true)
                {

                }

                public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    if (!preventSpawningProjectiles)
                    {
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.PoisonDef).AddGoopCircle(Position, 3f);
                    }
                }
            }
        }
    }
}
