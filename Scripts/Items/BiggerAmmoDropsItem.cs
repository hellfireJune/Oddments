using Dungeonator;
using JuneLib.Items;
using System;
using UnityEngine;

namespace Oddments
{
    public class BiggerAmmoDropsItem : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BiggerAmmoDropsItem));

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            JuneLib.ItemsCore.AddChangeSpawnItem(ChangeAmmoDrops);
        }

        private GameObject ChangeAmmoDrops(PickupObject arg)
        {
            return null;
        }
    }

}
