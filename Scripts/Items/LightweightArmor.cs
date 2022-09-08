using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Alexandria.ItemAPI;

namespace Oddments
{
    [HarmonyPatch]
    public class LightweightArmor : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(LightweightArmor))
        {
            Name = "Lightweight Armour",
            LongDescription = "Good for nimble movement, but makes it much harder for a gungeoneer to keep their footing",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.MovementSpeed, 1.5f);
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.AddFlagsToPlayer(GetType());
        }

        public override void DisableEffect(PlayerController disablingPlayer)
        {
            base.DisableEffect(disablingPlayer);
            disablingPlayer.RemoveFlagsFromPlayer(GetType());
        }

        //tots feel like just making it the item update stuff will make it Not Work
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Update))]
        [HarmonyPrefix]
        public static void DoMomentumStuff(PlayerController __instance)
        {
            if (GameManager.Instance.IsPaused || GameManager.Instance.UnpausedThisFrame)
            {
                return;
            }
            if (GameManager.Instance.IsLoadingLevel)
            {
                return;
            }
            if (IsFlagSetForCharacter(__instance, typeof(LightweightArmor)))
            {
                __instance.knockbackComponent *= 2;
            }
        }
    }
}
