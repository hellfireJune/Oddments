using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.DungeonAPI;
using GungeonAPI;
using UnityEngine;

namespace Oddments
{
    public static class DerringerShrine
    {
        public static void Add()
        {
            ShrineFactory shrine = new ShrineFactory()
            {
                name = "DerringerShrine",
                modID = Module.PREFIX,
                text = "testooo",
                spritePath = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/Shrines/placeholdershrineguy.png",
                RoomWeight = 100f,
                acceptText = "truuuue",
                declineText = "Kill Yourself",
                CanUse = CanUse,
                OnAccept = Accept,
                offset = new Vector3(-1f, -1f, 0),
                talkPointOffset = new Vector3(0, 3, 0),
                isToggle = false,
                isBreachShrine = false,

                preRequisites = new DungeonPrerequisite[0],
                ShrinePercentageChance = 0.2f,
            };
            shrine.BuildWithoutBaseGameInterference();
        }

        public static bool CanUse(PlayerController player, GameObject shrineObject)
        {
            return true;
        }


        public static void Accept(PlayerController player, GameObject shrine)
        {
            ETGModConsole.Log("sick tricks, june");
        }
    }
}
