using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class BlackCatItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(BlackCatItem))
        {
            PostInitAction = item =>
            {
                item.RemovePickupFromLootTables();
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnRoomClearEvent += ClearBossHopefully;
        }

        private int m_pickups = 5;
        private void ClearBossHopefully(PlayerController player)
        {
            RoomHandler room = player.CurrentRoom;
            if (room == null || room.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS) { return; }

            
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
        }
    }
}
