using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public static class UnlockCommands
    {
        public static void Init()
        {

            ConsoleCommandGroup group = ETGModConsole.Commands.AddGroup("oddments", args =>
            {
                Module.Log("Please specify a valid command.", Module.TEXT_COLOR);
            });
            ETGModConsole.Commands.GetGroup("oddments").AddUnit("unlocks", args =>
            {

                int totalItems = 0;
                int unlockedItems = 0;
                List<PickupObject> list = new List<PickupObject>();
                List<PickupObject> list2 = new List<PickupObject>();
                foreach (int id in JuneSaveManagerCore.unlocksDict.Keys)
                {
                    PickupObject item = PickupObjectDatabase.GetById(id);
                    EncounterTrackable trolling = item.GetComponent<EncounterTrackable>();
                    if (trolling)
                    {
                        totalItems++;
                        if (trolling.PrerequisitesMet())
                        {
                            unlockedItems++;
                            list2.Add(item);
                        }
                        else
                        {
                            list.Add(item);
                        }
                    }
                }
                if (list2.Count > 0)
                {
                    Module.Log("Items Unlocked:", Module.TEXT_COLOR);
                    foreach (PickupObject item in list2)
                    {
                        Module.Log(item.name, Module.TEXT_COLOR);
                    }
                }
                if (list.Count > 0)
                {
                    Module.Log("Items Left:", Module.TEXT_COLOR);
                    foreach (PickupObject item in list)
                    {
                        Module.Log($"{item.name}: {JuneSaveManagerCore.unlocksDict[item.PickupObjectId]}", Module.TEXT_COLOR);
                    }
                }
                Module.Log($"Unlocked: {unlockedItems}/{totalItems}", Module.TEXT_COLOR);
            });
            ETGModConsole.Commands.GetGroup("oddments").AddUnit("unlockall", args =>
            {
                GameStatsManager.Instance.SetFlag((GungeonFlags)OddmentsSaveFlags.GetFlag(OddFlags.FLAG_ODDMENTS_UNLOCK_ALL), 
                    GameStatsManager.Instance.GetFlag((GungeonFlags)OddmentsSaveFlags.GetFlag(OddFlags.FLAG_ODDMENTS_UNLOCK_ALL)) ^ true);

                Module.Log($"Unlock All is now { GameStatsManager.Instance.GetFlag((GungeonFlags)OddmentsSaveFlags.GetFlag(OddFlags.FLAG_ODDMENTS_UNLOCK_ALL))}", Module.TEXT_COLOR);
                Module.Log("Please note you will have to restart the game for this to take effect");
            });
        }
    }
}
