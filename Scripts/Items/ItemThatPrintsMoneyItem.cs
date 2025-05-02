using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;

namespace Oddments
{
    internal class ItemThatPrintsMoneyItem : PlayerItem
    {
        public static OddItemTemplate template2 = new OddItemTemplate(typeof(ItemThatPrintsMoneyItem))
        {
            PostInitAction = item =>
            {
                var active = item as ItemThatPrintsMoneyItem;
                active.SetCooldownType(ItemBuilder.CooldownType.Damage, 800f);
            }
        };

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            LootEngine.SpawnCurrency(user.specRigidbody.UnitBottomCenter, 20);
        }
    }
}
