using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Alexandria.ItemAPI;

namespace Oddments
{
    internal class LeadReliquaryItem : PassiveItem
    {
		public static OddItemTemplate template = new OddItemTemplate(typeof(LeadReliquaryItem))
		{
			Name = "Lead Reliquary",
			//Quality = ItemQuality.A,
			PostInitAction = item =>
			{
				((LeadReliquaryItem)item).AddPassiveStatModifier(PlayerStats.StatType.ReloadSpeed, 0.5f);
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

        [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.RecalculateStatsInternal))]
        [HarmonyPrefix]
        public static void RecalculateArmorInternal(PlayerStats __instance, PlayerController owner)
        {
			float num = 0;
			for (int n = 0; n < owner.ownerlessStatModifiers.Count; n++)
			{
				StatModifier statModifier2 = owner.ownerlessStatModifiers[n];
				if (!statModifier2.hasBeenOwnerlessProcessed && statModifier2.statToBoost == PlayerStats.StatType.Health && statModifier2.amount > 0f)
				{
					num += statModifier2.amount;
				}
			}
			for (int num2 = 0; num2 < owner.passiveItems.Count; num2++)
			{
				PassiveItem passiveItem = owner.passiveItems[num2];
				if (passiveItem.passiveStatModifiers != null && passiveItem.passiveStatModifiers.Length > 0)
				{
					for (int num3 = 0; num3 < passiveItem.passiveStatModifiers.Length; num3++)
					{
						StatModifier statModifier3 = passiveItem.passiveStatModifiers[num3];
						if (!passiveItem.HasBeenStatProcessed && statModifier3.statToBoost == PlayerStats.StatType.Health && statModifier3.amount > 0f)
						{
							num += statModifier3.amount;
						}
					}
				}
				if (passiveItem is BasicStatPickup)
				{
					BasicStatPickup basicStatPickup = passiveItem as BasicStatPickup;
					for (int num4 = 0; num4 < basicStatPickup.modifiers.Count; num4++)
					{
						StatModifier statModifier4 = basicStatPickup.modifiers[num4];
						if (!passiveItem.HasBeenStatProcessed && statModifier4.statToBoost == PlayerStats.StatType.Health && statModifier4.amount > 0f)
						{
							num += statModifier4.amount;
						}
					}
				}
			}
			for (int num9 = 0; num9 < owner.activeItems.Count; num9++)
			{
				PlayerItem playerItem = owner.activeItems[num9];
				if (playerItem.passiveStatModifiers != null && playerItem.passiveStatModifiers.Length > 0)
				{
					for (int num10 = 0; num10 < playerItem.passiveStatModifiers.Length; num10++)
					{
						StatModifier statModifier5 = playerItem.passiveStatModifiers[num10];
						if (!playerItem.HasBeenStatProcessed && statModifier5.statToBoost == PlayerStats.StatType.Health && statModifier5.amount > 0f)
						{
							num += statModifier5.amount;
						}
					}
				}
				StatHolder component = playerItem.GetComponent<StatHolder>();
				if (component && (!component.RequiresPlayerItemActive || playerItem.IsCurrentlyActive))
				{
					for (int num11 = 0; num11 < component.modifiers.Length; num11++)
					{
						StatModifier statModifier6 = component.modifiers[num11];
						if (!playerItem.HasBeenStatProcessed && statModifier6.statToBoost == PlayerStats.StatType.Health && statModifier6.amount > 0f)
						{
							num += statModifier6.amount;
						}
					}
				}
			}

			owner.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/vfx_healing_sparkles_001") as GameObject, Vector3.zero, true, false, false);
			owner.healthHaver.Armor += num;
		}
    }
}
