using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using UnityEngine;

namespace Oddments
{
    internal class BuffOnReloadItem : PassiveItem
    {

        public static OddItemTemplate template2 = new OddItemTemplate(typeof(BuffOnReloadItem))
        {
            Name = "Double Chamber",
            Description = "Really Wanna Know DOUBLE",
            LongDescription = "Reloading a gun briefly doubles fire rate, and allows you to fire automatically",
            DontAutoTitleize=true,
            Quality = ItemQuality.B,
            SpriteResource = $"{Module.SPRITE_PATH}/doublechamber.png",

            PostInitAction = item =>
            {
                item.AddToSubShop(ItemBuilder.ShopType.Cursula);
            }
        };

        public static OddItemTemplate template3 = new OddItemTemplate(typeof(BuffOnReloadItem))
        {
            Name = "hunt'ers harpoon",

            PostInitAction = item =>
            {
                var harpoon = item as BuffOnReloadItem;
                harpoon.ReloadDuration = 0.8f;
                harpoon.isDouble = false;
            },
            //Quality = ItemQuality.D,
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReloadedGun += ReloadedGun;
            player.GunChanged += Player_GunChanged;
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player)
            {
                player.OnReloadedGun -= ReloadedGun;
                player.GunChanged -= Player_GunChanged;
            }

            base.DisableEffect(player);
        }

        private void Player_GunChanged(Gun arg1, Gun arg2, bool arg3)
        {
            isActive = false;
        }

        private void ReloadedGun(PlayerController arg1, Gun arg2)
        {
            if (arg1 && arg2)
            {
                if (!isActive)
                {
                    arg1.StartCoroutine(OnReloadBuff(arg1));
                } else
                {
                    elapsed = 0;
                }
            }
        }

        bool isDouble = true;

        public IEnumerator OnReloadBuff(PlayerController player)
        {
            this.RemoveStat(PlayerStats.StatType.RateOfFire);

            if (isDouble)
            {
                player.stats.AdditionalVolleyModifiers += Stats_AdditionalVolleyModifiers;
                this.AddStat(PlayerStats.StatType.RateOfFire, 2f);
            } else
            {
                this.AddStat(PlayerStats.StatType.MovementSpeed, 2);
            }
            player.stats.RecalculateStats(player);
            elapsed = 0;
            isActive = true;

            while (elapsed < ReloadDuration && isActive && this)
            {
                yield return null;
                if (!player.CurrentGun || !player.CurrentGun.IsReloading)
                {
                    elapsed += BraveTime.DeltaTime;
                }
            }

            if (isDouble)
            {
                player.stats.AdditionalVolleyModifiers -= Stats_AdditionalVolleyModifiers;
                this.RemoveStat(PlayerStats.StatType.RateOfFire);
            }
            else
            {
                this.RemoveStat(PlayerStats.StatType.MovementSpeed);
            }
            player.stats.RecalculateStats(player);
            if (this)
            {
                elapsed = ReloadDuration;
                isActive = false;
            }
            yield break;

        }

        private void Stats_AdditionalVolleyModifiers(ProjectileVolleyData obj)
        {
            foreach (ProjectileModule mod in obj.projectiles)
            {
                if (mod.shootStyle == ProjectileModule.ShootStyle.SemiAutomatic)
                {
                    mod.shootStyle = ProjectileModule.ShootStyle.Automatic;
                }
            }
        }

        protected float elapsed = 1f;
        protected bool isActive = false;

        public float ReloadDuration = 0.4f;
    }
}
