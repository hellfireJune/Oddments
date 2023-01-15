using Alexandria.ItemAPI;
using Alexandria.Misc;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class KingWorm : GunBehaviour
    {
        public static GunTemplate gunTemplate = new GunTemplate(typeof(SpidAR))
        {
            Name = "King Worm",
            PostInitAction = gun =>
            {
                for (int i = 0; i < 4; i++)
                {
                    ProjectileModule module = gun.AddProjectileModuleFrom("ak-47", true, false);
                    gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                    gun.DefaultModule.angleVariance = 20f;
                    gun.DefaultModule.cooldownTime = 0.75f;
                    gun.DefaultModule.numberOfShotsInClip = 5;
                    Gun gun2 = PickupObjectDatabase.GetById(151) as Gun;
                    gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
                    if (i > 0)
                    {
                        module.ammoCost = 0;
                        module.ignoredForReloadPurposes = true;
                    }
                    else
                    {
                        gun.DefaultModule.ammoCost = 1;
                    }

                    Projectile projectile = Instantiate(module.projectiles[0]);
                    projectile.gameObject.SetActive(false);
                    FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                    UnityEngine.Object.DontDestroyOnLoad(projectile);
                    projectile.baseData.damage = 5;
                    projectile.gameObject.AddComponent<KingWormRandomModifier>();
                }

                gun.RemovePickupFromLootTables();
            }
        };
    }

    public class KingWormRandomModifier : BraveBehaviour
    {
        void Start()
        {
            if (projectile)
            {
                Action<Projectile> mod = BraveUtility.RandomElement(randomMod);
                mod.Invoke(projectile);
            }
        }

        private static List<Action<Projectile>> randomMod = new List<Action<Projectile>>()
        {
            projectile =>
            {
                projectile.gameObject.AddComponent<PulsingProjectileEffect>();
            },
            projectile =>
            {
                if (projectile.OverrideMotionModule == null)
                {
                    projectile.OverrideMotionModule = new HelixProjectileMotionModule();
                }
            },
            projectile =>
            {
                if (projectile.OverrideMotionModule == null)
                {
                    projectile.OverrideMotionModule = new HookWormMovementModifier();
                }
            },
            projectile =>
            {
                projectile.Speed *= 1.5f;
            },
            projectile =>
            {
                projectile.Speed *= 0.5f;
            }
        };
    }
}
