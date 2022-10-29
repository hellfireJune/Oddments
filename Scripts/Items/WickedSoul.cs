using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using HarmonyLib;
using Dungeonator;
using Alexandria.Misc;
using JuneLib.Items;

namespace Oddments
{ 
    [HarmonyPatch]
    public class WickedSoul : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(WickedSoul))
        {
            Name = "Wicked Soul",
            Description = "Something Wicked",
            Quality = ItemQuality.B,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 0.5f);

                item.SetupUnlockOnMaximum(TrackedMaximums.HIGHEST_CURSE_LEVEL, 9, DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN);
            }
        };
        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            base.DisableEffect(player);
        }

        [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.GetTotalCurse))]
        [HarmonyPostfix]
        public static void ThisWayComes(ref int __result)
        {
            if (IsFlagSetAtAll(typeof(WickedSoul)))
            {
                __result *= 2;
            }
        }

        [HarmonyPatch(typeof(Dungeon), nameof(Dungeon.SpawnCurseReaper))]
        [HarmonyPrefix]
        public static bool NoCurseReaperPlease()
        {
            if (IsFlagSetAtAll(typeof(WickedSoul))
                && PlayerStats.GetTotalCurse() < 20)
            {
                //return false;
            }
            return true;
        }
    }
}
