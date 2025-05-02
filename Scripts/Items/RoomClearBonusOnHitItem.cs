using Alexandria;
using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    internal class RoomClearBonusOnHitItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(RoomClearBonusOnHitItem))
        {
            Name = "Consolation Shield",
            PostInitAction = item =>
            {
                ((PassiveItem)item).ArmorToGainOnInitialPickup++;
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += Player_OnReceivedDamage;
            RoomRewardAPI.OnRoomRewardDetermineContents += AddRoomClearChance;
            RoomRewardAPI.OnRoomClearItemDrop += PostItemSpawn;
        }
        public override void DisableEffect(PlayerController player)
        {
            player.OnReceivedDamage += Player_OnReceivedDamage;
            RoomRewardAPI.OnRoomRewardDetermineContents += AddRoomClearChance;
            RoomRewardAPI.OnRoomClearItemDrop += PostItemSpawn;
            base.DisableEffect(player);
        }

        private void PostItemSpawn(DebrisObject arg1, RoomHandler arg2) { DamageTaken--; }
        private void Player_OnReceivedDamage(PlayerController obj) { DamageTaken += perDamage; }

        public int DamageTaken
        {
            get { return DamageTaken; }
            set { DamageTaken = Mathf.Clamp(value, 0, upperLimit * perDamage); }
        }
        private readonly int upperLimit = 5;
        private readonly int perDamage = 3;

        private void AddRoomClearChance(RoomHandler arg1, RoomRewardAPI.ValidRoomRewardContents arg2, float arg3)
        {
            float chance = (float)((Math.Pow(2, DamageTaken) - 1) / Math.Pow(2, DamageTaken));
            arg2.additionalRewardChance -= chance;
        }
    }
}
