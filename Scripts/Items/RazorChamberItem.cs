using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuneLib;
using UnityEngine;
using Alexandria.ItemAPI;

namespace Oddments
{
    public class RazorChamberItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(RazorChamberItem))
        {
            Name = "Razor Chamber",
            Description = "Sharp Reload",
            LongDescription = "Reloading an empty gun will damage nearby enemies. Sharp to the touch!",
            Quality = ItemQuality.C,
            SpriteResource = $"{Module.SPRITE_PATH}/razorchamber.png",
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReloadedGun += ReloadGun;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if(player)
                player.OnReloadedGun -= ReloadGun;
        }

        public float baseReloadDamage = 15f;
        public float distance = 8f;
        private void ReloadGun(PlayerController arg1, Gun arg2)
        {
            if (arg2 == null || arg2.ClipShotsRemaining > 0 ) { return; }

            float dmg = arg2.reloadTime;
            ProjectileModule mod = arg2.DefaultModule;
            if (mod != null)
            {
                dmg += (mod.cooldownTime / 10) * (mod.numberOfShotsInClip-1);
            }
            dmg *= baseReloadDamage;

            RoomHandler room = arg1.CurrentRoom;
            if (room != null)
            {
                room.DoToNearbyEnemiesBetter(arg2.barrelOffset.position, distance, (enemy, distance) =>
                {
                    if (enemy && enemy.healthHaver)
                    {
                        enemy.healthHaver.ApplyDamage(dmg, Vector2.zero, "Razor Chamber");
                    } 
                });
            }
        }
    }
}
