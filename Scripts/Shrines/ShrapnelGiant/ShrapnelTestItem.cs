using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;
using JuneLib.Items;

namespace Oddments
{
    public class ShrapnelTestItem : ShrapnelAbilityBase
    {

        public static OddItemTemplate template = new OddItemTemplate(typeof(ShrapnelTestItem))
        {
            Name = "SHRAPNEL TEST ITEM",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                item.RemovePickupFromLootTables();
                ShrapnelAbilityBase ability = (ShrapnelAbilityBase)item;
                ability.maxCooldown = 15f;
                ability.type = ShrapnelChargeType.TIMED;
            }
        };

        public override void Effect()
        {
            ETGModConsole.Log("Did Effect");
        }
    }
}
   