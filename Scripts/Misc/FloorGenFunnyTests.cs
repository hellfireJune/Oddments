using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using Alexandria.DungeonAPI;
using HarmonyLib;

namespace Oddments
{
    [HarmonyPatch]
    public static class FloorGenFunnyTests
    {
        public static SharedInjectionData BaseSharedInjectionData;
        public static MetaInjectionData BaseMetaInjectionData;
        public static GenericRoomTable fireplaceTable;

        public static void Init()
        {
            ETGModConsole.Log("Loading funny room stuff", true);
            AssetBundle sharedAssets1 = ResourceManager.LoadAssetBundle("shared_auto_001");
            AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");

            BaseSharedInjectionData = sharedAssets2.LoadAsset<SharedInjectionData>("Base Shared Injection Data");
            BaseMetaInjectionData = sharedAssets1.LoadAsset<MetaInjectionData>("_Meta Injection Data");
            //ETGModConsole.Log(metainjection.GetType());
            Dungeon CastlePrefab = DungeonDatabase.GetOrLoadByName("Base_Castle");

            fireplaceTable = CastlePrefab.PatternSettings.flows[0].sharedInjectionData[1].InjectionData[1].roomTable;
            PrototypeDungeonRoom testRoom = RoomFactory.BuildFromResource($"{Module.ASSEMBLY_NAME}/Resources/Rooms/FireplaceTestRoom.room").room;
            for (int i = 0; i < testRoom.Height; i++)
            {
                for (int j = 0; j < testRoom.Width; j++)
                {
                    /*PrototypeDungeonRoomCellData cellData =*/ testRoom.m_cellData[j + (i * testRoom.Width)].appearance.OverrideFloorType = CellVisualData.CellFloorType.Flesh;
                    //cellData;
                }
            }
            BaseMetaInjectionData.entries[0].injectionData.InjectionData[0].exactRoom = testRoom;
            DungeonMaterial material = CastlePrefab.roomMaterialDefinitions[0];
            CastlePrefab.roomMaterialDefinitions = new DungeonMaterial[] { material }; //clueless
            CastlePrefab.BossMasteryTokenItemId = 21;
            /*testRoom.category = PrototypeDungeonRoom.RoomCategory.SPECIAL;
            testRoom.subCategorySpecial = PrototypeDungeonRoom.RoomSpecialSubCategory.STANDARD_SHOP;*/
            CommandsBox.testRoomPrefab = testRoom;
            WeightedRoom weightedRoom = GenerateWeightedRoom(testRoom, 100);

            fireplaceTable.includedRooms.Add(weightedRoom);

            foreach (var wghtdRoom in fireplaceTable.includedRooms.elements)
            {
                var room = wghtdRoom.room;
                ETGModConsole.Log($"{room.name}, {wghtdRoom.weight}, {room.Width}-{room.Height}");
            }
            /*foreach (var node in CastlePrefab.PatternSettings.flows[0].m_nodes)
            {
                ETGModConsole.Log("Node:");
                ETGModConsole.Log($"{node.overrideRoomTable}, {node.overrideExactRoom}, {node.nodeType}, {node.roomCategory}");
            }*/

            //CastlePrefab = null;
        }

        [HarmonyPatch(typeof(PrototypeDungeonRoomCellAppearance), nameof(PrototypeDungeonRoomCellAppearance.GetOverridesForTilemap))]
        [HarmonyPrefix]
        public static bool ReturnNull(ref List<int> __result)
        {
            //ETGModConsole.Log("Returning null");
            __result = null;
            return true;
        }

        public static WeightedRoom GenerateWeightedRoom(PrototypeDungeonRoom Room, float Weight = 1, bool LimitedCopies = true, int MaxCopies = 1, DungeonPrerequisite[] AdditionalPrerequisites = null)
        {
            if (Room == null) { return null; }
            if (AdditionalPrerequisites == null) { AdditionalPrerequisites = new DungeonPrerequisite[0]; }
            return new WeightedRoom() { room = Room, weight = Weight, limitedCopies = LimitedCopies, maxCopies = MaxCopies, additionalPrerequisites = AdditionalPrerequisites };
        }
    }
}
