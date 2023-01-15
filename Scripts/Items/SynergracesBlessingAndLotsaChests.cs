using Alexandria.Misc;
using Dungeonator;
using HarmonyLib;
using JuneLib.Chests;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oddments
{
    public class SynergracesBlessing : SynergyChanceModificationItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(SynergracesBlessing))
        {
            Name = "Synergrace's Blessing",
            Description = "<3",
            LongDescription = "A blessing from the matchmaker synergrace. Synergy chests will be substantially more common",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/synergracesblessing.png",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            {
                AdditionalSynergyChestChanceDictionary.Add(typeof(SynergracesBlessing), 0.375f);
            }
        };

        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            base.DisableEffect(player);
        }
    }


    public class SynergyChanceModificationItem : ChestModifyItem
    {
        public static Dictionary<Type, float> AdditionalSynergyChestChanceDictionary = new Dictionary<Type, float>();

        public static void InitBase()
        {
            AddModifyChestAction(OnSpawnChest);
        }

        public static void OnSpawnChest(Chest chest)
        {
            RewardManager manager = GameManager.Instance?.RewardManager;
            if (IsFlagSetAtAll(typeof(SacredOrb))
                && manager?.GetQualityFromChest(chest) == ItemQuality.D)
            {

                Chest cChest = manager?.C_Chest;
                chest = ChestHelpers.ReplaceChestWithOtherChest(chest, cChest);
            }

            if (chest.lootTable == null || !chest.lootTable.CompletesSynergy)
            {
                float bonusChance = 0f;
                foreach (Type item in AdditionalSynergyChestChanceDictionary.Keys)
                {
                    if (IsFlagSetAtAll(item))
                    {
                        float addChance = AdditionalSynergyChestChanceDictionary[item];

                        bonusChance += addChance;
                    }
                }

                float num = UnityEngine.Random.value;
                if (num < bonusChance)
                {
                    Chest synergyChest = manager?.Synergy_Chest;
                    //GameManager.Instance.RewardManager.GlobalSynerchestChance = 10f;
                    chest = ChestHelpers.ReplaceChestWithOtherChest(chest, synergyChest);
                }
                else
                if ((IsFlagSetAtAll(typeof(CrownOfGuns)) && chest.ChestType != Chest.GeneralChestType.WEAPON)
                    || (!IsFlagSetAtAll(typeof(CrownOfGuns)) && IsFlagSetAtAll(typeof(CrownOfLove)) && chest.ChestType != Chest.GeneralChestType.ITEM))
                {
                    Chest newchest = manager.GetTargetChestPrefab(manager.GetQualityFromChest(chest));
                    newchest.ChestType = IsFlagSetAtAll(typeof(CrownOfGuns)) ? Chest.GeneralChestType.WEAPON : Chest.GeneralChestType.ITEM;
                    chest = ChestHelpers.ReplaceChestWithOtherChest(chest, newchest);
                }
            }
        }

        /*public static Chest ChangeChestOdds(RewardManager self, IntVector2 positionInRoom, RoomHandler targetRoom, PickupObject.ItemQuality? targetQuality, float overrideMimicChance)
        {
            Type selfType = typeof(RewardManager);
            MethodInfo _GetRewardDataForFloor = selfType.GetMethod("GetRewardDataForFloor", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo _GetTargetChestPrefab = selfType.GetMethod("GetTargetChestPrefab", BindingFlags.NonPublic | BindingFlags.Instance);

            System.Random random = (!GameManager.Instance.IsSeeded) ? null : BraveRandom.GeneratorRandom;
            FloorRewardData rewardDataForFloor = _GetRewardDataForFloor.Invoke(self, new object[] { GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId }) as FloorRewardData; //self.GetRewardDataForFloor(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId);
            if (targetQuality == null)
            {
                targetQuality = new PickupObject.ItemQuality?(rewardDataForFloor.GetRandomTargetQuality(true, StaticReferenceManager.DChestsSpawnedInTotal >= 2 || IsFlagSetAtAll(typeof(SacredOrb))));
                if (IsFlagSetAtAll(typeof(SevenLeafCloverItem)))
                {
                    targetQuality = new PickupObject.ItemQuality?((((random == null) ? UnityEngine.Random.value : ((float)random.NextDouble())) >= 0.5f) ? PickupObject.ItemQuality.S : PickupObject.ItemQuality.A);
                }
            }

            if (targetQuality.GetValueOrDefault() == ItemQuality.D && targetQuality != null
                && ((StaticReferenceManager.DChestsSpawnedOnFloor >= 1 && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON)
                || IsFlagSetAtAll(typeof(SacredOrb))))
            {
                targetQuality = new PickupObject.ItemQuality?(ItemQuality.C);
            }
            Vector2 zero = Vector2.zero;
            if ((targetQuality.GetValueOrDefault() == ItemQuality.A && targetQuality != null || targetQuality.GetValueOrDefault() == ItemQuality.S) && targetQuality != null)
            {
                zero = new Vector2(-0.5f, 0f);
            }
            Chest chest = _GetTargetChestPrefab.Invoke(self, new object[] { targetQuality.Value }) as Chest; //self.GetTargetChestPrefab(targetQuality.Value);


            if (GameStatsManager.Instance.GetFlag(GungeonFlags.SYNERGRACE_UNLOCKED) && GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON)
            {
                float bonusChance = 0f;
                foreach (Type item in allItems)
                {
                    if (IsFlagSetAtAll(item))
                    {
                        float addChance = AdditionalSynergyChestChanceDictionary[item];

                        bonusChance += addChance;
                    }
                }

                float num = (random == null) ? UnityEngine.Random.value : ((float)random.NextDouble());
                float synergyChance = self.GlobalSynerchestChance + bonusChance;
                if (num < synergyChance)
                {
                    chest = self.Synergy_Chest;
                    zero = new Vector2(-0.1875f, 0f);
                }

                ETGModConsole.Log(string.Concat(new object[]
                {
                "synergy ",
                num,
                " | ",
                synergyChance
                }));
            }

            Chest.GeneralChestType generalChestType = (BraveRandom.GenerationRandomValue() >= rewardDataForFloor.GunVersusItemPercentChance) ? Chest.GeneralChestType.ITEM : Chest.GeneralChestType.WEAPON;
            if (((StaticReferenceManager.ItemChestsSpawnedOnFloor > 0 && StaticReferenceManager.WeaponChestsSpawnedOnFloor == 0)
                || IsFlagSetAtAll(typeof(CrownOfGuns))) && !IsFlagSetAtAll(typeof(CrownOfLove)))
            {
                generalChestType = Chest.GeneralChestType.WEAPON;
            }
            else if ((StaticReferenceManager.WeaponChestsSpawnedOnFloor > 0 && StaticReferenceManager.ItemChestsSpawnedOnFloor == 0)
                || IsFlagSetAtAll(typeof(CrownOfLove)))
            {
                generalChestType = Chest.GeneralChestType.ITEM;
            }

            GenericLootTable genericLootTable = (generalChestType != Chest.GeneralChestType.WEAPON) ? self.ItemsLootTable : self.GunsLootTable;
            GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceable(chest.gameObject, targetRoom, positionInRoom, true, AIActor.AwakenAnimationType.Default, false);
            gameObject.transform.position = gameObject.transform.position + zero.ToVector3ZUp(0f);
            Chest component = gameObject.GetComponent<Chest>();
            if (overrideMimicChance >= 0f)
            {
                component.overrideMimicChance = overrideMimicChance;
            }

            Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(IPlaceConfigurable));
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                IPlaceConfigurable placeConfigurable = componentsInChildren[i] as IPlaceConfigurable;
                if (placeConfigurable != null)
                {
                    placeConfigurable.ConfigureOnPlacement(targetRoom);
                }
            }

            if ((targetQuality.GetValueOrDefault() == PickupObject.ItemQuality.A || targetQuality.GetValueOrDefault() == PickupObject.ItemQuality.S) && targetQuality != null)
            {
                GameManager.Instance.Dungeon.GeneratedMagnificence += 1f;
                component.GeneratedMagnificence += 1f;
            }

            if (component.specRigidbody)
            {
                component.specRigidbody.Reinitialize();
            }
            component.ChestType = generalChestType;
            component.lootTable.lootTable = genericLootTable;
            if (component.lootTable.canDropMultipleItems && component.lootTable.overrideItemLootTables != null && component.lootTable.overrideItemLootTables.Count > 0)
            {
                component.lootTable.overrideItemLootTables[0] = genericLootTable;
            }

            if (targetQuality.GetValueOrDefault() == PickupObject.ItemQuality.D && targetQuality != null && !component.IsMimic)
            {
                StaticReferenceManager.DChestsSpawnedOnFloor++;
                StaticReferenceManager.DChestsSpawnedInTotal++;
                component.IsLocked = true;
                if (component.LockAnimator)
                {
                    component.LockAnimator.renderer.enabled = true;
                }
            }

            targetRoom.RegisterInteractable(component);
            if (self.SeededRunManifests.ContainsKey(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId))
            {
                component.GenerationDetermineContents(self.SeededRunManifests[GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId], random);
            }
            return component;
        }*/
    }

    public class LotsaSynergyChests : SynergyChanceModificationItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(LotsaSynergyChests))
        {
            Name = "Crown of Synergies",
            Description = "Grand design",
            LongDescription = "All chests become synergy chests",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/crownofsynergy.png",
            Quality = ItemQuality.A,
            PostInitAction = item =>
            {
                AdditionalSynergyChestChanceDictionary.Add(typeof(LotsaSynergyChests), 10f);
            }
        };

        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            base.DisableEffect(player);
        }
    }
}
