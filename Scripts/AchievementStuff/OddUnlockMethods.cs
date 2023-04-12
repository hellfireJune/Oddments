using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;
using SaveAPI;
using Dungeonator;

namespace Oddments
{
    public static class OddUnlockMethods
    {
        public static void Init()
        {
            CustomActions.OnRunStart += NewRun;
            CustomActions.OnBossKilled += BossKilled;
        }

        private static void BossKilled(HealthHaver arg1, bool arg2)
        {
            if (arg1.aiActor && arg1.aiActor.ParentRoom != null && !arg1.aiActor.ParentRoom.PlayerHasTakenDamageInThisRoom)
            {
                GlobalDungeonData.ValidTilesets indices = GameManager.Instance.Dungeon.tileIndices.tilesetId;
                switch (indices)
                {
                    default: break;
                    case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
                        SaveAPIManager.SetFlag(CustomDungeonFlags.CADUELCEUS_FLAG, true);
                        break;
                }
            }
        }

        private static readonly int highestVanillaID = 823;
        private static readonly List<int> blackList = new List<int>()
        {
            569, 521
        };

        private static void NewRun(PlayerController arg1, PlayerController arg2, GameManager.GameMode arg3)
        {
            List<PickupObject> bullets = Alexandria.ItemAPI.AlexandriaTags.GetAllItemsWithTag("bullet_modifier");
            List<PickupObject> familiars = Alexandria.ItemAPI.AlexandriaTags.GetAllItemsWithTag("companion");

            bool allVanillaBullets = AllVanillaUnlocked(bullets);
            SaveAPIManager.SetFlag(CustomDungeonFlags.EVERY_VANILLA_BULLET_UNLOCKED, allVanillaBullets);
            bool allVanillaFriends = AllVanillaUnlocked(familiars);
            SaveAPIManager.SetFlag(CustomDungeonFlags.EVERY_VANILLA_COMPANION_UNLOCKED, allVanillaFriends);
        }

        public static bool AllVanillaUnlocked(List<PickupObject> items)
        {
            bool allVanillaUnlocked = true;
            foreach (var bullet in items)
            {
                if (bullet.quality != PickupObject.ItemQuality.EXCLUDED
                    && bullet.PickupObjectId <= highestVanillaID
                    && !blackList.Contains(bullet.PickupObjectId))
                {
                    allVanillaUnlocked &= bullet.PrerequisitesMet();
                }
            }
            return allVanillaUnlocked;
        }
    }
}
