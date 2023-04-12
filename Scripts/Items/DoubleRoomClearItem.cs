using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria;
using Dungeonator;
using UnityEngine;

namespace Oddments
{
    public class DoubleRoomClearItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(DoubleRoomClearItem));

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            RoomRewardAPI.OnRoomRewardDetermineContents += ChanceDown;
            RoomRewardAPI.OnRoomClearItemDrop += SpawnItem;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            RoomRewardAPI.OnRoomRewardDetermineContents -= ChanceDown;
            RoomRewardAPI.OnRoomClearItemDrop -= SpawnItem;
        }

        private void SpawnItem(DebrisObject arg1, RoomHandler arg2)
        {
            PickupObject pickup = arg1.GetComponent<PickupObject>();
            if (pickup)
            {
                PickupObject copy = PickupObjectDatabase.GetById(pickup.PickupObjectId);
                LootEngine.DelayedSpawnItem(0.5f, copy.gameObject, arg1.specRigidbody.UnitCenter, Vector2.zero, 1f);
            }
        }

        private void ChanceDown(RoomHandler arg1, RoomRewardAPI.ValidRoomRewardContents arg2, float arg3)
        {
            arg2.additionalRewardChance += 0.33f;
        }
    }
}
