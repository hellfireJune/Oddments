using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public static class ChestHelpers
    {
        public static Chest SpawnGenerationChestAt(this RewardManager manager, GameObject chestPrefab, RoomHandler targetRoom, IntVector2 positionInRoom, float overrideMimicChance = 0f, Chest.GeneralChestType generalChestType = Chest.GeneralChestType.UNSPECIFIED)
		{
			System.Random random = (!GameManager.Instance.IsSeeded) ? null : BraveRandom.GeneratorRandom;
			FloorRewardData rewardDataForFloor = manager.CurrentRewardData;
			if (generalChestType == Chest.GeneralChestType.UNSPECIFIED)
            {
				generalChestType = (BraveRandom.GenerationRandomValue() >= rewardDataForFloor.GunVersusItemPercentChance) ? Chest.GeneralChestType.ITEM : Chest.GeneralChestType.WEAPON;
				if (StaticReferenceManager.ItemChestsSpawnedOnFloor > 0 && StaticReferenceManager.WeaponChestsSpawnedOnFloor == 0)
				{
					generalChestType = Chest.GeneralChestType.WEAPON;
				}
				else if (StaticReferenceManager.WeaponChestsSpawnedOnFloor > 0 && StaticReferenceManager.ItemChestsSpawnedOnFloor == 0)
				{
					generalChestType = Chest.GeneralChestType.ITEM;
				}
			}
			GenericLootTable genericLootTable = (generalChestType != Chest.GeneralChestType.WEAPON) ? manager.ItemsLootTable : manager.GunsLootTable;
			GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceable(chestPrefab, targetRoom, positionInRoom, true);
			gameObject.transform.position = gameObject.transform.position /*+ zero.ToVector3ZUp(0f)*/;
			Chest component = gameObject.GetComponent<Chest>();
			if (overrideMimicChance >= 0f)
			{
				component.overrideMimicChance = overrideMimicChance;
			}
			component.ChestType = generalChestType;
			component.lootTable.lootTable = genericLootTable;
			Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(IPlaceConfigurable));
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				IPlaceConfigurable placeConfigurable = componentsInChildren[i] as IPlaceConfigurable;
				if (placeConfigurable != null)
				{
					placeConfigurable.ConfigureOnPlacement(targetRoom);
				}
			}

			PickupObject.ItemQuality targetQuality = manager.GetQualityFromChest(component);
			if (targetQuality == PickupObject.ItemQuality.A)
			{
				GameManager.Instance.Dungeon.GeneratedMagnificence += 1f;
				component.GeneratedMagnificence += 1f;
			}
			else if (targetQuality == PickupObject.ItemQuality.S)
			{
				GameManager.Instance.Dungeon.GeneratedMagnificence += 1f;
				component.GeneratedMagnificence += 1f;
			}
			if (component.specRigidbody)
			{
				component.specRigidbody.Reinitialize();
			}
			if (component.lootTable.canDropMultipleItems && component.lootTable.overrideItemLootTables != null && component.lootTable.overrideItemLootTables.Count > 0)
			{
				component.lootTable.overrideItemLootTables[0] = genericLootTable;
			}
			if (targetQuality == PickupObject.ItemQuality.D && !component.IsMimic)
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
			if (manager.SeededRunManifests.ContainsKey(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId))
			{
				component.GenerationDetermineContents(manager.SeededRunManifests[GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId], random);
			}
			return component;
		}
    }
}
