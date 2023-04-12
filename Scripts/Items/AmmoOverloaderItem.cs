using Alexandria.Misc;
using HarmonyLib;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    [HarmonyPatch]
    public class AmmoOverloaderItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(AmmoOverloaderItem))
        {
            Name = "overloader",
            PostInitAction = item =>
            {
                AmmoOverloaderItem aitem = item as AmmoOverloaderItem;
                aitem.DoOverload = true;
                aitem.SpreadAmmoBonusPercent = 0.25f;
                aitem.SplitForOtherMult = 0f;
            }
        };
        public static OddItemTemplate template2 = new OddItemTemplate(typeof(AmmoOverloaderItem))
        {
            Name = "splitter",
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.GetExtComp().OnPickedUpAmmo += PickUpAmmo;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            player.GetExtComp().OnPickedUpAmmo -= PickUpAmmo;
        }

        private void PickUpAmmo(PlayerController player, AmmoPickup arg2)
        {
            Gun gun = player.CurrentGun;
            if (arg2.mode == AmmoPickup.AmmoPickupMode.FULL_AMMO)
            {
                if (DoOverload)
                {
                    AmmoExtenderComponent extender = gun.gameObject.GetOrAddComponent<AmmoExtenderComponent>();
                    Skip = true;
                    extender.AmmoAdded += Mathf.FloorToInt(gun.AdjustedMaxAmmo * 0.5f);
                    Skip = false;
                    gun.GainAmmo((int)(gun.AdjustedMaxAmmo * 0.5));
                }
            }
            float splitMult = SplitForOtherMult;
            if (arg2.mode == AmmoPickup.AmmoPickupMode.SPREAD_AMMO)
            {
                splitMult = SpreadAmmoBonusPercent / 2;
                gun.GainAmmo(Mathf.CeilToInt((float)gun.AdjustedMaxAmmo * SpreadAmmoBonusPercent));
                gun.ForceImmediateReload(false);
            }
            if (splitMult != 0)
            {
                for (int i = 0; i < player.inventory.AllGuns.Count; i++)
                {
                    if (player.inventory.AllGuns[i] && gun != player.inventory.AllGuns[i])
                    {
                        player.inventory.AllGuns[i].GainAmmo(Mathf.FloorToInt(player.inventory.AllGuns[i].AdjustedMaxAmmo * splitMult));
                    }
                }
            }
        }

        public float SpreadAmmoBonusPercent = 0.5f;
        public bool DoOverload = false;
        public float SplitForOtherMult = 0.25f;

        public class AmmoExtenderComponent : BraveBehaviour
        {
            private int ammoAdded;
            public int AmmoAdded
            {
                get { return ammoAdded; }
                set { 
                    ammoAdded = Math.Max(0, value); 
                    if (ammoAdded == 0)
                    {
                        Destroy(this);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.AdjustedMaxAmmo), MethodType.Getter)]
        [HarmonyPostfix]
        public static void ChangeResults(Gun __instance, ref int __result)
        {
            if (Skip) { return; }
            AmmoExtenderComponent extender = __instance.GetComponent<AmmoExtenderComponent>();
            if (extender != null) { __result += extender.AmmoAdded; }
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.LoseAmmo))]
        [HarmonyPostfix]
        public static void Fucker(Gun __instance, int amt)
        {
            AmmoExtenderComponent extender = __instance.GetComponent<AmmoExtenderComponent>();
            if (extender != null) { extender.AmmoAdded -= amt; Debug.Log(__instance.AdjustedMaxAmmo); }
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.DecrementAmmoCost))]
        [HarmonyPostfix]
        public static void DP(Gun __instance, ProjectileModule module)
        {
            AmmoExtenderComponent extender = __instance.GetComponent<AmmoExtenderComponent>();
            if (extender != null)
            {
                int decrement = module.ammoCost;
                if (module.shootStyle == ProjectileModule.ShootStyle.Charged)
                {
                    ProjectileModule.ChargeProjectile chargeProjectile = module.GetChargeProjectile(__instance.m_moduleData[module].chargeTime);
                    if (chargeProjectile.UsesAmmo)
                    {
                        decrement = chargeProjectile.AmmoCost;
                    }
                }
                extender.AmmoAdded -= decrement; Debug.Log(__instance.AdjustedMaxAmmo);
            }
        }


        private static bool Skip = false;
        [HarmonyPatch(typeof(GameUIAmmoController), nameof(GameUIAmmoController.UpdateUIGun))]
        [HarmonyPrefix]
        public static void SkipMethod()
        {
            Skip = true;
        }
        [HarmonyPatch(typeof(GameUIAmmoController), nameof(GameUIAmmoController.UpdateUIGun))]
        [HarmonyPostfix]
        public static void unskip()
        {
            Skip = false;
        }
    }
}
