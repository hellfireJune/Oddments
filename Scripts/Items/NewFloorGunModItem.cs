using Alexandria.Misc;
using JuneLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class NewFloorGunModItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(NewFloorGunModItem))
        {
            Name = "Shelleton Hand",
            //Quality = ItemQuality.D,
        };
        public static OddItemTemplate template2 = new OddItemTemplate(typeof(NewFloorGunModItem))
        {
            Name = "gene splicer",
            //Quality = ItemQuality.A,
            PostInitAction = item =>
            {
                NewFloorGunModItem nitem = (NewFloorGunModItem)item;
                nitem.ItemType = ItemTypeEnum.DOUBLE_HELIX;
                nitem.ModsToAdd = new List<StatModifier>() { new StatModifier() { amount = 0.83f, modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE, statToBoost = PlayerStats.StatType.Damage } };
            }
        };
        public static OddItemTemplate template3 = new OddItemTemplate(typeof(NewFloorGunModItem))
        {
            Name = "shelltans touch",
            //Quality = ItemQuality.B,
            PostInitAction = item =>
            {
                NewFloorGunModItem nitem = (NewFloorGunModItem)item;
                nitem.ItemType = ItemTypeEnum.SHELLTANS_TOUCH;
                nitem.ModsToAdd = new List<StatModifier>()
                {
                    new StatModifier() { amount = 0.2f, modifyType = StatModifier.ModifyMethod.ADDITIVE, statToBoost = PlayerStats.StatType.Damage},
                    new StatModifier() {amount = 2, modifyType = StatModifier.ModifyMethod.ADDITIVE, statToBoost = PlayerStats.StatType.Curse},
                };
            }
        };
        public static OddItemTemplate template4 = new OddItemTemplate(typeof(NewFloorGunModItem))
        {
            Name = "tapeflower",
            //Quality = ItemQuality.S,
            PostInitAction = item =>
            {
                NewFloorGunModItem nitem = (NewFloorGunModItem)item;
                nitem.ItemType = ItemTypeEnum.SUPER_TAPE_THING;
            }
        };

        public ItemTypeEnum ItemType = ItemTypeEnum.SHELLETON_HAND;

        public enum ItemTypeEnum
        {
            SHELLTANS_TOUCH,
            SHELLETON_HAND,
            DOUBLE_HELIX,
            SUPER_TAPE_THING,
        }

        public bool GetGunCondition(Gun g)
        {
            Debug.Log("gun condition");
            Debug.Log(g == null);
            if (g == null)
            {
                return false;
            }
            switch (ItemType)
            {
                default: return true;

                case ItemTypeEnum.SHELLETON_HAND:
                    return g.CanGainAmmo && g.CurrentAmmo < g.AdjustedMaxAmmo;
                case ItemTypeEnum.SUPER_TAPE_THING:
                    return !g.InfiniteAmmo && !g.CanBeDropped && (g.DuctTapeMergedGunIDs == null || g.DuctTapeMergedGunIDs.Count == 0);
            }
        }

        public static StatModifier[] AddStatModToArray(StatModifier[] mods, List<StatModifier> mod)
        {
            foreach (StatModifier modifier in mod)
            {
                Array.Resize(ref mods, mods.Length + 1);
                mods[mods.Length - 1] = modifier;
            }
            return mods;
        }
        private void Instance_OnNewLevelFullyLoaded()
        {
            if (Owner)
            {
                List<Gun> guns = Owner.inventory.AllGuns.Where(g => GetGunCondition(g)).ToList();
                if (guns.Count == 0) return;
                var gun = BraveUtility.RandomElement(guns);

                if (ItemType == ItemTypeEnum.SUPER_TAPE_THING)
                {
                    Gun newGun;
                    do
                    {
                        newGun = PickupObjectDatabase.GetRandomGun();
                    } while (!newGun || newGun.InfiniteAmmo || !newGun.CanBeDropped);
                    DuctTapeItem.DuctTapeGuns(gun, newGun);
                }
                if (gun.CanGainAmmo)
                {
                    gun.GainAmmo(gun.maxAmmo);
                }

                if (ModsToAdd != null)
                {
                    var statModifiers = gun.currentGunStatModifiers;
                    statModifiers = AddStatModToArray(statModifiers, ModsToAdd);
                    gun.currentGunStatModifiers = statModifiers;
                }

                if (ItemType == ItemTypeEnum.SHELLTANS_TOUCH)
                {
                    gun.gameObject.GetOrAddComponent<ShelltanComponent>();
                }

                if (ItemType == ItemTypeEnum.DOUBLE_HELIX)
                {
                    var comp = gun.gameObject.GetOrAddComponent<HelixComponent>();
                    comp.mult++;

                    if (!Owner.GetComponent<HelixedFlag>())
                    {
                        Owner.gameObject.AddComponent<HelixedFlag>();
                        Owner.GetComponent<JEventsComponent>().ConstantModifyGunVolley += GunModify;
                    }

                    gun.PostProcessProjectile += p =>
                    {
                        if (p is InstantDamageOneEnemyProjectile)
                        {
                            return;
                        }
                        if (p is InstantlyDamageAllProjectile)
                        {
                            return;
                        }
                        if (!(p is HelixProjectile))
                        {
                            if (p.OverrideMotionModule != null && p.OverrideMotionModule is OrbitProjectileMotionModule)
                            {
                                OrbitProjectileMotionModule orbitProjectileMotionModule = p.OverrideMotionModule as OrbitProjectileMotionModule;
                                orbitProjectileMotionModule.StackHelix = true;
                                orbitProjectileMotionModule.ForceInvert = !UpOrDown;
                            }
                            p.OverrideMotionModule = new HelixProjectileMotionModule
                            {
                                ForceInvert = !UpOrDown
                            };
                            UpOrDown = !UpOrDown;
                        }
                    };
                }
            }
        }

        public static bool UpOrDown = false;
        public List<StatModifier> ModsToAdd = null;

        public override void Pickup(PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded += Instance_OnNewLevelFullyLoaded;
            base.Pickup(player);
        }

        private void GunModify(PlayerController arg1, Gun gun, ProjectileVolleyData data, RegeneratingVolleyModifiers.ModifyProjArgs arg4)
        {
            Debug.Log("volley shitter");
            if (gun)
            {
                Debug.Log("helixable");
                var comp = gun.GetComponent<HelixComponent>();
                if (!comp) { return; }
                Debug.Log("should helix");

                List<ProjectileModule> modules = new List<ProjectileModule>();
                for (int i = 0; i < comp.mult; i++)
                {
                    float num = data.projectiles.Count;
                    for (int j = 0; j < num; j++)
                    {
                        ProjectileModule projMod = data.projectiles[j];
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
                }
                arg4.projs.Add("odmnts_doubledHelix", modules);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= Instance_OnNewLevelFullyLoaded;
            player.GetComponent<JEventsComponent>().ConstantModifyGunVolley -= GunModify;
            base.DisableEffect(player);
        }

        public int timesToDo = 1;

        public class HelixComponent : BraveBehaviour
        {
            public int mult = 0;
        }
        public class ShelltanComponent : BraveBehaviour
        {

        }
        internal class HelixedFlag : BraveBehaviour
        {
        }
    }

}
