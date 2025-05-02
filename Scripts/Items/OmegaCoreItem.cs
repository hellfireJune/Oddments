using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class OmegaCoreItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(OmegaCoreItem))
        {
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            ETGMod.AIActor.OnPostStart += OnPostStart;
        }

        private void OnPostStart(AIActor obj)
        {
            if (obj == null || obj.CompanionOwner != null || !obj.IsNormalEnemy)
            {
                return;
            }
            obj.specRigidbody.OnTileCollision += BounceOffWalls;
            if (obj.knockbackDoer != null)
            {
                obj.gameObject.AddComponent<MoreKnockbackComponent>();
            }
        }

        private void BounceOffWalls(CollisionData tileCollision)
        {
            Vector2 normal = tileCollision.Normal;
            if (tileCollision.MyRigidbody != null)
            {
                SpeculativeRigidbody rigidBod = tileCollision.MyRigidbody;
                if (rigidBod.knockbackDoer)
                {

                    Vector2 velocity = rigidBod.Velocity;
                    float velAng = (-velocity).ToAngle();
                    float disAng = normal.ToAngle();
                    float knockBackAngle = BraveMathCollege.ClampAngle360(velAng + 2f * (disAng - velAng));

                    rigidBod.knockbackDoer.ApplyKnockback(BraveMathCollege.DegreesToVector(knockBackAngle), 20f);

                    if (rigidBod.healthHaver)
                    {
                        rigidBod.healthHaver.ApplyDamage(56, Vector2.zero, "knocback");
                    }
                }


            }
        }
    }

    internal class MoreKnockbackComponent : BraveBehaviour
    {
        void Update()
        {
            if (aiActor)
            {
                aiActor.KnockbackVelocity *= 2f;
            }
        }
    }
}
