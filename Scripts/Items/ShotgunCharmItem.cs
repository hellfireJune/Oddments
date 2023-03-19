using JuneLib;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class ShotgunCharmItem : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ShotgunCharmItem))
        {
            Name = "Shotgun Charm",
            Description = "More Shot for your Gun",
            LongDescription = "Shotguns, or any guns that shoot with a large enough volley, shoot 33% more bullets. Shotguns are more common.",
            SpriteResource = $"{Module.SPRITE_PATH}/shotguncharm.png",
            Quality = ItemQuality.A,
        };

        public override void Pickup(PlayerController player)
        {
            player.GetComponent<JEventsComponent>().ConstantModifyGunVolley += Stats_AdditionalVolleyModifiers;
            ItemsCore.GetItemChanceMult += ChangeWeight;
            base.Pickup(player);
        }


        public override void DisableEffect (PlayerController player)
        {
            player.GetComponent<JEventsComponent>().ConstantModifyGunVolley -= Stats_AdditionalVolleyModifiers;
            ItemsCore.GetItemChanceMult -= ChangeWeight;
            base.DisableEffect(player);
        }

        float weightMult = 5f;
        private void ChangeWeight(PickupObject arg1, PlayerController arg2, bool arg3, JuneLibLootEngineModificationAPI.ModifyLuckArgs arg4)
        {
            if (arg1 is Gun gun
                && gun.gunClass == GunClass.SHOTGUN)
            {
                arg4.WeightMult *= weightMult;
            }
        }

        private void Stats_AdditionalVolleyModifiers(PlayerController player, Gun gun, ProjectileVolleyData data, RegeneratingVolleyModifiers.ModifyProjArgs args)
        {
            //ETGModConsole.Log(gun == null ? "guns null" : gun.PickupObjectId.ToString());
            if (!gun /*|| gun.gunClass != GunClass.SHOTGUN*/)
            {
                return;
            }
            if (gun.rawVolley != null && (gun.rawVolley.projectiles.Count >= 3 || gun.gunClass == GunClass.SHOTGUN))
            {
                float num = gun.rawVolley.projectiles.Count / 3;
                num = (int)(UnityEngine.Random.value < num % 1 ? Math.Ceiling(num) : Math.Floor(num));
                List<ProjectileModule> modules = new List<ProjectileModule>();
                for (int i = 0; i < num; i++)
                {
                    ProjectileModule projMod = BraveUtility.RandomElement(data.projectiles);
                    int sourceIndex = 1;
                    if (projMod.CloneSourceIndex >= 0)
                    {
                        sourceIndex = projMod.CloneSourceIndex;
                    }

                    ProjectileModule projectileModule2 = ProjectileModule.CreateClone(projMod, false, sourceIndex);
                    projectileModule2.ignoredForReloadPurposes = true;
                    projectileModule2.ammoCost = 0;

                    modules.Add(projectileModule2);
                    //args.projs.projectiles.Add(projectileModule2);
                }
                args.projs.Add("odmnts_shotgunCharm", modules);
            }
        }
    }
}
