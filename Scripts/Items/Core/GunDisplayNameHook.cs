using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    [HarmonyPatch]
    internal class GunDisplayNameHook
    {
        [HarmonyPatch(typeof(Gun), nameof(Gun.DisplayName), MethodType.Getter)]
        [HarmonyPostfix]
        public static void MakeWet(Gun __instance, ref string __result)
        {
            var whetStoneWaxStoneComponent = __instance.GetComponent<WhetStoneWaxStoneItem.SlightlyBoostComponent>();
            if (whetStoneWaxStoneComponent != null)
            {
                string str = "";

                bool waxed = false;
                if (whetStoneWaxStoneComponent.WaxLevel > 0)
                {
                    waxed = true;
                    str += WhetStoneWaxStoneItem.SlightlyBoostComponent.powerLevel[whetStoneWaxStoneComponent.WaxLevel - 1] + "Waxed ";
                }

                if (whetStoneWaxStoneComponent.SharpnessLevel > 0)
                {
                    if (waxed)
                    {
                        str += "and ";
                    }
                    str += WhetStoneWaxStoneItem.SlightlyBoostComponent.powerLevel[whetStoneWaxStoneComponent.SharpnessLevel - 1] + "Sharpened ";
                }

                __result = str + __result;
            }

            if (__instance.GetComponent<NewFloorGunModItem>())
            {
                __result = "Shelltan's Own " + __result;
            }
        }
    }
}
