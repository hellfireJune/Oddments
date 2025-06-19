﻿using Alexandria.ItemAPI;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JuneLib.Items;
using JuneLib.Status;
using JuneLib;
using Dungeonator;

namespace Oddments
{
    public class WebImmunityItem : PassiveItem
    {
        public static DeadlyDeadlyGoopManager ReplaceWebGoop(Func<GoopDefinition, DeadlyDeadlyGoopManager> orig, GoopDefinition goop)
        {
            if (IsFlagSetAtAll(typeof(WebImmunityItem))
                && goop.SpeedModifierEffect != null && goop.SpeedModifierEffect.effectIdentifier.StartsWith("phase web"))
            {
                DeadlyDeadlyGoopManager gooper = orig(EasyGoopDefinitions.PlayerFriendlyWebGoop);
                /*if (gooper.GetComponent<CustomGoopEffectDoer>() == null)
                {
                    CustomGoopEffectDoer splooger = gooper.gameObject.AddComponent<CustomGoopEffectDoer>();
                    splooger.IsCloner = true;
                }*/
                return gooper;
            }
            return orig(goop);
        }
        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(typeof(WebImmunityItem));
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(typeof(WebImmunityItem));
            base.DisableEffect(player);
        }
    }

    public class WeaversCharm : WebImmunityItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(WeaversCharm))
        {
            Name = "Weaver's Charm",
            Quality = ItemQuality.D,
            Description = "Web",
            LongDescription = "A token of the phaser spider's friendship. Phaser Spider's are allied with you, and web will now affect the gundead instead",
            SpriteResource = $"{Module.SPRITE_PATH}/weaverscharm.png",
            PostInitAction = item =>
            {
                Hook hook = new Hook(typeof(DeadlyDeadlyGoopManager).GetMethod("GetGoopManagerForGoopType", BindingFlags.Static | BindingFlags.Public), typeof(WebImmunityItem).GetMethod("ReplaceWebGoop"));
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            ETGMod.AIActor.OnPreStart += OnPreStart;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            ETGMod.AIActor.OnPreStart -= OnPreStart;
        }

        public void OnPreStart(AIActor actor)
        {
            if (actor && actor.EnemyGuid == "98ca70157c364750a60f5e0084f9d3e2")
            {
                actor.AddPermanentCharm();
            }
        }


    }

    public class SpiderBoots : WebImmunityItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(SpiderBoots))
        {
            Name = "Spider Boots",
            SpriteResource = $"{Module.SPRITE_PATH}/silkboots.png",
            Description = "Spin a web unseen",
            LongDescription = "Walking leaves a silk trail. Lets the user glide accross web unharmed, with gundead now being caught in it",
            Quality = ItemQuality.C,
        };

        public override void Update()
        {
            base.Update();

            // The game does not like when goop is generated while a floor is loading
            if (!GameManager.HasInstance || GameManager.Instance.Dungeon == null)
                return;

            if (Dungeon.IsGenerating)
                return;

            if (Owner && Owner.specRigidbody)
            {
                DeadlyDeadlyGoopManager manager = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.PlayerFriendlyWebGoop);
                if (Owner.Velocity.magnitude > 0
                    && (UnityEngine.Random.value < 0.1f || Owner.CurrentRollState == PlayerController.DodgeRollState.InAir) && !Owner.IsOnFire)
                {
                    manager.AddGoopCircle(Owner.specRigidbody.UnitCenter, 0.75f);
                }
            }
        }
    }
}
