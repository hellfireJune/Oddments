using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class ToothyBullets : PassiveItem
    {
        public const string CavitySynergyName = "Gum disease";
        public static readonly List<string> CavityIDs = new List<string>()
        {
            $"{Module.PREFIX}:toothy_bullets",
            "bug_boots",
        };

        public static ItemTemplate template = new ItemTemplate(typeof(ToothyBullets))
        {
            Name = "Toothy Bullets",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/toothlets.png",
            Description = "Enter the Gumgeon",
            LongDescription = "Bullets have a chance to bleed enemies, causing them to leave damaging blood creep behind",
            Quality = ItemQuality.A,
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
            if (UnityEngine.Random.value < 0.15f)
            {
                if (Owner && Owner.PlayerHasActiveSynergy(CavitySynergyName))
                {
                    arg1.statusEffectsToApply.Add(AilmentsCore.PoisBleedEffect);
                } else
                {
                    arg1.statusEffectsToApply.Add(AilmentsCore.HemorragingEffect);
                }
                arg1.AdjustPlayerProjectileTint(new UnityEngine.Color(1, 1f, 0.75f), 3);
            }
        }
    }
}
