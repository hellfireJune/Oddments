using Alexandria.ItemAPI;
using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class PickupGenerator : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PickupGenerator))
        {
            Name = "The Book of Sin",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/bookofsin.png",
            Description = "Wrath of the Jam",
            LongDescription = "Spawns a random pickup on use. Cooldown is based on rooms cleared.",
            Quality = ItemQuality.B,
            Cooldown = 6f,
            CooldownType = ItemBuilder.CooldownType.PerRoom,
            PostInitAction = item =>
            {
                item.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);
            }
        };

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            FloorRewardData data = GameManager.Instance.RewardManager.CurrentRewardData;
            if (data != null && user && user.CurrentRoom != null)
            {
                GameObject pickup = data.SingleItemRewardTable.SelectByWeight();
                LootEngine.SpawnItem(pickup, LastOwner.CurrentRoom.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.CameraCenter).ToVector3() + new Vector3(0.25f, 0f, 0f), Vector2.up, 1f, true, true);
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            if (user && user.CurrentRoom != null)
            {
                return base.CanBeUsed(user);
            }
            return false;
        }
    }

    public class BunchaChoicePickupsYknow : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BunchaChoicePickupsYknow))
        {
            Name = "Derringer's Sack",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/derringerssack.png",
            Description = "Bountiful Harvest",
            LongDescription = "An old tool of the god of harvest, Derringer. Contains anything you'd need",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            {
                (item as PlayerItem).consumable = true;
            }
        };

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            FloorRewardData data = GameManager.Instance.RewardManager.CurrentRewardData;
            if (data != null && user && user.CurrentRoom != null)
            {
                ETGMod.StartGlobalCoroutine(SpawningStuff(data, m_maxSpawns, m_pickupSpawns, m_xIncrement));
            }
        }

        private int m_pickupSpawns = 3;
        private int m_maxSpawns = 3;
        private float m_xIncrement = 1.5f;

        public IEnumerator SpawningStuff(FloorRewardData data, int maxSpawns, int pickupSpawns, float xIncrement)
        {
            float spawns = maxSpawns;
            bool doSpawn = true;
            List<DebrisObject> list = new List<DebrisObject>(pickupSpawns);

            while (spawns > 0)
            {
                if (doSpawn)
                {
                    list.Clear();
                    for (int i = 0; i < pickupSpawns; i++)
                    {
                        GameObject pickup;
                        do
                        {
                            pickup = data.SingleItemRewardTable.SelectByWeight();
                        }
                        while (pickup.GetComponent<PickupMover>() != null);
                        list.Add(LootEngine.SpawnItem(pickup, LastOwner.CurrentRoom.GetBestRewardLocation(new IntVector2(1, 1),
                            RoomHandler.RewardLocationStyle.CameraCenter).ToVector3() + new Vector3((m_xIncrement * i) - (m_xIncrement * 2), 0f, 0f), Vector2.up, 1f, true, true));
                    }
                    doSpawn = false;
                }
                else
                {
                    bool getRidOfItAll = false;
                    foreach (DebrisObject item in list)
                    {
                        if (!item)
                        {
                            getRidOfItAll = true;
                        }
                    }

                    if (getRidOfItAll)
                    {
                        foreach (DebrisObject item in list)
                        {
                            if (item)
                            {
                                LootEngine.DoDefaultItemPoof(item.transform.position);
                                Destroy(item.gameObject);
                            }
                        }
                        spawns--;
                        doSpawn = true;
                        yield return new WaitForSeconds(0.75f);
                    }
                }
                yield return null;
            }
            yield break;
        }
        public override bool CanBeUsed(PlayerController user)
        {
            if (user && user.CurrentRoom != null)
            {
                return base.CanBeUsed(user);
            }
            return false;
        }
    }
}
