using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    internal class ReloadEffectOnDamageItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(ReloadEffectOnDamageItem))
        {
            Name = "Reloadium",
            Description = "Reload Effects On Damage",
            LongDescription = "Taking damage will activate any on-reload effects that you have. Also increases reload speed.\n\nAn extremely reactive piece of metal. Applying enough force to it will reload the chunk of metal itself.",
            SpriteResource = $"{Module.SPRITE_PATH}/reloadium.png",
            Quality = ItemQuality.D,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.ReloadSpeed, -0.2f);
            }
        };
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += Player_OnReceivedDamage;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            if(player)
                player.OnReceivedDamage -= Player_OnReceivedDamage;
        }

        private void Player_OnReceivedDamage(PlayerController obj)
        {
            Gun gun = obj.CurrentGun;
            if (!gun) { return; }

            Action<PlayerController, Gun> action = obj.OnReloadedGun;
            action?.Invoke(obj, gun);
        }
    }
}
