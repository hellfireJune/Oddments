using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuneLib;
using Dungeonator;
using Alexandria.Misc;

namespace Oddments
{
    public class RoomClearOnlyHeals : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RoomClearOnlyHeals))
        {

            PostInitAction = item =>
            {
                item.RemovePickupFromLootTables();
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            JuneLibCore.OnRoomRewardDetermineContents += PreRoomClearReward;
        }

        public override void DisableEffect(PlayerController player)
        {
            JuneLibCore.OnRoomRewardDetermineContents -= PreRoomClearReward;
            base.DisableEffect(player);
        }

        public void PreRoomClearReward(RoomHandler room, JuneLibRoomRewardAPI.ValidRoomRewardContents contents, float chance)
        {
            foreach (var i in healthPickups)
            {
                contents.overrideItemPool.Add(new Tuple<float, int>( i.First, i.Second));
            }
        }

        public static readonly List<Tuple<float, int>> healthPickups = new List<Tuple<float, int>> { new Tuple<float, int>(0.33f, GlobalItemIds.FullHeart), new Tuple<float, int>(0.33f, GlobalItemIds.FullHeart), new Tuple<float, int>(0.33f, GlobalItemIds.FullHeart) };
    }
}
