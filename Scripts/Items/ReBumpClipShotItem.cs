using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    internal class ReBumpClipShotItem : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ReBumpClipShotItem));
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.GetComponent<JEventsComponent>().PostProcessProjectileMod += Player_PostProcessProjectile;
        }

        public override void DisableEffect(PlayerController player)
        {
            player.GetComponent<JEventsComponent>().PostProcessProjectileMod -= Player_PostProcessProjectile;
            base.DisableEffect(player);
        }

        private void Player_PostProcessProjectile(Projectile arg1, Gun arg2, ProjectileModule arg3)
        {
            RebumpModifier mod = arg1.gameObject.AddComponent<RebumpModifier>();
            mod.module = arg3;
            mod.gun = arg2;
        }

        public class RebumpModifier : BraveBehaviour
        {
            public ProjectileModule module;
            public Gun gun;

            void Start()
            {
                if (projectile)
                {
                    projectile.OnHitEnemy += HitEnemy;
                }
            }

            private void HitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
            {
                int numBullets = 1;

                int num = Mathf.Min(numBullets, gun.m_moduleData[module].numberShotsFired);
                int num2 = module.GetModNumberOfShotsInClip(gun.CurrentOwner) - gun.m_moduleData[module].numberShotsFired;
                num = Mathf.Min(num, gun.ammo - num2);
                if (num > 0)
                {
                    gun.m_moduleData[module].numberShotsFired -= num;
                    gun.m_moduleData[module].needsReload = false;
                }

                if (gun.Volley != null)
                {
                    for (int i = 0; i < gun.Volley.projectiles.Count; i++)
                    {
                        ProjectileModule mod2 = gun.Volley.projectiles[i];
                        if (gun.DefaultModule == mod2 || (mod2.IsDuctTapeModule && mod2.ammoCost > 0))
                        {
                            continue;
                        }

                        int num3 = mod2.GetModNumberOfShotsInClip(gun.CurrentOwner);
                        int num4 = gun.m_moduleData[mod2].numberShotsFired;
                        if (num3 > num4) { continue; }

                        num = Mathf.Min(numBullets, gun.m_moduleData[mod2].numberShotsFired);
                        num2 = num3 - num4;
                        num = Mathf.Min(num, gun.ammo - num2);
                        if (num > 0)
                        {
                            gun.m_moduleData[mod2].numberShotsFired -= num;
                            gun.m_moduleData[mod2].needsReload = false;
                        }
                    }
                }
            }
        }


        /*private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            Gun gun = arg1.PossibleSourceGun;
            if (gun == null)
            {
                Debug.Log("gun is null");
                return;
            }
            ProjectileModule module = gun.m_moduleData.Keys.First(proj => (proj.projectiles != null && proj.projectiles.Contains(arg1))/* || 
            /*(proj.chargeProjectiles != null && proj.chargeProjectiles.Where(charged => charged.Projectile == arg1).Count() != 0));
            Debug.Log(module == null);
        }*/
    }
}
