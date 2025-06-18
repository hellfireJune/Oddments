using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using JuneLib.Status;
using JuneLib;

namespace Oddments
{
    [Serializable]
    public class GameActorOnDeathEffect : GameActorHealthEffect
    {
        protected Type onDeathBehaviour;
        public OnDeathBehavior.DeathType deathType;
        protected Component comp;
        public Action<Component> PostApplyAction;
        public GameActorOnDeathEffect(Type behaviour) : base()
        {
            onDeathBehaviour = behaviour;
        }

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            Debug.Log("tried to apply effect");
            base.OnEffectApplied(actor, effectData, partialAmount);
            Debug.Log("added effect");
            comp = actor.gameObject.AddComponent(onDeathBehaviour);
            Debug.Log("saved component");
            PostApplyAction?.Invoke(comp);
            Debug.Log("applied action");
            if (comp is OnDeathBehavior onDeath)
            {
                onDeath.deathType = deathType;
            }
        }

        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            base.OnEffectRemoved(actor, effectData);
            if (comp != null)
                UnityEngine.Object.Destroy(comp);
        }
    }
    public static class AilmentsCore
    {
        public static GoopDefinition HemorragingBloodGoop;
        public static GoopDefinition CloneGoop;
        public static GoopDefinition CryotheumGoop;

        public static GameActorWarpEffect WarpEffect;
        public static GameActorHemorragingEffect HemorragingEffect;
        public static GameActorHemorragingEffect PoisBleedEffect;

        public static GameActorOnDeathEffect HellfireEffect;
        public static GameActorOnDeathEffect AcridRoundsEffect;

        public static GameObject hemoraggingOverhead;
        public static GameObject greenShitOverhead;
        public static void Init()
        {
            CloneGoop = ScriptableObject.CreateInstance<GoopDefinition>();
            CloneGoop.CanBeIgnited = true;
            CloneGoop.damagesPlayers = false;
            CloneGoop.damagesEnemies = false;
            CloneGoop.baseColor32 = Color.red;

            CryotheumGoop = ScriptableObject.CreateInstance<GoopDefinition>();
            CryotheumGoop.CanBeIgnited = false;
            //CryotheumGoop.eff

            WarpEffect = new GameActorWarpEffect
            {
                AppliesTint = true,
                duration = float.MaxValue,
                TintColor = new Color(0.77f, 0.21f, 0.48f, 1f),
                AffectsPlayers = false
            };

            List<string> bloodOverheadPaths = new List<string>();
            for (int i = 1; i <= 6; i++)
            {
                bloodOverheadPaths.Add($"{Module.ASSEMBLY_NAME}/Resources/Sprites/VFX/blooddripoverhead_00{i}.png");
            }

            HemorragingBloodGoop = EasyGoopDefinitions.GenerateBloodGoop(7, Color.red);
            hemoraggingOverhead = VFXAndAnimationShit.CreateOverheadVFX(bloodOverheadPaths, "HemorragingOverhead", 10);

            List<string> greenShitOverheadPaths = new List<string>();
            for (int i = 1; i <= 6; i++)
            {
                greenShitOverheadPaths.Add($"{Module.ASSEMBLY_NAME}/Resources/Sprites/VFX/greenblood_overhead_00{i}.png");
            }
            greenShitOverhead = VFXAndAnimationShit.CreateOverheadVFX(greenShitOverheadPaths, "HemorragingOverhead", 10);

            HemorragingEffect = new GameActorHemorragingEffect()
            {
                AffectsPlayers = false,
                duration = float.MaxValue,
                AffectsEnemies = true,
                gooper = HemorragingBloodGoop,
                effectIdentifier = "hemorraging",
                OverheadVFX = hemoraggingOverhead,
            };
            PoisBleedEffect = new GameActorHemorragingEffect()
            {
                AffectsPlayers = false,
                duration = float.MaxValue,
                AffectsEnemies = true,
                gooper = EasyGoopDefinitions.PoisonDef,
                DMGOnBleed = 0.1f,
                effectIdentifier = "poisonbleed",
                isGreenBlood = true,
                OverheadVFX = greenShitOverhead
            };

            HellfireEffect = new GameActorOnDeathEffect(typeof(OnDeathEffectModifierItem.HellfireExplodeOnDeath))
            {
                TintColor = Color.red,
                AppliesTint = true,
                duration = 6f,
                effectIdentifier = "explosiveHellfire",
                deathType = OnDeathBehavior.DeathType.PreDeath,

                PostApplyAction = onDeath =>
                {
                    OnDeathEffectModifierItem.HellfireExplodeOnDeath deathExplode = (OnDeathEffectModifierItem.HellfireExplodeOnDeath)onDeath;
                    deathExplode.explosionData = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
                }
            };
            AcridRoundsEffect = new GameActorOnDeathEffect(typeof(GoopDoer))
            {
                duration = 6f,
                effectIdentifier = "causticAcridity",

                PostApplyAction = goopDoer =>
                {
                    var goopdoer = goopDoer as GoopDoer;

                    goopdoer.goopDefinition = EasyGoopDefinitions.PoisonDef;
                    goopdoer.updateOnPreDeath = true;
                    goopdoer.defaultGoopRadius = 2f;
                    goopdoer.updateOnAnimFrames = false;
                    goopdoer.updateTiming = GoopDoer.UpdateTiming.TriggerOnly;
                }
            };
            /*
                                                                       * 

            CustomGoopEffectDoer gooper = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopClone).gameObject.AddComponent<CustomGoopEffectDoer>();
            gooper.IsCloner = true;*/
        }
    }
}
