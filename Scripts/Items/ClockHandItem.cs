using JuneLib;
using JuneLib.Items;
using System.Collections.Generic;

namespace Oddments
{
    public class ClockHandItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(ClockHandItem));

        public override void Pickup(PlayerController player)
        {
            player.GetComponent<JEventsComponent>().ConstantModifyGunVolley += ModifyVolleyConstant;
            base.Pickup(player);
        }

        public float MaxRotate;
        private void ModifyVolleyConstant(PlayerController player, Gun gun, ProjectileVolleyData data, RegeneratingVolleyModifiers.ModifyProjArgs args)
        {
            List<ProjectileModule> modules = RegeneratingVolleyModifiers.AllAvailableModules(data, gun);
            if (modules.Count <= 0) { return; }

            List<ProjectileModule> projMods = new List<ProjectileModule>();
            int count = modules.Count;
            for (int i = 0; i < count; i++)
            {
                ProjectileModule projectileModule = modules[i];
                //TGModConsole.Log(j);
                int sourceIndex = i;
                if (projectileModule.CloneSourceIndex >= 0)
                {
                    sourceIndex = projectileModule.CloneSourceIndex;
                }
                ProjectileModule projectileModule2 = ProjectileModule.CreateClone(projectileModule, false, sourceIndex);
                projectileModule2.ignoredForReloadPurposes = true;
                projectileModule2.ammoCost = 0;

                float rotate = (player.m_elapsedNonalertTime % MaxRotate) / MaxRotate;
                projectileModule2.angleFromAim = projectileModule.angleFromAim + (rotate*360);
                projMods.Add(projectileModule2);
            }

            args.projs.Add("odmnts_clockHand", projMods);
        }
    }
}
