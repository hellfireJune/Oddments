using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using HarmonyLib;
using Dungeonator;
using Alexandria.Misc;
using JuneLib.Items;
using SaveAPI;

namespace Oddments
{ 
    [HarmonyPatch]
    public class WickedSoul : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(WickedSoul))
        {
            Name = "Wicked Soul",
            Description = "Something Wicked",
            LongDescription = "Gives great strength, at the cost of increasing the potency of any curse related effects\n\nWill prevent any interference from the Lord of the Jammed while held",
            SpriteResource = $"{Module.SPRITE_PATH}/wickedsou.png",
            Quality = ItemQuality.A,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 0.5f);
                item.AddPassiveStatModifier(PlayerStats.StatType.Health, 2f);

                item.SetupUnlockOnMaximum(TrackedMaximums.HIGHEST_CURSE_LEVEL, 9, DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN);
                item.AddUnlockText("Get 9 or more curse in one run");
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

        [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.GetTotalCurse))]
        [HarmonyPostfix]
        public static void ThisWayComes(ref int __result)
        {
            if (IsFlagSetAtAll(typeof(WickedSoul)))
            {
                __result *= 2;
            }
            if (DecanterItem.UsedThisFloor)
            {
                __result = 0;
            }
        }

        [HarmonyPatch(typeof(Dungeon), nameof(Dungeon.SpawnCurseReaper))]
        [HarmonyPrefix]
        public static bool NoCurseReaperPlease()
        {
            if (IsFlagSetAtAll(typeof(WickedSoul)))
            {
                return false;
            }
            return true;
        }

        public class DecanterItem : PlayerItem
        {
            public static ItemTemplate template = new ItemTemplate(typeof(DecanterItem))
            {
                Quality = ItemQuality.D,
                PostInitAction = item =>
                {
                    DecanterItem ditem = ((DecanterItem)item);
                    ditem.consumable = true;
                    ditem.numberOfUses = 3;

                    CustomActions.PostDungeonTrueStart += NewFloor;
                }
            };

            private static void NewFloor(Dungeon obj)
            {
                UsedThisFloor = false;
            }

            public static bool UsedThisFloor = false;

            public override void DoEffect(PlayerController user)
            {
                base.DoEffect(user);
                UsedThisFloor = true;

                LootEngine.SpawnCurrency(user.specRigidbody.UnitCenter, 8);
            }
        }
    }
}
