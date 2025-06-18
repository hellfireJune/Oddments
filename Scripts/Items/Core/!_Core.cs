using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using UnityEngine;

namespace Oddments
{
    public static class Core
    {
        public static bool DontDropMore = false;

        public static AIActor GetNearestEnemyWithIgnore(RoomHandler room, Vector2 position, out float nearestDistance, AIActor ignored, bool includeBosses = true, bool excludeDying = false)
        {
            AIActor result = null;
            nearestDistance = float.MaxValue;

            var activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

            if (activeEnemies == null)
            {
                return null;
            }
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                if (ignored == activeEnemies[i]) { continue; }

                if (includeBosses || !activeEnemies[i].healthHaver.IsBoss)
                {
                    if (!excludeDying || !activeEnemies[i].healthHaver.IsDead)
                    {
                        float num = Vector2.Distance(position, activeEnemies[i].CenterPosition);
                        if (num < nearestDistance)
                        {
                            nearestDistance = num;
                            result = activeEnemies[i];
                        }
                    }
                }
            }
            return result;
        }
    }

    public class OddItemTemplate : ItemTemplate
    {
        public OddItemTemplate(Type type) : base(type)
        {
        }

        public override void SpecialClassBasedThing(PickupObject pickup)
        {
            base.SpecialClassBasedThing(pickup);
            if (AllowForMetaPick && pickup is PassiveItem passive)
            {
                metaItems.Add(passive);
            }

            if (!DontAutoTitleize && pickup.encounterTrackable)
            {
                pickup.SetShortDescription(Description.ToTitleCaseInvariant());
            }

            if (JuneSaveManagerCore.AltTextC && !string.IsNullOrEmpty(AltTitle))
            {
                pickup.SetName(AltTitle);
            }
            if (JuneSaveManagerCore.AltTextB && !string.IsNullOrEmpty(AltDesc))
            {
                pickup.SetShortDescription(AltDesc);
            }
            if (JuneSaveManagerCore.AltTextA && !string.IsNullOrEmpty(AltLongDesc))
            {
                pickup.SetLongDescription(AltLongDesc);
            }
        }

        public bool AllowForMetaPick = true;
        public bool DontAutoTitleize = false;
        public string AltTitle;
        public string AltDesc;
        public string AltLongDesc;

        public static List<PassiveItem> metaItems = new List<PassiveItem>();
    }

    public class PickupTemplate : OddItemTemplate
    {
        public PickupTemplate(Type type) : base(type)
        {
        }

        public override void SpecialClassBasedThing(PickupObject pickup)
        {
            base.SpecialClassBasedThing(pickup);

            GameObject gameObject = pickup.gameObject;
            SpeculativeRigidbody specRig = gameObject.AddComponent<SpeculativeRigidbody>();
            PixelCollider collide = new PixelCollider
            {
                IsTrigger = true,
                ManualWidth = ColliderWidth,
                ManualHeight = ColliderHeight,
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.PlayerBlocker,
                ManualOffsetX = 0,
                ManualOffsetY = 0
            };
            specRig.PixelColliders = new List<PixelCollider>
            {
                collide
            };
            pickup.UsesCustomCost = CustomCost >= 0;
            pickup.CustomCost = CustomCost;

            if (AutoAddToPools)
            {
                if (ShopPoolWeight > 0)
                {
                    WeightedGameObject weightedObject = new WeightedGameObject();
                    weightedObject.SetGameObject(gameObject);
                    weightedObject.weight = ShopPoolWeight;
                    weightedObject.rawGameObject = gameObject;
                    weightedObject.pickupId = pickup.PickupObjectId;
                    weightedObject.forceDuplicatesPossible = true;
                    weightedObject.additionalPrerequisites = new DungeonPrerequisite[0];

                    GenericLootTable shopPool = ItemBuilder.LoadShopTable("Shop_Gungeon_Cheap_Items_01");
                    shopPool.defaultItemDrops.elements.Add(weightedObject);
                }
                if (RewardPoolWeight > 0)
                {
                    WeightedGameObject weightedObject = new WeightedGameObject();
                    weightedObject.SetGameObject(gameObject);
                    weightedObject.weight = RewardPoolWeight;
                    weightedObject.rawGameObject = gameObject;
                    weightedObject.pickupId = pickup.PickupObjectId;
                    weightedObject.forceDuplicatesPossible = true;
                    weightedObject.additionalPrerequisites = new DungeonPrerequisite[0];
                    foreach (FloorRewardData rewardData in GameManager.Instance.RewardManager.FloorRewardData)
                    {
                        if (rewardData.SingleItemRewardTable.defaultItemDrops.elements != null && rewardData.SingleItemRewardTable.defaultItemDrops.elements.Count != 0 && !rewardData.SingleItemRewardTable.defaultItemDrops.elements.Contains(weightedObject))
                        {
                            rewardData.SingleItemRewardTable.defaultItemDrops.Add(weightedObject);
                        }
                    }
                }
            }
        }

        public bool AutoAddToPools = false;
        public float RewardPoolWeight = 0f;
        public float ShopPoolWeight = 0f;

        public int ColliderWidth = 1;
        public int ColliderHeight = 1;
        public int CustomCost = -1;
    }

    public class ItemTemplatePlusVolley : OddItemTemplate
    {
        public ItemTemplatePlusVolley(Type type) : base(type)
        {
            ProjectilesToFire = 1;
            IsRadial = false;
            AutoAddVolley = true;
        }

        public override void SpecialClassBasedThing(PickupObject pickup)
        {
            base.SpecialClassBasedThing(pickup);
            if (Projectile != null)
            {
                CachedVolleyData = ScriptableObject.CreateInstance<ProjectileVolleyData>();
                CachedVolleyData.projectiles = new List<ProjectileModule>();
                for (int i = 0; i < ProjectilesToFire; i++)
                {
                    var module = new ProjectileModule();
                    module.shootStyle = ProjectileModule.ShootStyle.Automatic;
                    module.cooldownTime = -1f;
                    module.numberOfShotsInClip = -1;
                    if (IsRadial)
                    {
                        module.angleFromAim = (i / ProjectilesToFire) * 360;
                    }
                    module.ammoCost = 0;
                    module.projectiles = new List<Projectile> { Projectile };
                    PreAddModuleAction?.Invoke(module);

                    CachedVolleyData.projectiles.Add(module);
                }

                if (AutoAddVolley)
                {
                    if (pickup is VolleyOnBlankItem blankItem)
                    {
                        blankItem.ThingToFire = CachedVolleyData;
                    }
                    if (pickup is VolleySpamPlayerItem spamPlayerItem)
                    {
                        spamPlayerItem.BaseVolley = CachedVolleyData;
                    }
                }
            }
        }

        public Projectile Projectile;
        public Action<ProjectileModule> PreAddModuleAction;
        public int ProjectilesToFire;
        public bool IsRadial;
        public bool AutoAddVolley;

        public ProjectileVolleyData CachedVolleyData;
    }

    /*
    public class HemorragingStatusEffectItemTemplate : BulletModifierItemTemplate
    {
        public HemorragingStatusEffectItemTemplate(Type type) : base(type)
        {
        }

        public override void SpecialClassBasedThing(PickupObject pickup)
        {
            base.SpecialClassBasedThing(pickup);
            if (pickup is OddStatusEffectModifierItem)
            {
                var oddMod = (OddStatusEffectModifierItem)pickup;
                oddMod.EffectToApply = EffectToApply;
                oddMod.SynergyToCheck = SynergyToCheck;
                oddMod.SynergyAffect = SynergyEffect;
            }
        }
        public GameActorHemorragingEffect EffectToApply;
        public GameActorHemorragingEffect SynergyEffect;
        public string SynergyToCheck;
        public Color SynergyTint;
    }
    */




    public class BulletModifierItemTemplate : OddItemTemplate
    {
        public BulletModifierItemTemplate(Type type) : base(type)
        {
        }
        public override void SpecialClassBasedThing(PickupObject pickup)
        {
            if (pickup is OddProjectileModifierItem oddMod)
            {
                oddMod.ActivationChance = ProcChance;
                oddMod.BulletTint = ProjectileTint;
                oddMod.ChanceScalesWithDamageFired = ChanceScalesWithDamageFired;
                oddMod.TintPriority = TintPriority;
            }

            if (IsBulletMod)
            {
                pickup.MakeBulletMod();
            }
            base.SpecialClassBasedThing(pickup);
        }

        public Color ProjectileTint;
        public float ProcChance;
        public bool ChanceScalesWithDamageFired;
        public int TintPriority;

        public bool IsBulletMod = true;
    }

    public static class OddItemIDs
    {
        public static int BlendedHeart;

        public static int StackedArmor;

        public static int StackedBlanks;

        public static int KeyRingPickup;

        public static int BigAmmoCratePickup;

        public static int InfAmmoPickup;
    }
}
