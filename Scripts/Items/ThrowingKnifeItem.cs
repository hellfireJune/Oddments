using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;

namespace Oddments
{
    public class ThrowingKnifeItem : PlayerItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(ThrowingKnifeItem))
        {
            Name = "Throwing Knife",
            AltTitle = "Throwing Knives",
            Quality = ItemQuality.C,
            Description = "Facestabber",
            SpriteResource = $"{Module.SPRITE_PATH}/facestabber.png",
            LongDescription = "Some tactical throwing knives. Causes enemies to bleed on strike",
            Cooldown = 250,
            CooldownType = ItemBuilder.CooldownType.Damage,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);
                ThrowingKnifeItem throwingKnifeItem = item as ThrowingKnifeItem;
                throwingKnifeItem.UsesNumberOfUsesBeforeCooldown = true;
                throwingKnifeItem.numberOfUses = 3;
                InitProjectile();
            }
        };

        public static Projectile knifeProjectile;

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            GameObject gameObject = SpawnManager.SpawnProjectile(knifeProjectile.gameObject, user.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, (user.CurrentGun == null) ? 0 : user.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            if (component)
            {
                component.Owner = user;
                component.Shooter = user.specRigidbody;
                user.DoPostProcessProjectile(component);
                component.statusEffectsToApply.Add(AilmentsCore.HemorragingEffect);

            }
        }

        public static void InitProjectile()
        {
            Projectile projectile = Instantiate(((Gun)PickupObjectDatabase.GetById(15)).DefaultModule.projectiles[0]);
            projectile.baseData.speed *= 2;
            projectile.baseData.damage = 10f;
            projectile.shouldRotate = true;

            PierceProjModifier pierce = projectile.gameObject.AddComponent<PierceProjModifier>();
            pierce.penetration = 1;
            pierce.penetratesBreakables = true;

            List<string> list = new List<string>();
            for (int i = 1; i <= 8; i++)
            {
                list.Add($"oddments_throwingknifeprojecctile_00{i}");
            }
            List<IntVector2> intVectors = new List<IntVector2>
            {
                new IntVector2(16, 6),
                new IntVector2(16, 6),
                new IntVector2(16, 6),
                new IntVector2(16, 6),
                new IntVector2(16, 6),
                new IntVector2(16, 6),
                new IntVector2(16, 6),
                new IntVector2(16, 6),
            };
            projectile.AddAnimationToProjectile(list, 12, intVectors, dontEvenKnowWhatThisIs, anchors, fixesScale, fixesScale, vector2dotZero, offsets, offsets, Intuitivecoding);

            projectile.gameObject.SetActive(false);
            DontDestroyOnLoad(projectile.gameObject);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            knifeProjectile = projectile;
        }
        static readonly List<Projectile> Intuitivecoding = new List<Projectile>
            {
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
            };

        static readonly List<IntVector2?> offsets = new List<IntVector2?>
            {
            null,
            null,null, null,
            null, null,
            null,
            null,
            }; //this is a negative skew i think

        static List<Vector3?> vector2dotZero = new List<Vector3?>
            {
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero
            };

        static List<bool> fixesScale = new List<bool>
            {
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
            };

        static List<bool> dontEvenKnowWhatThisIs = new List<bool>
            {
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            };
        static List<tk2dBaseSprite.Anchor> anchors = new List<tk2dBaseSprite.Anchor>
            {
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter,
                tk2dBaseSprite.Anchor.MiddleCenter
            };
        static List<bool> changesColliders = new List<bool>
            {
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
            };
    }
}
