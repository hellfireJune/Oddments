using Dungeonator;
using JuneLib.Items;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Oddments
{
    public class MimicBait : PassiveItem
    {
        public static readonly List<string> GnarlyIDs = new List<string>
                {
                    $"{Module.PREFIX}:mimic_bait",
                    "mimic_tooth_necklace"
            };
        public const string GnarlySynergyName = "COFFEE IS TURNING GNARLY";
        public static OddItemTemplate template = new OddItemTemplate(typeof(MimicBait))
        {
            Name = "Mimic Bait",
            Description = "Strong Coffee!",
            LongDescription = "Increases the chance for mimics to appear as chests.\n\nAs hard workers, mimics cant resist a cup of Coffee.",
            SpriteResource = $"{Module.SPRITE_PATH}/mimicbait.png",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            {
                Hook hook = new Hook(typeof(SharedDungeonSettings).GetMethod("RandomShouldBecomeMimic", BindingFlags.Instance | BindingFlags.Public), typeof(MimicBait).GetMethod("ChangeMimicChance"));
            }
        };

        public static bool ChangeMimicChance(Func<SharedDungeonSettings, float, bool> orig, SharedDungeonSettings self, float overrideChance)
        {
            for (int i = 0; i < self.MimicPrerequisites.Count; i++)
            {
                if (!self.MimicPrerequisites[i].CheckConditionsFulfilled())
                {
                    return false;
                }
            }
            float num;
            if (overrideChance >= 0f)
            {
                num = overrideChance;
            }
            else
            {
                float num2 = PlayerStats.GetTotalCurse();
                num = self.MimicChance + self.MimicChancePerCurseLevel * num2;
                if (IsFlagSetAtAll(typeof(MimicToothNecklaceItem)))
                {
                    num += 10f;
                }
                if (IsFlagSetAtAll(typeof(MimicBait)))
                {
                    num += 0.33f;
                }
            }
            float value = UnityEngine.Random.value;
            /*ETGModConsole.Log(string.Concat(new object[]
            {
                "mimic ",
                value,
                " | ",
                num
            }));*/
            return value <= num;
        }


        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(GetType());

            if (!m_pickedUpThisRun)
            {
                for (int i = 0; i < StaticReferenceManager.AllChests.Count; i++)
                {
                    Chest chest = StaticReferenceManager.AllChests[i];
                    if (chest && !chest.IsOpen && !chest.IsBroken)
                    {
                        chest.MaybeBecomeMimic();

                        if (chest.IsMimic) chest.ForceUnlock();
                    }
                }
            }
            ETGMod.AIActor.OnPostStart += CoffeeIsTurningGnarly;
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.RemoveFlagsFromPlayer(GetType());
            ETGMod.AIActor.OnPostStart -= CoffeeIsTurningGnarly;
            base.DisableEffect(player);
        }

        public void CoffeeIsTurningGnarly(AIActor actor)
        {
            if (actor == null)
                return;

            if (actor.IsMimicEnemy && ItemHelpers.SynergyActiveAtAll("COFFEE IS TURNING GNARLY"))
            {
                float chance = UnityEngine.Random.value;
                if (chance < synergyJamChance)
                {
                    actor.healthHaver.NextShotKills = true;
                }

                /*ETGModConsole.Log(string.Concat(new object[]
                {
                "gnarly ",
                chance,
                " | ",
                synergyJamChance
                }));*/
            }
        }

        public float synergyJamChance = 0.33f;
    }
}
