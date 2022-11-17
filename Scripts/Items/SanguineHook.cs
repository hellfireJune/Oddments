using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class SanguineHook : PlayerItem
    {
        public SanguineGrappleModule hookData;

        public static GameObject hookPrefab;

        public const string PlaceholderSprite = "Oddments/Resources/example_item_sprite.png";

        public static ItemTemplate template = new ItemTemplate(typeof(SanguineHook))
        {
            Name = "Sanguine Hook",
            Quality = ItemQuality.EXCLUDED,
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
            ETGModConsole.Log(hookData == null);
            hookData.Trigger(user);
        }

        public class SanguineGrappleModule : BaseAdvancedGrappleModule
        {
            protected override void OnPreFireHook(PlayerController user, SpeculativeRigidbody hook)
            {
                return;
            }

            
        }
    }
}
