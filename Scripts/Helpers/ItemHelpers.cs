using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static PassiveItem;

namespace Oddments
{
    public static class ItemHelpers
    {

        public static void AddFlagsToPlayer(this PlayerController player, Type type)
        {
            if (!ActiveFlagItems.ContainsKey(player))
            {
                ActiveFlagItems.Add(player, new Dictionary<Type, int>());
            }
            if (!ActiveFlagItems[player].ContainsKey(type))
            {
                ActiveFlagItems[player].Add(type, 1);
            }
            else
            {
                ActiveFlagItems[player][type] = ActiveFlagItems[player][type] + 1;
            }
        }
        public static void RemoveFlagsFromPlayer(this PlayerController player, Type type)
        {

            if (ActiveFlagItems.ContainsKey(player) && ActiveFlagItems[player].ContainsKey(type))
            {
                ActiveFlagItems[player][type] = Mathf.Max(0, ActiveFlagItems[player][type] - 1);
                if (ActiveFlagItems[player][type] == 0)
                {
                    ActiveFlagItems[player].Remove(type);
                }
            }
        }

        public static bool PlayerHasActiveSynergy(this PlayerController player, string synergyNameToCheck)
        {
            foreach (int index in player.ActiveExtraSynergies)
            {
                AdvancedSynergyEntry synergy = GameManager.Instance.SynergyManager.synergies[index];
                if (synergy.NameKey == synergyNameToCheck)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool SynergyActiveAtAll(string synergyNameToCheck)
        {
            for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
            {
                PlayerController player = GameManager.Instance.AllPlayers[i];
                if (player.PlayerHasActiveSynergy(synergyNameToCheck))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
