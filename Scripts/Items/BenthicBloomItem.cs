using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class BenthicBloomItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(BenthicBloomItem))
        {
            PostInitAction = item =>
            {
                item.RemovePickupFromLootTables();
            }
        };

        private readonly List<ItemQuality> validQualites = new List<ItemQuality>() { ItemQuality.D, ItemQuality.C, ItemQuality.B, ItemQuality.A };
        private readonly float m_itemsToUpgrade = 4;
        public void UpgradeItems(PlayerController player)
        {
            List<PassiveItem> items = player.passiveItems.Where(passiveItem => passiveItem != null && passiveItem.CanBeDropped && validQualites.Contains(passiveItem.quality)).ToList();
            for (int i = 0; i < m_itemsToUpgrade; i++)
            {
                PassiveItem passiveItem = BraveUtility.RandomElement(items);

                ItemQuality quality = passiveItem.quality.Next();
                DebrisObject obj = player.DropPassiveItem(passiveItem);
                Destroy(obj.gameObject);

                PassiveItem item = LootEngine.GetItemOfTypeAndQuality<PassiveItem>(quality, GameManager.Instance.RewardManager.ItemsLootTable);
                player.AcquirePassiveItemPrefabDirectly(item);
            }
        }
        public override void Update()
        {
            base.Update();
            if (Owner
                && !GameManager.Instance.IsLoadingLevel
                && queueUpgradeItem)
            {
                queueUpgradeItem = false;
                UpgradeItems(Owner);
            }
        }

        public void StartFloor(Dungeon dungeon)
        {
            if (Owner)
            {
                queueUpgradeItem = true;
            }
        }
        private bool queueUpgradeItem = false;
    }
}
