using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;
using JuneLib;
using JuneLib.Items;

namespace Oddments
{
    public class WhollyGhost : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(WhollyGhost))
        {
            Name = "The Wholly Ghost",
            Description = "Woo!",
            LongDescription = "Enemies have a chance to re-animate as hollowpoints. All hollowpoints pledge allegiance to you.\n\nscrunkly",
            SpriteResource = $"{Module.SPRITE_PATH}/thewhollyghost.png",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            {
                ChallengeManager manager = Instantiate(ChallengeHelper.ChallengeManagerPrefab.gameObject).GetComponent<ChallengeManager>();
                HauntedChallengeModifier challengeMod = (HauntedChallengeModifier)manager.PossibleChallenges.Find(c => c.challenge is HauntedChallengeModifier).challenge;
                
                Destroy(manager);
                haunt = challengeMod;
                haunt.Chance = 0.2f;
            }
        };

        public static HauntedChallengeModifier haunt;

        public override void Pickup(PlayerController player)
        {
            player.OnAnyEnemyReceivedDamage += haunt.OnEnemyDamaged;
            ETGMod.AIActor.OnPreStart += OnPreStart;
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController disablingPlayer)
        {
            disablingPlayer.OnAnyEnemyReceivedDamage -= haunt.OnEnemyDamaged;
            ETGMod.AIActor.OnPreStart -= OnPreStart;
            base.DisableEffect(disablingPlayer);
        }

        private void OnPreStart(AIActor obj)
        {
            if (obj.EnemyGuid == haunt.GhostGuid)
            {
                obj.AddPermanentCharm();
            }
        }
    }
}
