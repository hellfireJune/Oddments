using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuneLib.Items;

namespace Oddments
{
    public class DoubleHealingItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(DoubleHealingItem))
        {
            Name = "Rejuvenation Rack",
            Description = "Breathe in the air",
            LongDescription = "All healing is doubled",
            SpriteResource = $"{Module.SPRITE_PATH}/rejuvenatingrack.png",
            Quality = ItemQuality.A,
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.healthHaver.ModifyHealing += ModifyHealing;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if(player)
                player.healthHaver.ModifyHealing -= ModifyHealing;
        }

        public void ModifyHealing(HealthHaver healthhaver, HealthHaver.ModifyHealingEventArgs args)
        {
            args.ModifiedHealing *= 2;
        }
    }
}
