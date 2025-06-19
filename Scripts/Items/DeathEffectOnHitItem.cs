using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using System.Reflection;
using HarmonyLib;

namespace Oddments
{
    public class DeathEffectOnHitItem : OddProjectileModifierItem
    {
        public static BulletModifierItemTemplate template = new BulletModifierItemTemplate(typeof(DeathEffectOnHitItem))
        {
            Name = "Mortal Rounds",
            Description = "Dead shells",
            LongDescription = "Your bullets sometimes will activate enemy kill effects, aswell as doing increased damage.",
            SpriteResource = $"{Module.SPRITE_PATH}/mortalrounds.png",
            LongDescription = "Bullets have a chance to trigger enemy death and on-hit effects.",
            Quality = ItemQuality.A,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);
            },

            ProcChance = (1f / 80f)*0.1f,
            ChanceScalesWithDamageFired = true,
        };

        public override bool ApplyBulletEffect(Projectile projectile)
        {
            projectile.gameObject.AddComponent<DeathEffectOnHitModifier>();
            return true;
        }
    }

    internal class DeathEffectOnHitModifier : BraveBehaviour
    {
        private PlayerController m_owner;

        public void Start()
        {
            if (!projectile || !projectile.Owner) { return; }
            m_owner = projectile.Owner as PlayerController;

            projectile.OnHitEnemy += OnHitEnemy;
        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2 && arg2.healthHaver)
            {
                Type playerController = typeof(PlayerController);
                FieldInfo _onKilledEnemy = playerController.GetField(nameof(PlayerController.OnKilledEnemy), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                FieldInfo _onKilledEnemyContext = playerController.GetField(nameof(PlayerController.OnKilledEnemyContext), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                Action<PlayerController> killedEnemy = (Action<PlayerController>)_onKilledEnemy.GetValue(m_owner);
                if (killedEnemy != null)
                {
                    killedEnemy.Invoke(m_owner);
                }

                Action<PlayerController, HealthHaver> killedEnemyContext = (Action<PlayerController, HealthHaver>)_onKilledEnemyContext.GetValue(m_owner);
                if (killedEnemyContext != null)
                {
                    killedEnemyContext.Invoke(m_owner, arg2.healthHaver);
                }

                m_owner.OnAnyEnemyTookAnyDamage(27.616f, true, arg2.healthHaver);
            }
        }
    }
}
