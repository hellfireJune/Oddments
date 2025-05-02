using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class OddStatusEffectModifierItem : OddProjectileModifierItem
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

        public override bool ApplyBulletEffect(Projectile arg1)
        {
            if (!string.IsNullOrEmpty(SynergyToCheck) && Owner && Owner.PlayerHasActiveSynergy(SynergyToCheck))
            {
                arg1.statusEffectsToApply.Add(SynergyAffect);
            }
            else
            {
                arg1.statusEffectsToApply.Add(EffectToApply);
            }
            return true;
        }

        public string SynergyToCheck;
        public GameActorEffect EffectToApply;
        public GameActorEffect SynergyAffect;
    }
}
