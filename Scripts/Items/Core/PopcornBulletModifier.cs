using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    internal class PopcornBulletModifier : BraveBehaviour
    {
        public static void Init()
        {
            GameObject popcornPrefab = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("Popped Corn VFX");
        }

        public static GameObject PopcornPrefab;

        void Start()
        {
            if (projectile)
            {
                projectile.OnHitEnemy += OnHitEnemy;
            }
        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg1 && arg1.sprite)
            {
                var pos = arg1.sprite.WorldCenter;
            }
        }

        float Chance = 1f;
    }

    public class PopcornVFXBehaviour : BraveBehaviour
    {
        public static void Init()
        {
            GameObject vfxPrefab = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("Popcorn fire burst vfx");
        }

        public static GameObject FireBurstVFX;

        public GameObject CurrentBurst = FireBurstVFX;
    }
}
