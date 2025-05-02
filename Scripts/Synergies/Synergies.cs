using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;

namespace Oddments
{
    public class Synergies
    {
        public static void Init()
        {
            CustomSynergies.Add(HelixBulletsSinWave.SynergyName, HelixBulletsSinWave.IDs);
            CustomSynergies.Add(MimicBait.GnarlySynergyName, MimicBait.GnarlyIDs);
            CustomSynergies.Add(OddStatusEffectModifierItem.CavitySynergyName, OddStatusEffectModifierItem.CavityIDs);
            CustomSynergies.Add(ShotsFiredToDamageUpItem.RedoxSynergyName, ShotsFiredToDamageUpItem.ids)
                .statModifiers = new List<StatModifier>() { new StatModifier() 
                    { statToBoost = PlayerStats.StatType.ReloadSpeed, modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE, amount = 0.66f } 
                };

        }
    }
}
