using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Gungeon;
using UnityEngine;
using JuneLib.Items;

namespace Oddments
{
    public class SpidAR : GunBehaviour
    {
        public static GunTemplate gunTemplate = new GunTemplate(typeof(SpidAR))
        {
            Name = "Spid-AR",
            PostInitAction = gun =>
            {
                gun.AddProjectileModuleFrom("ak-47", true, false);
                gun.DefaultModule.ammoCost = 1;
                gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
                gun.DefaultModule.burstShotCount = 3;
                gun.DefaultModule.burstCooldownTime = 0.1f;
                gun.DefaultModule.angleVariance = 0f;
                gun.DefaultModule.cooldownTime = 0.5f;
                gun.DefaultModule.numberOfShotsInClip = 20;
                Gun gun2 = PickupObjectDatabase.GetById(151) as Gun;
                gun.muzzleFlashEffects = gun2.muzzleFlashEffects;

                Projectile projectile = Instantiate(gun.DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 5;
                //projectile.gameObject.AddComponent<WebProjectile>();
                GoopModifier mod = projectile.gameObject.AddComponent<GoopModifier>();
                mod.goopDefinition = JuneLib.Status.EasyGoopDefinitions.PlayerFriendlyWebGoop;
                mod.CollisionSpawnRadius = 0.5f;
                gun.DefaultModule.projectiles[0] = projectile;

                gun.RemovePickupFromLootTables();
            }
        };
    }
}
