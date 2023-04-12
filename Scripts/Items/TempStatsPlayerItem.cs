using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class TempStatsPlayerItem : PlayerItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(TempStatsPlayerItem))
        {
            Name = "Jet",
            PostInitAction = item =>
            {
                TempStatsPlayerItem jet = (TempStatsPlayerItem)item;
                jet.stats = new StatModifier[]
                {
                    new StatModifier() { statToBoost = PlayerStats.StatType.ReloadSpeed, amount = 0.25f, modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE },
                    new StatModifier() { statToBoost = PlayerStats.StatType.RateOfFire, amount = 1.5f, modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE },
                };
                jet.IsJet = true;
                jet.duration = 8f;
            }
        };
        public static OddItemTemplate potionTemplate = new OddItemTemplate(typeof(TempStatsPlayerItem))
        {
            Name = "Potion of Haste-loading",
            PostInitAction = item =>
            {
                var poti = (TempStatsPlayerItem)item;
                poti.stats = new StatModifier[]
                {
                    new StatModifier() { statToBoost = PlayerStats.StatType.ReloadSpeed, amount = -999f},
                    new StatModifier() { statToBoost = PlayerStats.StatType.RateOfFire, amount = 0.1f },
                };
                poti.duration = 14f;
            }
        };
        public static OddItemTemplate moneyTemplate = new OddItemTemplate(typeof(TempStatsPlayerItem))
        {
            Name = "moneyh",
            PostInitAction = item =>
            {
                var poti = (TempStatsPlayerItem)item;
                poti.stats = new StatModifier[]
                {
                    new StatModifier() { statToBoost = PlayerStats.StatType.ExtremeShadowBulletChance, amount = 35f},
                };
                poti.duration = 10f;
            }
        };

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            foreach (StatModifier modifier in stats)
            {
                this.RemoveStat(modifier.statToBoost);
            }
            passiveStatModifiers = passiveStatModifiers.Concat(stats).ToArray();
            user.stats.RecalculateStats(user);

            StartCoroutine(ItemBuilder.HandleDuration(this, duration, user, player =>
            {
                foreach (StatModifier modifier in stats)
                {
                    this.RemoveStat(modifier.statToBoost);
                }
                user.stats.RecalculateStats(user);

                if (IsJet)
                {
                    user.healthHaver.ApplyDamage(1f, Vector2.zero, "Jet");
                }
            }));
        }

        public StatModifier[] stats;
        public float duration;
        public bool IsJet = false;
    }
}
