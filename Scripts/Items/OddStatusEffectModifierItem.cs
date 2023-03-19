using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class OddStatusEffectModifierItem : PassiveItem
    {
        public const string CavitySynergyName = "Gum disease";
        public static readonly List<string> CavityIDs = new List<string>()
        {
            $"{Module.PREFIX}:toothy_bullets",
            "bug_boots",
        };

        public static StatusEffectItemTemplate toothy_template = new StatusEffectItemTemplate(typeof(OddStatusEffectModifierItem))
        {
            Name = "Toothy Bullets",
            SpriteResource = $"{Module.SPRITE_PATH}/toothlets.png",
            Description = "Enter the Gumgeon",
            LongDescription = "Bullets have a chance to bleed enemies, causing them to leave damaging blood creep behind",
            Quality = ItemQuality.A,

            ProcChance = 0.15f,
            SynergyToCheck = CavitySynergyName,
            SynergyEffect = AilmentsCore.PoisBleedEffect,
            EffectToApply = AilmentsCore.HemorragingEffect,
            ProjectileTint = new Color(1, 1f, 0.75f)
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += Player_PostProcessProjectile;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            player.PostProcessProjectile -= Player_PostProcessProjectile;
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            if (UnityEngine.Random.value < ProcChance)
            {
                if (!string.IsNullOrEmpty(SynergyToCheck) && Owner && Owner.PlayerHasActiveSynergy(SynergyToCheck))
                {
                    arg1.statusEffectsToApply.Add(SynergyAffect);
                }
                else
                {
                    arg1.statusEffectsToApply.Add(EffectToApply);
                }
                arg1.AdjustPlayerProjectileTint(TintColor, 3);
            }
        }

        public float ProcChance = 0;
        public string SynergyToCheck;

        public GameActorEffect EffectToApply;
        public GameActorEffect SynergyAffect;
        public Color TintColor;
    }
}
