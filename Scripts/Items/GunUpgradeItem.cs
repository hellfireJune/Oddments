using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace Oddments
{
    [HarmonyPatch]
    public class GunUpgradeItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(GunUpgradeItem));

        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            if (!m_pickedUpThisRun)
            {
                JuneLib.Chests.ChestHelpers.ForEveryChest(chest =>
                {
                    if (chest != null && chest.contents != null && chest.contents.Count != 0 && chest.ChestType == Chest.GeneralChestType.WEAPON)
                    {
                        chest.contents = null;
                        chest.DetermineContents(player);
                    }
                });
            }
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            base.DisableEffect(player);
        }

        [HarmonyPatch(typeof(Chest), nameof(Chest.GenerateContents))]
        [HarmonyPrefix]
        public static void OverrideContentsGen(Chest __instance, ref int tierShift)
        {
            if (IsFlagSetAtAll(typeof(GunUpgradeItem)) && __instance.ChestType == Chest.GeneralChestType.WEAPON)
            {
                tierShift += 1;
            }
        }
    }
}
