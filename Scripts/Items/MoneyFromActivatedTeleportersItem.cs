using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace Oddments
{
    [HarmonyPatch]
    public class MoneyFromActivatedTeleportersItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(VolleyOnBlankItem))
        {
            
        };

        public static int MoneyMadeThisFloor = 0;

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                MoneyMadeThisFloor = 0;
            }
                base.Pickup(player);
            player.AddFlagsToPlayer(GetType());
            player.OnNewFloorLoaded += _ =>
            {
                MoneyMadeThisFloor = 0;
            };
        }

        public static int MoneyToBeMadeThisFloor = 45;

        [HarmonyPatch(typeof(TeleporterController), nameof(TeleporterController.TriggerActiveVFX))]
        public static void Fuck(TeleporterController __instance)
        {
            int randNum = UnityEngine.Random.Range(1, 5);
            randNum = UnityEngine.Mathf.Min(randNum, Math.Max(1, MoneyToBeMadeThisFloor* - MoneyMadeThisFloor));
            MoneyMadeThisFloor -= randNum;

            LootEngine.SpawnCurrency(__instance.sprite.WorldCenter, randNum);
        }
    }
}
