using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuneLib;
using Dungeonator;
using Alexandria;

namespace Oddments
{
    public class RoomClearOnlyHeals : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(RoomClearOnlyHeals))
        {
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            RoomRewardAPI.OnRoomRewardDetermineContents += PreRoomClearReward;
        }

        public override void DisableEffect(PlayerController player)
        {
            RoomRewardAPI.OnRoomRewardDetermineContents -= PreRoomClearReward;
            base.DisableEffect(player);
        }

        public void PreRoomClearReward(RoomHandler room, RoomRewardAPI.ValidRoomRewardContents contents, float chance)
        {
            foreach (var i in healthPickups)
            {
                contents.overrideItemPool.Add(new Tuple<float, int>( i.First, i.Second));
            }
        }

        public static readonly List<Tuple<float, int>> healthPickups = new List<Tuple<float, int>> { new Tuple<float, int>(0.33f, GlobalItemIds.FullHeart), new Tuple<float, int>(0.33f, GlobalItemIds.FullHeart), new Tuple<float, int>(0.33f, GlobalItemIds.FullHeart) };
    }
}
