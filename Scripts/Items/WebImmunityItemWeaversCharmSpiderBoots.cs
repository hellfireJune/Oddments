using Alexandria.ItemAPI;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Oddments
{
    public class WebImmunityItem : PassiveItem
    {
        public static DeadlyDeadlyGoopManager ReplaceWebGoop(Func<GoopDefinition, DeadlyDeadlyGoopManager> orig, GoopDefinition goop)
        {
            if (IsFlagSetAtAll(typeof(WebImmunityItem))
                && goop.SpeedModifierEffect != null && goop.SpeedModifierEffect.effectIdentifier.StartsWith("phase web"))
            {
                return orig(EasyGoopDefinitions.PlayerFriendlyWebGoop);
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
        public static ItemTemplate template = new ItemTemplate(typeof(WeaversCharm))
        {
            Name = "Weaver's Charm",
            Quality = ItemQuality.D,
            Description = "Web",
            LongDescription = "A token of the phaser spider's friendship. Web will no longer affect you",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/weaverscharm.png",
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
        public static ItemTemplate template = new ItemTemplate(typeof(SpiderBoots))
        {
            Name = "Spider Boots",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/silkboots.png",
            Description = "Spin a web unseen",
            LongDescription = "Walking leaves a silk trail. Lets the user glide accross web unharmed, with gundead now being caught in it",
            Quality = ItemQuality.C,
        };

        public override void Update()
        {
            base.Update();
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
