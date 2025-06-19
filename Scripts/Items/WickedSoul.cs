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
using UnityEngine;

namespace Oddments
{ 
    [HarmonyPatch]
    public class WickedSoul : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(WickedSoul))
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
            public static OddItemTemplate template = new OddItemTemplate(typeof(DecanterItem))
            {
                Name = "Decanter",
                Description = "Curse-B-Gone",
                LongDescription = "A miserable little potion of malice and hatred. Drinking from this will negate any effects of curse for the entire floor, and will kill any jammed enemies that spawn.",
                SpriteResource = $"{Module.SPRITE_PATH}/decanter.png",
                Quality = ItemQuality.D,
                PostInitAction = item =>
                {
                    DecanterItem ditem = ((DecanterItem)item);
                    ditem.consumable = true;
                    ditem.numberOfUses = 3;

                    CustomActions.PostDungeonTrueStart += NewFloor;
                    ETGMod.AIActor.OnPostStart += KillJammed;
                }
            };

            private static void KillJammed(AIActor obj)
            {
                if (obj && obj.healthHaver && obj.IsBlackPhantom && !obj.healthHaver.IsBoss && UsedThisFloor)
                {
                    obj.healthHaver.ApplyDamage(27616, Vector2.zero, "Decanted", CoreDamageTypes.None, DamageCategory.Unstoppable);
                }
            }

            private static void NewFloor(Dungeon obj)
            {
                UsedThisFloor = false;
            }

            public static bool UsedThisFloor = false;

            public override void DoEffect(PlayerController user)
            {
                base.DoEffect(user);
                UsedThisFloor = true;

                //LootEngine.SpawnCurrency(user.specRigidbody.UnitCenter, 8);
            }
        }
    }
}
