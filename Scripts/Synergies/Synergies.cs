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
        }
    }
}
