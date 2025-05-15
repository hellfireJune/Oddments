using Alexandria.ItemAPI;
using Alexandria.Misc;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class FuckyBarrelItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(FuckyBarrelItem))
        {

            PostInitAction = item =>
            {
                item.RemovePickupFromLootTables();
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.stats.AdditionalVolleyModifiers += Stats_AdditionalVolleyModifiers;
        }

        private void Stats_AdditionalVolleyModifiers(ProjectileVolleyData volleyToModify)
        {
            int count = volleyToModify.projectiles.Count;
            for (int i = 0; i < count; i++)
            {
                ProjectileModule projectileModule = volleyToModify.projectiles[i];
                int sourceIndex = i;
                if (projectileModule.CloneSourceIndex >= 0)
                {
                    sourceIndex = projectileModule.CloneSourceIndex;
                }
                ProjectileModule projectileModule2 = ProjectileModule.CreateClone(projectileModule, false, sourceIndex);
                projectileModule2.ignoredForReloadPurposes = true;
                projectileModule2.ammoCost = 0;

                projectileModule2.angleFromAim = projectileModule.angleFromAim + 45;
                projectileModule.angleFromAim -= 45;
                volleyToModify.projectiles.Add(projectileModule2);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if(player)
                player.stats.AdditionalVolleyModifiers += Stats_AdditionalVolleyModifiers;

            base.DisableEffect(player);
        }
    }
}
