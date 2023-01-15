using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using Alexandria.ItemAPI;
using System.Collections;
using Alexandria.Misc;

namespace Oddments
{
    public class GungeonVeins : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(GungeonVeins))
        {
            Name = "Gungeon Veins",
            Description = "Interconnected rooms",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                item.RemovePickupFromLootTables();
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 3);
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnRoomClearEvent += Player_OnRoomClearEvent;
        }

        private void Player_OnRoomClearEvent(PlayerController obj)
        {
            RoomHandler room = obj.CurrentRoom;
            if (room != null && !room.PlayerHasTakenDamageInThisRoom)
            {
                if (UnityEngine.Random.value > 0.15)
                {
                    return;
                }
                List<RoomHandler> adjacentRooms = room.connectedRooms.Where(roomer => roomer.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) != 0 && roomer.visibility == RoomHandler.VisibilityStatus.OBSCURED).ToList();
                if (adjacentRooms.Count <= 0)
                {
                    return;
                }
                RoomHandler roomToClear = BraveUtility.RandomElement(adjacentRooms);
                obj.StartCoroutine(ButcherRoom(roomToClear, obj));
            }
        }

        private IEnumerator ButcherRoom(RoomHandler targetRoom, PlayerController player)
        {
            targetRoom.ClearReinforcementLayers();
            targetRoom.m_hasGivenReward = true;
            List<AIActor> enemies = targetRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (enemies != null)
            {
                List<AIActor> enemiesToKill = new List<AIActor>(enemies);
                for (int i = 0; i < enemiesToKill.Count; i++)
                {
                    AIActor aiactor = enemiesToKill[i];
                    if (aiactor)
                    {
                        aiactor.enabled = true;
                    }
                }
                yield return null;
                for (int j = 0; j < enemiesToKill.Count; j++)
                {
                    AIActor aiactor2 = enemiesToKill[j];
                    if (aiactor2)
                    {
                        UnityEngine.Object.Destroy(aiactor2.gameObject);
                    }
                }
            }
            targetRoom.OnBecameVisible(player);
            Minimap.Instance.RevealMinimapRoom(targetRoom);
            yield break;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            player.OnRoomClearEvent -= Player_OnRoomClearEvent;
        }
    }
}
