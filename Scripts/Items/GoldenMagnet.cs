using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using JuneLib.Items;

namespace Oddments
{
    [HarmonyPatch]
    public class GoldenMagnet : GoldMoverItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(GoldenMagnet))
        {
            Name = "Golden Magnet",
            Description = "More Money",
            LongDescription = "Holding this alone makes any money worth 30% more.\n\nBarely functional as a magnet, but it makes up for it in looks",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/goldenmagnet.png",
            Quality = ItemQuality.B,
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

        protected static readonly float chance = 0.3f;

        [HarmonyPatch(typeof(PlayerConsumables), nameof(PlayerConsumables.Currency), MethodType.Setter)]
        [HarmonyPrefix]
        public static void ChangeCurrencyStuff(PlayerConsumables __instance, ref int value)
        {
            if (IsFlagSetAtAll(typeof(GoldenMagnet)))
            {
                float num = value - __instance.m_currency;
                num *= chance;
                value += (int)(UnityEngine.Random.value < chance ? Math.Ceiling(num) : Math.Floor(num));
            }
        }
    }

    [HarmonyPatch]
    public class GoldMoverItem : PassiveItem
    {
        [HarmonyPatch(typeof(CurrencyPickup), nameof(CurrencyPickup.Start))]
        [HarmonyPostfix]
        public static void LetMove(CurrencyPickup __instance)
        {
            if (IsFlagSetAtAll(typeof(GoldMoverItem)))
            {
                PickupMover mover = __instance.GetComponent<PickupMover>();
                if (mover)
                {
                    mover.moveIfRoomUnclear = true;
                }
            }
        }
        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(typeof(GoldMoverItem));
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(typeof(GoldMoverItem));
            base.DisableEffect(player);
        }
    }
}
