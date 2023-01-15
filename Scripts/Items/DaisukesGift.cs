using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using JuneLib.Items;

namespace Oddments
{
    public class DaisukesGift : PassiveItem
    {
        public static readonly string longDescStart = "Grants 3 random stat ups on pickup\n\nAn old keepsake of the trickster dice Daisuke. ";
        public static readonly string longDescEnd = "You can see three sigils marked upon it, but you can't make them out right now.";

        public static ItemTemplate template = new ItemTemplate(typeof(DaisukesGift))
        {
            Name = "Daisuke's Gift",
            Description = "Random up!",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/daisukesshard.png",
            LongDescription = longDescStart + longDescEnd,
            Quality = ItemQuality.B,
        };


        public static string GenerateNewLongDesc(PlayerStats.StatType type1, PlayerStats.StatType type2, PlayerStats.StatType type3)
        {
            return "You can see three sigils marked upon it, one which looks like " + ammonomiconDescriptor[type1]
                + ", one which looks like " + ammonomiconDescriptor[type2]
                + ", and another one which looks like " + ammonomiconDescriptor[type3] + ".";
        }

        private List<PlayerStats.StatType> list = new List<PlayerStats.StatType>();
        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun || list.Count == 0)
            {
                this.passiveStatModifiers = new StatModifier[0];
                list.Clear();
                for (int i = 0; i < m_statsToAdd; i++)
                {
                    PlayerStats.StatType type;
                    do
                        type = BraveUtility.RandomElement(increments.Keys.ToList());
                    while (list.Contains(type));
                    list.Add(type);
                    this.AddPassiveStatModifier(type, increments[type]);
                }
            }

            this.SetLongDescription(longDescStart + GenerateNewLongDesc(list[0], list[1], list[2]));
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController disablingPlayer)
        {
            base.DisableEffect(disablingPlayer);

            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (player.HasPassiveItem(this.PickupObjectId)
                    && !GameManager.Instance.IsLoadingLevel)
                {
                    return;
                }
            }
            this.SetLongDescription(longDescStart + longDescEnd);
        }

        private int m_statsToAdd = 3;

        public static Dictionary<PlayerStats.StatType, float> increments = new Dictionary<PlayerStats.StatType, float>()
        {
            { PlayerStats.StatType.Curse, 1f },
            { PlayerStats.StatType.Health, 1f },
            { PlayerStats.StatType.Accuracy, -0.2f },
            { PlayerStats.StatType.Coolness, 1f },
            { PlayerStats.StatType.DamageToBosses, 0.2f },
            { PlayerStats.StatType.Damage, 0.1f },
            { PlayerStats.StatType.MovementSpeed, 1f },
            { PlayerStats.StatType.RateOfFire, 0.2f },
            { PlayerStats.StatType.ReloadSpeed, -0.2f },
            { PlayerStats.StatType.MoneyMultiplierFromEnemies, 0.15f },
        };

        static Dictionary<PlayerStats.StatType, string> ammonomiconDescriptor = new Dictionary<PlayerStats.StatType, string>()
        {
            { PlayerStats.StatType.Curse, "a skull" },
            { PlayerStats.StatType.Health, "a heart" },
            { PlayerStats.StatType.Accuracy, "a crosshair" },
            { PlayerStats.StatType.Coolness, "a pair of sunglasses" },
            { PlayerStats.StatType.DamageToBosses, "the Bullet King, dead" },
            { PlayerStats.StatType.Damage, "a knife" },
            { PlayerStats.StatType.MovementSpeed, "some boots" },
            { PlayerStats.StatType.RateOfFire, "a flurry of bullets" },
            { PlayerStats.StatType.ReloadSpeed, "a chamber" },
            { PlayerStats.StatType.MoneyMultiplierFromEnemies, "Bello's face" },
        };
    }
}
