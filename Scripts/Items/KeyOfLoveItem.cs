using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace Oddments
{
    [HarmonyPatch]
    public class KeyOfLoveItem : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(KeyOfLoveItem))
        {
            Name = "Key of Love",
            Quality = ItemQuality.B,
            Description = "Life => keys",
            LongDescription = "Turns hearts into keys, allowing you to use your life in place of keys, if you've ran out of keys.",
            SpriteResource = $"{Module.SPRITE_PATH}/howdoispritegungeon.png",
        };
        public override void Pickup(PlayerController player)
        {
            if (!this.m_pickedUpThisRun)
            {
                player.carriedConsumables.KeyBullets += 2;
            }
            player.AddFlagsToPlayer(GetType());
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            base.DisableEffect(player);
        }
        public static bool ShouldSkipHook = false;

        /*[HarmonyPatch(typeof(PlayerConsumables), nameof(PlayerConsumables.KeyBullets), MethodType.Getter)]
        public static void Getter(ref int __result)
        {
            if (LogIfSkip) { ETGModConsole.Log("will it skip?"); }
            if (ShouldSkipHook) { return;  }
            if (LogIfSkip) { ETGModConsole.Log("didnt skip"); }
            if (IsFlagSetAtAll(typeof(KeyOfLoveItem)))
            {
                if (LogIfSkip) { ETGModConsole.Log((int)GetAllHP()); }
                __result += (int)GetAllHP();
                if (LogIfSkip) { ETGModConsole.Log(__result); }
            }
        }

        public static int GetActualKeys(PlayerController player)
        {
            ShouldSkipHook = true;
            int keys = player.carriedConsumables.KeyBullets;
            ShouldSkipHook = false;
            return keys;
        }
        public static bool LogIfSkip = false;

        public static float GetAllHP()
        {
            return ((GameManager.Instance.PrimaryPlayer.healthHaver.GetCurrentHealth() - 0.5f) * 2
                    + (GameManager.Instance.SecondaryPlayer != null ? (GameManager.Instance.SecondaryPlayer.healthHaver.GetCurrentHealth() - 0.5f) * 2 : 0));
        }

        [HarmonyPatch(typeof(PlayerConsumables), nameof(PlayerConsumables.KeyBullets), MethodType.Setter)]
        public static void TakeHeartsWhenKey(PlayerConsumables __instance, ref int value)
        {
            if (!IsFlagSetAtAll(typeof(KeyOfLoveItem)))
            { return; }
            LogIfSkip = true;
            Debug.Log(__instance.KeyBullets);
            ETGModConsole.Log(value);
            ETGModConsole.Log(ShouldSkipHook);
            LogIfSkip = false;
            value -= (int)GetAllHP();
            if (value <= 0)
            {
                float dmgValue = Mathf.Abs((float)value) / 2;
                HealthHaver p1 = null;
                float? p1HP = GameManager.Instance.PrimaryPlayer?.healthHaver.GetCurrentHealth() * 2;
                if (p1HP.HasValue && p1HP > 1)
                {
                   p1 = GameManager.Instance.PrimaryPlayer.healthHaver;
                } else
                {
                    float? p2HP = GameManager.Instance.SecondaryPlayer?.healthHaver.GetCurrentHealth() * 2;
                    if (p2HP.HasValue && p2HP > 1)
                    {
                        p1 = GameManager.Instance.SecondaryPlayer.healthHaver;
                    }
                }
                value = 0;

                if (p1 == null) { return; }
                p1.ForceSetCurrentHealth(p1.GetCurrentHealth() - dmgValue/* - 0.5f);

            }
        }


        [HarmonyPatch(typeof(GameUIRoot), nameof(GameUIRoot.UpdatePlayerConsumables))]
        [HarmonyPrefix]
        public static void Shhh()
        {
            ShouldSkipHook = true;
        }
        [HarmonyPatch(typeof(GameUIRoot), nameof(GameUIRoot.UpdatePlayerConsumables))]
        [HarmonyPostfix]
        public static void No()
        {
            ShouldSkipHook = false;
        }*/
    }
}
