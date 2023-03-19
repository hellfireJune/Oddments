using Alexandria.ItemAPI;
using JuneLib.Chests;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class SacredOrb : ChestModifyItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(SacredOrb))
        {
            Name = "Crown of Quality",
            Description = "Guarantee",
            LongDescription = "D-Tier chests can no longer spawn, and will be replaced by higher tier chests",
            SpriteResource = $"{Module.SPRITE_PATH}/crownofquality.png",
            Quality = ItemQuality.C,
        };

        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            base.DisableEffect(player);
        }
    }
    public class CrownOfGuns : ChestModifyItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CrownOfGuns))
        {
            Name = "Crown of War",
            Quality = ItemQuality.D,
            Description = "All is fair",
            LongDescription = "All chests will drop guns instead of items",
            SpriteResource = $"{Module.SPRITE_PATH}/crownofwar.png",
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1);
            }
        };

        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            base.DisableEffect(player);
        }
    }
    public class CrownOfLove : ChestModifyItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CrownOfLove))
        {
            Name = "Crown of Love",
            Quality = ItemQuality.C,
            SpriteResource = $"{Module.SPRITE_PATH}/crownoflove.png",
            Description = "All is fair",
            LongDescription = "All chests will drop items instead of guns",
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2);
            }
        };

        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            base.DisableEffect(player);
        }
    }
}
