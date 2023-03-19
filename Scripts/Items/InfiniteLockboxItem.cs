using Dungeonator;
using JuneLib.Items;
using System;
using UnityEngine;

namespace Oddments
{
    public class InfiniteLockboxItem : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(InfiniteLockboxItem))
        {
            Name = "Infinite Lockbox",
            Description = "Down the chest-hole",
            LongDescription = "Uses all of your keys to spawn an up-to infinite amount of rewards\n\nOpening one reveals an equally as large box inside with just as many locks",
            Quality = ItemQuality.C,
            SpriteResource = $"{Module.SPRITE_PATH}/infinitelockbox.png",
            PostInitAction = item =>
            {
                ((PlayerItem)item).consumable = true;
            }
        };

        public override bool CanBeUsed(PlayerController user)
        {
            if (user.carriedConsumables.KeyBullets <= 0)
            {
                return false;
            }
            return base.CanBeUsed(user);
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            int keybullets = user.carriedConsumables.KeyBullets;

            FloorRewardData data = GameManager.Instance.RewardManager.CurrentRewardData;
            for (int i = 0; i < Math.Floor(keybullets * 1.5); i++)
            {
                GameObject pickup;
                do
                {
                    pickup = data.SingleItemRewardTable.SelectByWeight();
                }
                while (pickup.GetComponent<KeyBulletPickup>() != null);
                BraveUtility.RandomVector2(new Vector2(1, 0), new Vector2(0, 1));
                Vector2 area = LastOwner.CurrentRoom.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.PlayerCenter).ToVector2();
                IntVector2 spawnPoint = LastOwner.CurrentRoom.GetBestRewardLocation(new IntVector2(1, 1), BraveUtility.RandomVector2(area - new Vector2(6, 6), 
                    area + new Vector2(6, 6)));
                DebrisObject item = LootEngine.SpawnItem(pickup, spawnPoint.ToVector3() + new Vector3(0.25f, 0f, 0f), Vector2.up, 1f, true, true);
            }
            user.carriedConsumables.KeyBullets = 0;
        }
    }
}
