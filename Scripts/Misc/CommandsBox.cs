using Alexandria.DungeonAPI;
using Dungeonator;
using Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class CommandsBox
    {
        public static PrototypeDungeonRoom testRoomPrefab;

        private static RoomHandler testRoom;
        public static void Init()
        {
            /*ConsoleCommandGroup group = ETGModConsole.Commands.AddGroup("oddments", args =>
            {
                Module.Log("Please specify a valid command.", Module.TEXT_COLOR);
            });*/
            ETGModConsole.Commands.GetGroup("oddments").AddUnit("warptest", action: args =>
            {
                if (args.Contains("reverse"))
                {
                    Pixelator.Instance.FadeToColor(0.25f, Color.white, true, 0.125f);
                    GameManager.Instance.BestActivePlayer.WarpToPoint(new Vector2(20, 20), true, true);
                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                    {
                        GameManager.Instance.GetOtherPlayer(GameManager.Instance.BestActivePlayer).ReuniteWithOtherPlayer(GameManager.Instance.BestActivePlayer, false);
                    }
                    return;
                }

                Dungeon D = GameManager.Instance.Dungeon;
                if (testRoom == null)
                {
                    testRoom = D.AddRuntimeRoom(testRoomPrefab);
                }

                Vector2 oldPlayerPosition = GameManager.Instance.BestActivePlayer.transform.position.XY();
                Vector2 newPlayerPosition = testRoom.area.Center;
                Pixelator.Instance.FadeToColor(0.25f, Color.white, true, 0.125f);
                Pathfinder.Instance.InitializeRegion(D.data, testRoom.area.basePosition, testRoom.area.dimensions);
                GameManager.Instance.BestActivePlayer.WarpToPoint(newPlayerPosition, true, true);
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    GameManager.Instance.GetOtherPlayer(GameManager.Instance.BestActivePlayer).ReuniteWithOtherPlayer(GameManager.Instance.BestActivePlayer, false);
                }
            });

            ETGModConsole.Commands.GetGroup("oddments").AddUnit("getroomtype", args =>
            {
                ETGModConsole.Log(GameManager.Instance.PrimaryPlayer.CurrentRoom.RoomVisualSubtype);
            });

            UIUnlocksTest.Init();
        }
    }
}
