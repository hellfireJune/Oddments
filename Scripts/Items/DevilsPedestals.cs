using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuneLib.Items;
using Alexandria.Misc;
using Alexandria.DungeonAPI;
using UnityEngine;
using Alexandria.ItemAPI;

namespace Oddments
{
    public class DevilsPedestals : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(DevilsPedestals))
        {
            Name = "evil mastter rounds",
            Quality = ItemQuality.C,
        };

        public static ItemTemplate template2 = new ItemTemplate(typeof(BasicStatPickup))
        {
            Name = "Placeholder Evil Master Round",
            Quality = ItemQuality.SPECIAL,
            PostInitAction = item =>
            {
                BasicStatPickup statPickup = (BasicStatPickup)item;
                statPickup.IsMasteryToken = true;
                EMRid = statPickup.PickupObjectId;

                item.AddPassiveStatModifier(PlayerStats.StatType.Health, 1f);
                item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 0.5f);
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);

                statPickup.RemovePickupFromLootTables();
            }
        };

        public static int EMRid;

        public override void Pickup(PlayerController player)
        {
            CustomActions.OnRewardPedestalDetermineContents += NewAction2;
            CustomActions.OnRewardPedestalSpawned += NewAction;
            base.Pickup(player);
        }

        public void NewAction2(RewardPedestal pedestal, PlayerController player, CustomActions.ValidPedestalContents contents)
        {
            if (pedestal.ContainsMasteryTokenForCurrentLevel())
            {

                pedestal.gameObject.AddComponent<RewardPedestalEvilFlag>();
            }
        }

        public void NewAction(RewardPedestal pedestal)
        {
            if (pedestal.GetComponent<RewardPedestalEvilFlag>() != null)
            {
                PickupObject item = PickupObjectDatabase.GetById(EMRid);
                pedestal.contents = item;

                pedestal.m_itemDisplaySprite.collection = item.sprite.collection;
                pedestal.m_itemDisplaySprite.spriteId = item.sprite.spriteId;
            }
        }


        public class RewardPedestalEvilFlag : MonoBehaviour { }
    }
}
