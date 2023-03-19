using Alexandria.Misc;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class SanguineHookItem : PlayerItem
    {
        public SanguineGrappleModule hookData;

        public static GameObject hookPrefab;

        public const string PlaceholderSprite = "Oddments/Resources/example_item_sprite.png";

        public static ItemTemplate template = new ItemTemplate(typeof(SanguineHookItem))
        {
            Name = "Sanguine Hook",
            Description = "Crazy Battle",
            SpriteResource = $"{Module.SPRITE_PATH}/sanguinehook.png",
            Quality = ItemQuality.B,
            PostInitAction = item =>
            {
                /*SanguineHook hook = item as SanguineHook;
                SanguineGrappleModule hookData = new SanguineGrappleModule
                {*/
                hookPrefab = BaseAdvancedGrappleModule.GenerateHookPrefab("Sanguine Hook Grapple", new IntVector2(8, 8), PlaceholderSprite, PlaceholderSprite);
                /*};
                ETGModConsole.Log(hookData == null);
                hook.hookData = hookData;
                ETGModConsole.Log(hook.PickupObjectId);*/
                item.RemovePickupFromLootTables();
            }
        };

        public override void DoEffect(PlayerController user)
        {
            if (hookData == null)
            {
                hookData = new SanguineGrappleModule()
                {
                    hookPrefab = hookPrefab
                };
            }
            base.DoEffect(user);
            hookData.Trigger(user);
        }

        public class SanguineGrappleModule : BaseAdvancedGrappleModule
        {
            protected override void OnPreFireHook(PlayerController user, SpeculativeRigidbody hook)
            {
                return;
            }

            protected override void OnPreRigidBodyCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
            {
                base.OnPreRigidBodyCollision(myRigidbody, myCollider, otherRigidbody, otherCollider);
            }

            protected override void OnRigidBodyCollision(CollisionData rigidbodyCollision)
            {
                base.OnRigidBodyCollision(rigidbodyCollision);
            }
        }
    }
}
