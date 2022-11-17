using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Alexandria.ItemAPI;

namespace Oddments
{
    [HarmonyPatch]
    public class HelixBulletsSinWave
    {
        public const string SynergyName = "Sin Wave";
        public static readonly List<string> IDs = new List<string>() { "helix_bullets", "cursed_bullets" };


        [HarmonyPatch(typeof(GunVolleyModificationItem), nameof(GunVolleyModificationItem.PostProcessProjectileHelix))]
        [HarmonyPostfix]
        public static void PostProcessProjectileHelix(Projectile obj, GunVolleyModificationItem __instance)
        {
            if (__instance.Owner.PlayerHasActiveSynergy(SynergyName))
            {
                if (obj.OverrideMotionModule != null && obj.OverrideMotionModule is HelixProjectileMotionModule)
                {
                    HelixProjectileMotionModule helixModifier = obj.OverrideMotionModule as HelixProjectileMotionModule;
                    helixModifier.helixAmplitude = 3f;
                    helixModifier.helixWavelength = 2f;
                }

                obj.baseData.damage *= 1.1f;
                obj.baseData.range *= 2f;
            }
        }
    }
}
