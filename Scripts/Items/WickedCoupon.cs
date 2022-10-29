using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using JuneLib.Items;

namespace Oddments
{
    [HarmonyPatch]
    public class WickedCoupon : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(WickedCoupon))
        {
            Name = "Wicked Coupon",
            Description = "15% off!",
            LongDescription = "Worth approximately 15 casings, just holding this will mean you can never go below 15 casings of currency",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/coupon.png",
            Quality = ItemQuality.C,
        };

        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            player.carriedConsumables.Currency = player.carriedConsumables.Currency;
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            base.DisableEffect(player);
        }

        [HarmonyPatch(typeof(PlayerConsumables), nameof(PlayerConsumables.Currency), MethodType.Setter)]
        [HarmonyPostfix]
        public static void ChangeCurrencyStuff(PlayerConsumables __instance)
        {
            int minCurrency = 0;
            if (IsFlagSetAtAll(typeof(WickedCoupon)))
            {
                minCurrency += 15;
            }
            if (IsFlagSetAtAll(typeof(MemberCard)))
            {
                foreach (PlayerController player in GameManager.Instance.AllPlayers)
                {
                    foreach (PassiveItem item in player.passiveItems)
                    {
                        if (item is MemberCard card)
                        {
                            minCurrency += card.amount;
                        }
                    }
                }
            }
            __instance.m_currency = Math.Max(__instance.m_currency, minCurrency);
            if (GameUIRoot.HasInstance)
            {
                GameUIRoot.Instance.UpdatePlayerConsumables(__instance);
            }
        }
        public class MemberCard : PassiveItem
        {
            public static ItemTemplate template = new ItemTemplate(typeof(MemberCard))
            {
                Name = "Member Card",
                SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/membercard.png",
                Description = "Bello's Special Customer",
                LongDescription = "Purchases at the shop will add 7.5% of what you spent into a minimum cap your casings can never fall below",
                Quality = ItemQuality.A,
            };

            public int amount;
            protected static readonly float increment = 0.075f;

            public override void Pickup(PlayerController player)
            {
                player.AddFlagsToPlayer(GetType());
                player.OnItemPurchased += Player_OnItemPurchased;
                base.Pickup(player);
            }

            private void Player_OnItemPurchased(PlayerController arg1, ShopItemController arg2)
            {
                if (arg2 == null
                    || arg2.CurrencyType != ShopItemController.ShopCurrencyType.COINS) {
                    return;
                }
                amount += (int)Math.Floor(arg2.CurrentPrice * increment);
                arg1.carriedConsumables.Currency = arg1.carriedConsumables.Currency;
            }

            public override void DisableEffect(PlayerController player)
            {
                player.RemoveFlagsFromPlayer(GetType());
                player.OnItemPurchased -= Player_OnItemPurchased;
                base.DisableEffect(player);
            }
        }
    }
}
