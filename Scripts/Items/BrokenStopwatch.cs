using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using JuneLib.Items;

namespace Oddments
{
    public class BrokenStopwatch : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BrokenStopwatch))
        {
            Name = "Broken Stopwatch",
            Description = "Slowly, slowly, slowly",
            LongDescription = "Increases damage, but the speed at time which the gungeon flows faster while held",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/brokenstopwatch.png",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            { item.AddPassiveStatModifier(PlayerStats.StatType.EnemyProjectileSpeedMultiplier, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 0.15f);
                item.AddPassiveStatModifier(PlayerStats.StatType.DodgeRollSpeedMultiplier, 1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                item.AddPassiveStatModifier(PlayerStats.StatType.MovementSpeed, 1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            }
        };
    }
}                  