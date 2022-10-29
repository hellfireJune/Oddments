using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public static class AilmentsCore
    {
        public static GoopDefinition GoopClone;
        public static void Init()
        {
            GoopClone = new GoopDefinition();
            GoopClone.CanBeIgnited = true;
            GoopClone.damagesPlayers = false;
            GoopClone.damagesEnemies = false;
            GoopClone.baseColor32 = Color.red;/*

            CustomGoopEffectDoer gooper = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopClone).gameObject.AddComponent<CustomGoopEffectDoer>();
            gooper.IsCloner = true;*/
        }
    }
}
