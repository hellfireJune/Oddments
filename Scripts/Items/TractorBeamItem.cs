using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.Misc;

namespace Oddments
{
    public class TractorBeamItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(TractorBeamItem))
        {
            PostInitAction = item =>
            {
                item.RemovePickupFromLootTables();
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += Player_PostProcessProjectile;
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            arg1.OverrideMotionModule = new TractorBeamMotionModule();
            arg1.OnPostUpdate += Arg1_OnPostUpdate;
        }

        private void Arg1_OnPostUpdate(Projectile obj)
        {
            obj.OverrideMotionModule = new TractorBeamMotionModule();
            obj.OnPostUpdate -= Arg1_OnPostUpdate;
        }

        public class TractorBeamMotionModule : ProjectileMotionModule
        {
            public override void Move(Projectile source, Transform projectileTransform, tk2dBaseSprite projectileSprite, SpeculativeRigidbody specRigidbody, ref float m_timeElapsed, ref Vector2 m_currentDirection, bool Inverted, bool shouldRotate)
            {
                throw new NotImplementedException();
            }

            public override void UpdateDataOnBounce(float angleDiff)
            {
                throw new NotImplementedException();
            }
        }
    }
}
