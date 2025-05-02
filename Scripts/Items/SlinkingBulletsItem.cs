using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;

namespace Oddments
{
    internal class SlinkingBulletsItem : GunVolleyModificationItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(SlinkingBulletsItem))
        {
            Name = "Slinking Bullets",
            PostInitAction = item =>
            {
                GunVolleyModificationItem modItem = item as GunVolleyModificationItem;
                modItem.DuplicatesOfBaseModule = 1;

                modItem.AddPassiveStatModifier(PlayerStats.StatType.Damage, 0.72f);
                modItem.MakeBulletMod();
            },
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += PostProcess;
        }

        private void PostProcess(Projectile arg1, float arg2)
        {
            var mod = arg1.gameObject.AddComponent<SlinkingBulletsModifier>();
            mod.invert = invertWhipWhop;
            invertWhipWhop = !invertWhipWhop;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            player.PostProcessProjectile -= PostProcess;
        }

        public bool invertWhipWhop = false;

        public class SlinkingBulletsModifier : BraveBehaviour
        {
            public float minSize = 0.5f;
            public float maxSize = 2f;
            public float pulseSpeedMult = 0.05f;
            private float m_elapsedTick;
            private float m_lastDMG = 1f;
            public bool invert;

            void Start()
            {
                if (!projectile)
                {
                    return;
                }
                projectile.OnPostUpdate += Projectile_OnPostUpdate;
            }

            private void Projectile_OnPostUpdate(Projectile obj)
            {
                if (obj == null)
                    return;

                m_elapsedTick += BraveTime.DeltaTime * 100f;
                float sine = Mathf.Sin((m_elapsedTick * pulseSpeedMult) + (invert ? 0 : Mathf.PI));
                sine /= 4 / 3; sine += 0.85f;
                obj.Speed *= (1 / m_lastDMG) * sine;

                m_lastDMG = sine;
            }
        }
    }
}
