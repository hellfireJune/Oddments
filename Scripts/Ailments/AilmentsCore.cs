using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using JuneLib.Status;
using JuneLib;

namespace Oddments
{
    public static class AilmentsCore
    {
        public static GoopDefinition HemorragingBloodGoop;
        public static GoopDefinition GoopClone;
        public static GameActorWarpEffect WarpEffect;
        public static GameActorHemorragingEffect HemorragingEffect;
        public static GameActorHemorragingEffect PoisBleedEffect;

        public static GameObject hemoraggingOverhead;
        public static GameObject greenShitOverhead;
        public static void Init()
        {
            GoopClone = ScriptableObject.CreateInstance<GoopDefinition>();
            GoopClone.CanBeIgnited = true;
            GoopClone.damagesPlayers = false;
            GoopClone.damagesEnemies = false;
            GoopClone.baseColor32 = Color.red;

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
            /*
                                                                       * 

            CustomGoopEffectDoer gooper = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopClone).gameObject.AddComponent<CustomGoopEffectDoer>();
            gooper.IsCloner = true;*/
        }
    }
}
