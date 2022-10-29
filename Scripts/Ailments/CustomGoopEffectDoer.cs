using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;

namespace Oddments
{
    [HarmonyPatch]
    public class CustomGoopEffectDoer : MonoBehaviour
    {
        public CustomGoopEffectDoer()
        {
            IsCloner = false;
        }

        public bool IsCloner;


        [HarmonyPatch(typeof(DeadlyDeadlyGoopManager), nameof(DeadlyDeadlyGoopManager.DoGoopEffect))]
        [HarmonyPostfix]
        public static void CoolNewCustomGoopEffects(DeadlyDeadlyGoopManager __instance, GameActor actor, IntVector2 goopPosition)
        {
            if (actor is PlayerController ||
                !(actor is AIActor aiactor) || aiactor.CompanionOwner)
            {
                return;
            }
            if (!actor || actor.aiAnimator.IsPlaying("spawn") || actor.aiAnimator.IsPlaying("awaken"))
            {
                return;
            }

                CustomGoopEffectDoer customGoopProcessor = __instance.GetComponent<CustomGoopEffectDoer>();
            if (customGoopProcessor != null)
            {
                CustomEffectHandler effecthandler = actor.gameObject.GetOrAddComponent<CustomEffectHandler>();
                if (customGoopProcessor.IsCloner)
                {
                    bool result = effecthandler.IncrementCloneTick();
                    if (result)
                    {

                        AIActor prefab = EnemyDatabase.GetOrLoadByGuid(aiactor.EnemyGuid);
                        if (prefab != null)
                        {
                            AIActor clone = AIActor.Spawn(prefab, actor.CenterPosition.ToIntVector2(), actor.GetAbsoluteParentRoom(), true);
                            LootEngine.DoDefaultItemPoof(clone.Position);
                        }
                    }

                    __instance.RemoveGoopedPosition(goopPosition);
                }
            }
        }

        public class CustomEffectHandler : BraveBehaviour
        {
            protected float CloneTick;

            public bool IncrementCloneTick()
            {
                if (aiActor && aiActor.healthHaver
                    && 1 > aiActor.GetResistanceForEffectType(EffectResistanceType.Charm))
                {
                    CloneTick += 0.25f;//0.2f * (24f / aiActor.healthHaver.GetMaxHealth());
                    ETGModConsole.Log(CloneTick);
                    if (CloneTick >= 1)
                    {
                        CloneTick = 0;
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
