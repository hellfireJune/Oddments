using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using Alexandria.Misc;

namespace Oddments
{
    public class SafetyScissors : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(SafetyScissors))
        {
            Name = "Safety Scissors",
            Description = "Shoot the Fuse!", //https://www.youtube.com/watch?v=4Emb7zasmRc
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/safetyscissors.png",
            LongDescription = "Chests will no longer spawn with fuses\n\nEven a blade as blunt as this incurrs the wrath of the jammed",
            Quality = ItemQuality.D,
            PostInitAction = item =>
            {
                //Hook hook = new Hook(typeof(Chest).GetMethod("RoomEntered", BindingFlags.Instance | BindingFlags.NonPublic), typeof(SafetyScissors).GetMethod("PreventFuses"));
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);
            }
        };

        public static void PreventFuses(Action<Chest, PlayerController> orig, Chest self, PlayerController enterer)
        {
            Type type = typeof(Chest);
            FieldInfo _isCoopMode = type.GetField("m_IsCoopMode", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo _hasBeenCheckedForFuses = type.GetField("m_hasBeenCheckedForFuses", BindingFlags.Instance | BindingFlags.NonPublic);

            MethodInfo _UnbecomeCoopChest = type.GetMethod("UnbecomeCoopChest", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo _TriggerCountdownTimer = type.GetMethod("TriggerCountdownTimer", BindingFlags.Instance | BindingFlags.NonPublic);

            bool coopMode = (bool)_isCoopMode.GetValue(null);
            if (coopMode && GameManager.HasInstance && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.healthHaver.IsAlive && GameManager.Instance.SecondaryPlayer && GameManager.Instance.SecondaryPlayer.healthHaver.IsAlive)
            {
                _UnbecomeCoopChest.Invoke(self, new object[] { });
            }
            bool checkedforFuses = (bool)_hasBeenCheckedForFuses.GetValue(self);
            if (!coopMode && !self.IsOpen && !self.IsBroken && !checkedforFuses && !self.PreventFuse && self.ChestIdentifier != Chest.SpecialChestIdentifier.RAT)
            {
                _hasBeenCheckedForFuses.SetValue(self, true);
                float num = 0.02f + 1f;
                num += PlayerStats.GetTotalCurse() * 0.05f;
                num += PlayerStats.GetTotalCoolness() * -0.025f;
                num = Mathf.Max(0.01f, num);
                ETGModConsole.Log(self.lootTable.CompletesSynergy);
                if (self.lootTable != null && self.lootTable.CompletesSynergy)
                {
                    num = 1f;
                } else if (IsFlagSetAtAll(typeof(SafetyScissors)))
                {
                    num -= 1f;
                }
                float chance = UnityEngine.Random.value;
                if (chance < num)
                {
                    _TriggerCountdownTimer.Invoke(self, new object[] { });
                    AkSoundEngine.PostEvent("Play_OBJ_fuse_loop_01", self.gameObject);
                }

                ETGModConsole.Log(string.Concat(new object[]
                {
                "fuse ",
                chance,
                " | ",
                num
                }));
            }
        }

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
