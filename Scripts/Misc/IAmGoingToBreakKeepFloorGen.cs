using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using Alexandria.DungeonAPI;

namespace Oddments
{
    public static class IAmGoingToBreakKeepFloorGen
    {
        public static SharedInjectionData BaseSharedInjectionData;
        public static GenericRoomTable fireplaceTable;

        public static void Init()
        {
            AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");

            BaseSharedInjectionData = sharedAssets2.LoadAsset<SharedInjectionData>("Base Shared Injection Data");
            Dungeon CastlePrefab = DungeonDatabase.GetOrLoadByName("Base_Castle");

            fireplaceTable = CastlePrefab.PatternSettings.flows[0].sharedInjectionData[1].InjectionData[1].roomTable;
            PrototypeDungeonRoom testRoom = RoomFactory.BuildFromResource($"{Module.ASSEMBLY_NAME}/Resources/Rooms/FireplaceTestRoom.room").room;
            CommandsBox.testRoomPrefab = testRoom;
            WeightedRoom weightedRoom = GenerateWeightedRoom(testRoom, 100);

            fireplaceTable.includedRooms.Add(weightedRoom);

            foreach (var wghtdRoom in fireplaceTable.includedRooms.elements)
            {
                var room = wghtdRoom.room;
                ETGModConsole.Log($"{room.name}, {wghtdRoom.weight}, {room.Width}-{room.Height}");
            }

            CastlePrefab = null;
        }

        public static WeightedRoom GenerateWeightedRoom(PrototypeDungeonRoom Room, float Weight = 1, bool LimitedCopies = true, int MaxCopies = 1, DungeonPrerequisite[] AdditionalPrerequisites = null)
        {
            if (Room == null) { return null; }
            if (AdditionalPrerequisites == null) { AdditionalPrerequisites = new DungeonPrerequisite[0]; }
            return new WeightedRoom() { room = Room, weight = Weight, limitedCopies = LimitedCopies, maxCopies = MaxCopies, additionalPrerequisites = AdditionalPrerequisites };
        }
    }
}
