using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace Oddments
{
    //Original idea by "theguywhoasked9999" from the MTG discord
    [HarmonyPatch]
    public class SellDroppedItemsItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(SellDroppedItemsItem))
        {
            Name = "Sales 101",
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.AddFlagsToPlayer(GetType());
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            player.RemoveFlagsFromPlayer(GetType());
            return debrisObject;
        }

        public override void OnDestroy()
        {
            if (Owner)
            {
                Owner.AddFlagsToPlayer(GetType());
            }
            base.OnDestroy();
        }

        public static void DroppedItem(PlayerController player, DebrisObject item)
        {
            if (!IsFlagSetForCharacter(player, typeof(SellDroppedItemsItem))) { return; }
            if (!item.sprite) { return; }

            item.OnGrounded += db => {
                PickupObject po = db.GetComponent<PickupObject>();
                if (po != null) 
                {
                    int sellPrice = Mathf.Clamp(Mathf.CeilToInt((float)po.PurchasePrice*0.45f), 0, 200);
                    if (po.quality == ItemQuality.SPECIAL || po.quality == ItemQuality.EXCLUDED)
                    {
                        sellPrice = 3;
                    }
                    LootEngine.SpawnCurrency(po.sprite.WorldCenter, sellPrice, false);
                }

                LootEngine.DoDefaultItemPoof(po.sprite.WorldCenter);
                if (po is Gun && po.GetComponentInParent<DebrisObject>())
                {
                    Destroy(po.transform.parent.gameObject);
                } else
                {
                    Destroy(po.gameObject);
                }
            };
        }

        [HarmonyPatch(typeof(PassiveItem), nameof(PassiveItem.Drop))]
        [HarmonyPostfix]
        public static void DropPassive(PlayerController player, DebrisObject __result)
        {
            DroppedItem(player, __result);
        }
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.ForceDropGun))] 
        [HarmonyPostfix]
        public static void DropGun(PlayerController __instance, DebrisObject __result)
        {
            DroppedItem(__instance, __result);
        }

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.DropActiveItem))]
        [HarmonyPostfix] 
        public static void DropActive(PlayerController __instance, DebrisObject __result)
        {
            DroppedItem(__instance, __result);
        }
    }
}
