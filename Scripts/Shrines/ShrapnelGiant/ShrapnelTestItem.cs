using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class ShrapnelTestItem : PassiveItem
    {

        public static ItemTemplate template = new ItemTemplate(typeof(ShrapnelTestItem))
        {
            Name = "SHRAPNEL TEST ITEM",
            Quality = ItemQuality.EXCLUDED,
            /*PostInitAction = item =>
            {
                ShrapnelAbilityBase ability = (ShrapnelAbilityBase)item;
                ability.maxCooldown = 15f;
                ability.type = ShrapnelChargeType.TIMED;
            }*/
        };

        /*public override void Effect()
        {
            ETGModConsole.Log("Did Effect");
        }*/
    }
}
   