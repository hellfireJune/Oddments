using Alexandria.ItemAPI;
using JuneLib.Items;
using UnityEngine;

namespace Oddments
{
    public class PulsingBulletsItem : OddProjectileModifierItem
    {
        public static BulletModifierItemTemplate template = new BulletModifierItemTemplate(typeof(PulsingBulletsItem))
        {
            Name = "Hideous Bullets",
            AltTitle = "Viscerounds",
            Description = "Pulsating flesh",
            SpriteResource = $"{Module.SPRITE_PATH}/hideousbullets.png",
            LongDescription = "Bullets will pulsate in damage and size, leading to on average larger bullets with higher damage.",
            Quality = ItemQuality.B,
        };

        public override bool ApplyBulletEffect(Projectile arg1)
        {
            arg1.gameObject.AddComponent<PulsingProjectileEffect>();
            return true;
        }

    }
    public class PulsingProjectileEffect : BraveBehaviour
    {
        public float minSize = 0.5f;
        public float maxSize = 2f;
        public float minDamage = 0.8f;
        public float maxDamage = 1.4f;
        public float pulseSpeedMult = 0.1f;
        private float m_lastSize = 1f;
        private float m_elapsedTick;
        private float m_lastDMG = 1f;

        void Start()
        {
            if (!projectile)
            {
                return;
            }
            projectile.specRigidbody.UpdateCollidersOnScale = true;

            projectile.OnPostUpdate += Projectile_OnPostUpdate;
        }

        private void Projectile_OnPostUpdate(Projectile obj)
        {
            if (obj == null)
                return;

            m_elapsedTick += BraveTime.DeltaTime * 100f;
            float sine = Mathf.Sin(m_elapsedTick * pulseSpeedMult) / 2 + 0.5f;
            float mult = Mathf.Lerp(minSize, maxSize, sine);
            float dmgMult = Mathf.Lerp(minDamage, maxDamage, sine);
            obj.RuntimeUpdateScale((1 / m_lastSize) * mult);
            obj.baseData.damage *= (1 / m_lastDMG) * dmgMult;

            m_lastSize = mult;
            m_lastDMG = dmgMult;
        }
    }
}
