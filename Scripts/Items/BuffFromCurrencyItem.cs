using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace Oddments
{
    [HarmonyPatch]
    public class BuffFromCurrencyItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(BuffFromCurrencyItem))
        {
            Name = "metamoney to damage",
        };

        public static OddItemTemplate template2 = new OddItemTemplate(typeof(BuffFromCurrencyItem))
        {
            Name = "reg money to curse damage",

            PostInitAction = item =>
            {
                var money = item as BuffFromCurrencyItem;
                money.incrementsNeeded = 60;
                money.statTypes = new PlayerStats.StatType[] { PlayerStats.StatType.Damage, PlayerStats.StatType.Curse};
                money.damagePer = 0.075f;
                money.metaCurrency = false;
            }
        };

        public float damagePer = 0.01f;
        public PlayerStats.StatType[] statTypes = new PlayerStats.StatType[] { PlayerStats.StatType.Damage };
        public bool metaCurrency = true;

        public int incrementsNeeded = 1;
        public int increments = 0;

        [HarmonyPatch(typeof(CurrencyPickup), nameof(CurrencyPickup.Pickup))]
        public static void CurrencyPickupPatch(CurrencyPickup __instance, PlayerController player)
        {

            var metaItems = player.GetComponentsInChildren<BuffFromCurrencyItem>();
            foreach (var metaItem in metaItems)
            {
                if (__instance.IsMetaCurrency != metaItem.metaCurrency) { continue; }

                metaItem.increments += __instance.currencyValue;
                while (metaItem.increments >= metaItem.incrementsNeeded)
                {
                    metaItem.increments -= metaItem.incrementsNeeded;
                    foreach (var type in metaItem.statTypes)
                    {
                        float value = type == PlayerStats.StatType.Curse ? 1 : metaItem.damagePer;
                        player.ownerlessStatModifiers.Add(new StatModifier() { amount = value, statToBoost = type, modifyType = StatModifier.ModifyMethod.ADDITIVE });
                    }
                }
            }
        }
    }

}
