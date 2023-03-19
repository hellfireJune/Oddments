using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments 
{
    internal class ArmorGeneratingOnRoomClearItem : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ArmorGeneratingOnRoomClearItem));
        public int roomsCleared=0;
        public int timesPayedOut=0;

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnRoomClearEvent += Player_OnRoomClearEvent;
        }

        private void Player_OnRoomClearEvent(PlayerController obj)
        {
            roomsCleared++;
            if (roomsCleared > (2^timesPayedOut))
            {
                timesPayedOut++;
                roomsCleared = 0;

                obj.healthHaver.Armor++;
            }
        }


        public override void MidGameSerialize(List<object> data)
        {
            base.MidGameSerialize(data);
            data.Add(timesPayedOut);
            data.Add(roomsCleared);
        }

        public override void MidGameDeserialize(List<object> data)
        {
            base.MidGameDeserialize(data);
            timesPayedOut = (int)data[0];
            roomsCleared = (int)data[1];
        }
    }
}
