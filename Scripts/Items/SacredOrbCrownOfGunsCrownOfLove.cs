using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class ChestModifyItem : PassiveItem
    {
        public Action<Chest> modifyChestAction = SynergyChanceModificationItem.OnSpawnChest;

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            if (!this.m_pickedUpThisRun)
            {
                ItemHelpers.ForEveryChest(modifyChestAction);
            }
        }
    }

    public class SacredOrb : ChestModifyItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(SacredOrb))
        {
            Name = "Crown of Quality",
            Description = "Guarantee",
            LongDescription = "D-Tier chests can no longer spawn, and will be replaced by higher tier chests",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/crownofquality.png",
            Quality = ItemQuality.C,
        };

        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            base.Pickup(player);
        }

        protected override void DisableEffect(PlayerController player)
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
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/crownofwar.png",
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

        protected override void DisableEffect(PlayerController player)
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
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/crownoflove.png",
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

        protected override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            base.DisableEffect(player);
        }
    }
}
