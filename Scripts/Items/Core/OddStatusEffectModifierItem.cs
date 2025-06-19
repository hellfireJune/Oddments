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


        public static BulletModifierItemTemplate toothy_template = new BulletModifierItemTemplate(typeof(OddStatusEffectModifierItem))
        {
            Name = "Toothy Bullets",
            SpriteResource = $"{Module.SPRITE_PATH}/toothlets.png",
            Description = "Enter the Gumgeon",
            LongDescription = "Bullets have a chance to bleed enemies, causing them to leave damaging blood creep behind",
            Quality = ItemQuality.A,
            ProcChance = 0.15f,
            PostInitAction = item =>
            {
                var odd = (item as OddStatusEffectModifierItem);
                odd.SynergyToCheck = CavitySynergyName;
                odd.SynergyEffect = AilmentsCore.PoisBleedEffect;
                odd.EffectToApply = AilmentsCore.HemorragingEffect;
            },
            ProjectileTint = new Color(1, 1f, 0.75f),
            
        };

        public override bool ApplyBulletEffect(Projectile arg1)
        {
            if (!string.IsNullOrEmpty(SynergyToCheck) && Owner && Owner.PlayerHasActiveSynergy(SynergyToCheck))
            {
                arg1.statusEffectsToApply.Add(SynergyEffect);
            }
            else
            {
                arg1.statusEffectsToApply.Add(EffectToApply);
            }
            return true;
        }

        public string SynergyToCheck;
        public GameActorHemorragingEffect EffectToApply = AilmentsCore.HemorragingEffect; 
        public GameActorHemorragingEffect SynergyEffect = AilmentsCore.PoisBleedEffect;
    }
}
