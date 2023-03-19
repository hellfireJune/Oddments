using JuneLib.Items;
using System.Collections.Generic;
using JuneLib;
using SaveAPI;

namespace Oddments
{
    public class CardinalBulletsItem : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CardinalBulletsItem))
        {
            Name = "Cardinallets",
            Description = "Sign of the Cross",
            LongDescription = "Firing your guns has a chance to shoot 3 additional bullets, to form a cross from where you fired",
            Quality = ItemQuality.B,
            SpriteResource = $"{Module.SPRITE_PATH}/cardinallets.png",
            PostInitAction = item =>
            {
                item.SetupUnlockOnFlag(GungeonFlags.BOSSKILLED_OLDKING, true);
                item.AddUnlockText("Kill the Old King");
            }
        };
        public override void Pickup(PlayerController player)
        {
            player.GetComponent<JEventsComponent>().ConstantModifyGunVolley += Stats_AdditionalVolleyModifiers;
            base.Pickup(player);
        }
        public override void DisableEffect(PlayerController player)
        {
            player.GetComponent<JEventsComponent>().ConstantModifyGunVolley -= Stats_AdditionalVolleyModifiers;
            base.DisableEffect(player);
        }
        private float m_ProcChance = 0.25f;
        private void Stats_AdditionalVolleyModifiers(PlayerController player, Gun gun, ProjectileVolleyData data, RegeneratingVolleyModifiers.ModifyProjArgs args)
        {
            List<ProjectileModule> modules = RegeneratingVolleyModifiers.AllAvailableModules(data, gun);
            if (modules.Count <= 0) { return; }
            if (UnityEngine.Random.value < m_ProcChance)
            {
                List<ProjectileModule> projMods = new List<ProjectileModule>();
                for (int i = 90; i < 360; i += 90)
                {
                    int j = UnityEngine.Random.Range(0, modules.Count - 1);
                    ProjectileModule projectileModule = modules[j];
                    //TGModConsole.Log(j);
                    int sourceIndex = j;
                    if (projectileModule.CloneSourceIndex >= 0)
                    {
                        sourceIndex = projectileModule.CloneSourceIndex;
                    }
                    ProjectileModule projectileModule2 = ProjectileModule.CreateClone(projectileModule, false, sourceIndex);
                    projectileModule2.ignoredForReloadPurposes = true;
                    projectileModule2.ammoCost = 0;

                    projectileModule2.angleFromAim = projectileModule.angleFromAim + i;
                    projMods.Add(projectileModule2);
                }

                args.projs.Add("odmnts_cardinalBullets", projMods);
            }
        }
    }
}
