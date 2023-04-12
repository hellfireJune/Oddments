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

        public static List<int> sprites;
        public static OddItemTemplate template = new OddItemTemplate(typeof(DevilsPedestals))
        {
            Name = "The Master's Pact",
            Description = "To sign away eternity",
            LongDescription = "All master rounds are replaced with cursed \"Dammed Rounds\", which give an additional damage up.",
            SpriteResource = $"{Module.SPRITE_PATH}/devilscontract.png",
            Quality = ItemQuality.B,
        };

        public static OddItemTemplate template2 = new OddItemTemplate(typeof(EvilMasteryItem))
        {
            Name = "Damned Round",
            SpriteResource = $"{Module.SPRITE_PATH}/DammedRound/dammedround_006.png",
            Description = "Hellfire",
            LongDescription = "A token of floor mastery, corrupted and twisted by the infernal depths of bullet hell",
            Quality = ItemQuality.SPECIAL,
            PostInitAction = item =>
            {
                BasicStatPickup statPickup = (BasicStatPickup)item;
                statPickup.IsMasteryToken = true;
                EMRid = statPickup.PickupObjectId;

                item.AddPassiveStatModifier(PlayerStats.StatType.Health, 1f);
                item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 0.1f);
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);

                statPickup.RemovePickupFromLootTables();

                sprites = new List<int>();
                for (int i = 1; i < 6; i++)
                {
                    sprites.Add(SpriteBuilder.AddSpriteToCollection($"{Module.SPRITE_PATH}/DammedRound/dammedround_00{i}.png", item.sprite.Collection));
                }
            }
        };

        public static int EMRid;

        public override void Pickup(PlayerController player)
        {
            CustomActions.OnRewardPedestalDetermineContents += NewAction2;
            CustomActions.OnRewardPedestalSpawned += NewAction;
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            CustomActions.OnRewardPedestalDetermineContents -= NewAction2;
            CustomActions.OnRewardPedestalSpawned -= NewAction;
            base.DisableEffect(player);
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
                EvilMasteryItem item = PickupObjectDatabase.GetById(EMRid) as EvilMasteryItem;
                pedestal.contents = item;

                pedestal.m_itemDisplaySprite.collection = item.sprite.collection;
                int newSpriteId = item. GetSprite();
                pedestal.m_itemDisplaySprite.spriteId = newSpriteId != -1 ? newSpriteId : item.sprite.spriteId;
            }
        }

        public class RewardPedestalEvilFlag : MonoBehaviour { }

        public class EvilMasteryItem : BasicStatPickup
        {
            public override void Pickup(PlayerController player)
            {
                if (!m_pickedUpThisRun)
                {
                    int newSpriteId = GetSprite();
                    if (newSpriteId != -1)
                    {
                        sprite.SetSprite(newSpriteId);
                    }
                }
                base.Pickup(player);
            }

            public int GetSprite()
            {
                GlobalDungeonData.ValidTilesets? tileset = GameManager.Instance.Dungeon?.tileIndices.tilesetId;
                if (tileset != null)
                {
                    switch (tileset) {
                        case GlobalDungeonData.ValidTilesets.CASTLEGEON:
                            return sprites[0];
                        case GlobalDungeonData.ValidTilesets.GUNGEON:
                            return sprites[1];
                        case GlobalDungeonData.ValidTilesets.MINEGEON:
                            return sprites[2];
                        case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
                            return sprites[3];
                        case GlobalDungeonData.ValidTilesets.FORGEGEON:
                            return sprites[4];
                    }
                }
                return -1;
            }
        }
    }
}
