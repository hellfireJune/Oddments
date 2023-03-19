using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using System.Collections;
using JuneLib.Status;

namespace Oddments
{
    [HarmonyPatch]
    public class QuickFreezeItem : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(QuickFreezeItem));

        [HarmonyPatch(typeof(GameActor), nameof(GameActor.ApplyEffect))]
        [HarmonyPrefix]
        public static void ChangeEffectApplied(GameActor __instance, ref GameActorEffect effect)
        {
            if (!IsFlagSetAtAll(typeof(QuickFreezeItem))
                || __instance is PlayerController)
            {
                return;
            }
            
            if (effect is GameActorFreezeEffect freeze)
            {
                QuickFreezeTimerHandler quickFreezeTimerHandler = __instance.gameObject.GetOrAddComponent<QuickFreezeTimerHandler>();
                if (quickFreezeTimerHandler != null
                    && quickFreezeTimerHandler.CanDoQuickFreeze()
                    && UnityEngine.Random.value < 0.18)
                {
                    freeze.FreezeAmount = __instance.healthHaver?.IsBoss == true ? 100 : 150;
                    quickFreezeTimerHandler.DoFreezeIEnumerator();
                }
            }
        }

        public class QuickFreezeTimerHandler : BraveBehaviour
        {
            public QuickFreezeTimerHandler() { m_canDoQuickFreeze = true; }

            private bool m_canDoQuickFreeze;
            private float m_coolDown = 6f;
            public bool CanDoQuickFreeze()
            {
                return m_canDoQuickFreeze;
            }
            public void DoFreezeIEnumerator()
            {
                m_canDoQuickFreeze = false;
                StartCoroutine(FreezeTimeGirlies());
            }

            private IEnumerator FreezeTimeGirlies()
            {
                float elapsed = 0f;
                while (elapsed < m_coolDown)
                {
                    if (!aiActor.IsFrozen)
                    {
                        elapsed += BraveTime.DeltaTime;
                    }
                    yield return null;
                }
                m_canDoQuickFreeze = true;
                yield break;
            }
        }

        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            player.PostProcessProjectile += Player_PostProcessProjectile;
            base.Pickup(player);
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            if (Owner.CurrentGun == null
                || Owner.CurrentGun.ClipShotsRemaining == 0) 
            { return; }

            if (UnityEngine.Random.value < 0.18)
            {
                arg1.statusEffectsToApply.Add(GenericStatusEffects.frostBulletsEffect);
                arg1.AdjustPlayerProjectileTint(GenericStatusEffects.frostBulletsEffect.TintColor, 1);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            player.PostProcessProjectile -= Player_PostProcessProjectile;
            base.DisableEffect(player);
        }
    }
}
