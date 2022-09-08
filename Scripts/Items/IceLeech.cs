using System;
using HarmonyLib;
using Alexandria.Misc;

namespace Oddments
{
    [HarmonyPatch]
    [HarmonyPatch]
    public class IceLeech : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(IceLeech))
        {
            Name = "Frost Leech",
            Quality = ItemQuality.D,
        };
        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            player.healthHaver.ModifyHealing += ModifyHealing;
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            player.healthHaver.ModifyHealing += ModifyHealing;
            base.DisableEffect(player);
        }

        protected float dmgChargePerHeal = 150;
        protected int roomChargePerHeal = 2;
        private void ModifyHealing(HealthHaver arg1, HealthHaver.ModifyHealingEventArgs arg2)
        {
            if (arg2 == EventArgs.Empty)
            {
                return;
            }
            float chargeToAdd = arg2.ModifiedHealing * 2;
            ETGModConsole.Log(chargeToAdd);

            PlayerController playerController = arg1.GetComponent<PlayerController>();
            if (playerController != null && playerController.activeItems.Count > 0
                && AnyItemsNeedHealing(playerController))
            {
                foreach (var item in playerController.activeItems)
                {
                    item.CurrentDamageCooldown = Math.Max(item.CurrentDamageCooldown - (dmgChargePerHeal * chargeToAdd), 0);
                    item.CurrentRoomCooldown = (int)Math.Max(item.CurrentRoomCooldown - (roomChargePerHeal * chargeToAdd), 0);
                }
                arg2.ModifiedHealing *= 0;
            }
        }

        public static bool AnyItemsNeedHealing(PlayerController player)
        {
            foreach (var item in player.activeItems)
            {
                if (item.CurrentDamageCooldown > 0 || item.CurrentRoomCooldown > 0)

                return true;
            }
            return false;
        }

        [HarmonyPatch(typeof(HealthPickup), "PrePickupLogic")]
        [HarmonyPrefix]
        public static void HandlePickupLogic(HealthPickup __instance, SpeculativeRigidbody otherRigidbody, SpeculativeRigidbody selfRigidbody)
        {
            PlayerController playerController = otherRigidbody.GetComponent<PlayerController>();
            if (playerController == null
                || playerController.IsGhost
                || !IsFlagSetForCharacter(playerController, typeof(IceLeech))
                
                || __instance.healAmount <= 0) {
                return;
            }

            if (AnyItemsNeedHealing(playerController))
            {
                __instance.Pickup(playerController);
                Destroy(__instance.gameObject);
            }
        }

        [HarmonyPatch(typeof(HealthPickup), "OnEnteredRange")]
        [HarmonyPrefix]
        public static bool CantSlurp(HealthPickup __instance, PlayerController interactor)
        {
            if (!__instance
                || !IsFlagSetForCharacter(interactor, typeof(IceLeech))
                || !AnyItemsNeedHealing(interactor)) 
            {
                return true;
            }
            return false;
        }

        [HarmonyPatch(typeof(HealthPickup), nameof(HealthPickup.Interact))]
        [HarmonyPrefix]
        public static bool CantGlurp(PlayerController interactor)
        {
            if (!IsFlagSetForCharacter(interactor, typeof(IceLeech))
                ||!AnyItemsNeedHealing(interactor))
            {
                return true;
            }
            return false;
        }
    }
}
