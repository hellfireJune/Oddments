using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;

namespace Oddments
{
    public class WhollyGhost : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(WhollyGhost))
        {
            Name = "The Wholly Ghost",
            Description = "Woo!",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            {
                ChallengeManager manager = Instantiate(ChallengeHelper.ChallengeManagerPrefab).GetComponent<ChallengeManager>();
                HauntedChallengeModifier challengeMod = (HauntedChallengeModifier)manager.PossibleChallenges.Where(c => c.challenge.GetType() == typeof(HauntedChallengeModifier)).ToList()[0].challenge;
                
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
