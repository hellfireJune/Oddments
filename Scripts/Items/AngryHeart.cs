using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuneLib.Items;

namespace Oddments
{
    public class AngryHeart : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(AngryHeart))
        {
            Name = "Lead Heart",
            Description = "My Iron Lung",
            LongDescription = "Empty hearts will turn into shields upon taking damage",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/leadheart.png",
            Quality = ItemQuality.D,
        };

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                player.healthHaver.Armor += 3;
            }

            base.Pickup(player);
            player.OnReceivedDamage += Player_OnReceivedDamage;
        }

        private void Player_OnReceivedDamage(PlayerController obj)
        {
            if (obj && obj.healthHaver)
            {
                HealthHaver haver = obj.healthHaver;
                if (haver.GetMaxHealth() - 1 >= haver.GetCurrentHealth())
                {
                    ETGModConsole.Log(haver.GetMaxHealth());
                    ETGModConsole.Log(haver.GetCurrentHealth());
                    haver.SetHealthMaximum(haver.GetCurrentHealth());
                    haver.Armor += 2;
                }
            }
        }
    }
}
