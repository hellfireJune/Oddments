using HarmonyLib;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using JuneLib;
using Alexandria.ItemAPI;

namespace Oddments
{
    [HarmonyPatch]
    public class CaduceusBulletsItem : PassiveItem
    {
        public static GameObject HealDamageVFX;

        public static OddItemTemplate template = new OddItemTemplate(typeof(CaduceusBulletsItem))
        {
            Name = "Hippocratic Rounds",
            Description = "Do No Harm",
            LongDescription = "Fire rate up. Poison, fire and electricity immunity. Any status effects applied to enemies will instead be converted into raw damage",
            Quality = ItemQuality.A,

            PostInitAction = item =>
            {
                List<string> vfxanims = new List<string>();
                for (int i = 1; i <= 4; i++)
                {
                    vfxanims.Add($"{Module.ASSEMBLY_NAME}/Resources/Sprites/VFX/caduceusonhitflash_00{i}.png");
                }
                GameObject vfx = VFXAndAnimationShit.CreateOverheadVFX(vfxanims, "Caduceus Flash", 10);
                vfx.AddComponent<CaduceusFloater>();
                HealDamageVFX = vfx;

                item.PlaceItemInAmmonomiconAfterItemById(524);
                item.AddPassiveStatModifier(PlayerStats.StatType.RateOfFire, 0.15f);
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.AddFlagsToPlayer(GetType());
            foreach (var modifier in mods) { player.healthHaver.damageTypeModifiers.Add(modifier); }
        }
        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            player.RemoveFlagsFromPlayer(GetType());
            foreach (var modifier in mods) { player.healthHaver.damageTypeModifiers.Remove(modifier); }
        }

        public static float DMG = 6f;

        [HarmonyPatch(typeof(GameActor), nameof(GameActor.ApplyEffect))]
        [HarmonyPrefix]
        public static bool PreventAffectApply(GameActor __instance, GameActorEffect effect)
        {
            if (!IsFlagSetAtAll(typeof(CaduceusBulletsItem))
                || __instance is PlayerController)
            {
                return true;
            }
            if (effect is AIActorBuffEffect
                || banlist.Contains(effect.effectIdentifier))
            {
                return true;
            }

            CaduceusHelper caddy = __instance.gameObject.GetOrAddComponent<CaduceusHelper>();
            if (caddy != null && caddy.ShouldDoBonusEffect(effect))
            {
                __instance.healthHaver.ApplyDamage(DMG * Mathf.Min(8f, effect.duration), Vector2.zero, "Caduceus");
                Instantiate(HealDamageVFX, __instance.transform.position, Quaternion.identity);
            }

            return false;
        }

        public class CaduceusHelper : BraveBehaviour
        {
            public static readonly float Cooldown = 2.5f;
            private float m_elapsed = 0f;
            private Dictionary<string, float> dict = new Dictionary<string, float>();
            private void Update()
            {
                m_elapsed += BraveTime.DeltaTime;
            }

            public bool ShouldDoBonusEffect(GameActorEffect effect)
            {
                string id = effect.effectIdentifier;
                if (!dict.ContainsKey(id))
                {
                    dict.Add(id, -999);
                }

                if (dict[id] - m_elapsed < Cooldown)
                {
                    dict[id] = m_elapsed;
                    return true;
                }
                return false;
            }
        }

        public class CaduceusFloater : BraveBehaviour
        {
            void Update()
            {
                float elapsed = m_elapsed + BraveTime.DeltaTime;

                if (elapsed < m_duration)
                {
                    Destroy(gameObject);
                    return;
                }
                Vector3 uppies = GetVector(elapsed) - GetVector(m_elapsed);
                transform.localPosition += uppies;

                if (elapsed >= m_postBlinkDuration)
                {
                    float dur = elapsed - m_postBlinkDuration;
                    dur %= m_blinkspeed * 2;
                    bool rendered = dur>m_blinkspeed;

                    if (rendered != m_isRenderedRn)
                    {
                        m_isRenderedRn = rendered;
                        renderer.enabled = rendered;
                    }
                }

                m_elapsed = elapsed;
            }

            Vector3 GetVector(float elapsed)
            {
                float y = 1 - 1 / (1 + elapsed*2);
                return maxHeight * y;
            }
            Vector3 maxHeight = new Vector3(0, 40, 0);

            bool m_isRenderedRn = false;
            float m_elapsed = 0f;
            float m_duration = 2f;
            float m_postBlinkDuration = 1f;
            float m_blinkspeed = 0.1f;
        }


        public static readonly List<string> banlist = new List<string>()
        {
            "jamBuff",
            "Infection",
            "InfectionBoss",
            "broken Armor"

            //Brain host debuff intentionally left unincluded. Caduceus can have a little cheese.
            // i did take these from bunny btw yoink :-)
        };
        List<DamageTypeModifier> mods = new List<DamageTypeModifier>()
        {
            new DamageTypeModifier()
            {
                damageMultiplier = 0,
                damageType = CoreDamageTypes.Fire
            },
            new DamageTypeModifier()
            {
                damageMultiplier = 0,
                damageType = CoreDamageTypes.Poison
            },
            new DamageTypeModifier()
            {
                damageMultiplier = 0,
                damageType = CoreDamageTypes.Electric
            },
        };
    }
}
