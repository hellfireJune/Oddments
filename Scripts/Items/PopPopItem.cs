using HarmonyLib;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oddments
{
    public class PopPopItem : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PopPopItem));

        public override bool CanBeUsed(PlayerController user)
        {
            Gun gun = user.CurrentGun;
            if (gun == null) { return false; }
            bool canFire = false;
            foreach (var mod in gun.m_moduleData.Keys)
            {
                var data = gun.m_moduleData[mod];
                if (!mod.ignoredForReloadPurposes && data.needsReload) { return false; }
                if (mod.shootStyle != ProjectileModule.ShootStyle.Beam) { canFire |= !data.onCooldown; }
            }
            if (!canFire) { return false; }
            bool ammo = gun.CurrentAmmo <= 1 || gun.IsReloading;
            if (ammo) { return false; }
            return !gun.IsFiring && !gun.IsGunBlocked() && !gun.IsHeroSword && base.CanBeUsed(user);
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            Gun gun = user.CurrentGun;
            ProjectileVolleyData volley = ScriptableObject.CreateInstance<ProjectileVolleyData>(); 
            if (gun.Volley == null) 
            {
                volley.projectiles = new List<ProjectileModule> { ProjectileModule.CreateClone(gun.DefaultModule) };
            } 
            else
            {
                volley.InitializeFrom(gun.Volley);
            }
            int count = volley.projectiles.Count;
            for (int i = 0; i < count; i++) {
                ProjectileModule mod = volley.projectiles[i];
                if (mod.shootStyle == ProjectileModule.ShootStyle.Beam
                    || (gun.m_moduleData.ContainsKey(mod) && (gun.m_moduleData[mod].needsReload || gun.m_moduleData[mod].onCooldown)))
                {
                    volley.projectiles.RemoveAt(i);
                    i--;
                    count--;
                    continue;
                }

                ProjectileModule mod2 = ProjectileModule.CreateClone(mod);
                volley.projectiles.Add(mod2);
            }

            if (volley.projectiles.Count <= 0) { return; }
            VolleyUtility.FireVolley(volley, gun.barrelOffset.position, BraveMathCollege.DegreesToVector(gun.CurrentAngle), user);

            ProjectileModule module = gun.DefaultModule;
            gun?.OnPostFired?.Invoke(user, gun);
            gun.HandleShootAnimation(module);
            gun.HandleShootEffects(module);
                if (module.runtimeGuid != null && gun.AdditionalShootSoundsByModule.ContainsKey(module.runtimeGuid))
                {
                    AkSoundEngine.SetSwitch("WPN_Guns", gun.AdditionalShootSoundsByModule[module.runtimeGuid], base.gameObject);
                }
                if (GameManager.AUDIO_ENABLED && (!gun.isAudioLoop || !gun.m_isAudioLooping))
                {
                    string in_pszEventName = (!module.IsFinalShot(gun.m_moduleData[module], user) || gun.OverrideFinaleAudio) ? "Play_WPN_gun_shot_01" : "Play_WPN_gun_finale_01";
                    if (!gun.PreventNormalFireAudio)
                    {
                        AkSoundEngine.PostEvent(in_pszEventName, base.gameObject);
                    }
                    else
                    {
                        AkSoundEngine.PostEvent(gun.OverrideNormalFireAudioEvent, base.gameObject);
                    }
                    gun.m_isAudioLooping = true;
                }
            DecreaseAmmo(gun);

            //gun.RawFireVolley(volley);
        }

        public static void DecreaseAmmo(Gun gun)
        {
            List<ProjectileModule> modules = gun.Volley.projectiles ?? new List<ProjectileModule>() { gun.DefaultModule };
            foreach (ProjectileModule module in modules) 
            { 
                gun.IncrementModuleFireCountAndMarkReload(module, null);
                GameManager.Instance.StartCoroutine(ModifiedModuleCooldown(gun, module));

                ProjectileModule mod2 = ProjectileModule.CreateClone(module);
                mod2.ammoCost *= 2;
                gun.DecrementAmmoCost(mod2);
            }
        }

        private static IEnumerator ModifiedModuleCooldown(Gun gun, ProjectileModule mod)
        {
            gun.m_moduleData[mod].onCooldown = true;
            float elapsed = 0f;
            float fireMultiplier = ((!(gun.m_owner is PlayerController)) ? 1f : (gun.m_owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.RateOfFire));
            if (gun.GainsRateOfFireAsContinueAttack)
            {
                float num = gun.RateOfFireMultiplierAdditionPerSecond * gun.m_continuousAttackTime;
                fireMultiplier += num;
            }
            float cooldownTime;
            if (mod.shootStyle == ProjectileModule.ShootStyle.Burst && gun.m_moduleData[mod].numberShotsFiredThisBurst < mod.burstShotCount)
            {
                cooldownTime = mod.burstCooldownTime;
            }
            else
            {
                cooldownTime = mod.cooldownTime + gun.gunCooldownModifier;
            }
            cooldownTime *= (1f / fireMultiplier)*2;
            while (elapsed < cooldownTime)
            {
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            if (gun.m_moduleData != null && gun.m_moduleData.ContainsKey(mod))
            {
                gun.m_moduleData[mod].onCooldown = false;
                gun.m_moduleData[mod].chargeTime = 0f;
                gun.m_moduleData[mod].chargeFired = false;
            }
            yield break;
        }
    }
}
