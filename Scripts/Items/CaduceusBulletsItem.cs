using HarmonyLib;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    [HarmonyPatch]
    public class CaduceusBulletsItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(CaduceusBulletsItem))
        {
            Name = "Caduceus Rounds",
            Description = "Do No Harm",
            LongDescription = "Poison, fire and electricity immunity. Any status effects applied to enemies will instead be converted into raw damage",
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

        public static float DMG = 12f;

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
                __instance.healthHaver.ApplyDamage(DMG, Vector2.zero, "Caduceus");
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


        public static readonly List<string> banlist = new List<string>()
        {
            "jamBuff",
            "Infection",
            "InfectionBoss",
            "broken Armor"

            //Brain host debuff intentionally left unincluded. Caduceus can have a little cheese.
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
