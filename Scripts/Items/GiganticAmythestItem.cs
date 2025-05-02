using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;

namespace Oddments
{
    internal class GiganticAmythestItem : PlayerItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(GiganticAmythestItem))
        {
            Name = "Gigantic Amythest",
            AltTitle = "Supermassive Amythest",
            Quality = ItemQuality.C,
            SpriteResource = $"{Module.SPRITE_PATH}/giganticamythest.png",
            Description = "Full focus",
            AltDesc = "Fully charged",
            LongDescription = "Instantly reloads your current gun on use",
            PostInitAction = item =>
            {
                var pitem = (GiganticAmythestItem)item;
                pitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 7f);
            }
        };

        public override bool CanBeUsed(PlayerController user)
        {
            var gun = user.CurrentGun;
            if (gun == null)
            {
                return false;
            }
            if (gun.UsesRechargeLikeActiveItem)
            {
                return false;
            }

            if (gun.ClipShotsRemaining == gun.ClipCapacity)
            {
                return false;
            }

            return base.CanBeUsed(user);
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);

            var gun = user.CurrentGun;
            if (gun)
            {
                gun.ForceImmediateReload();
                gun.ClearCooldowns();
            }
        }
    }
}
