using JuneLib;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    internal class GrimoireItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(GrimoireItem));
        //give minor damage up and 1 curse up

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            ItemsCore.GetItemChanceMult += ChanceMult;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            ItemsCore.GetItemChanceMult -= ChanceMult;
        }

        public static float WeightMult = 3f;

        private void ChanceMult(PickupObject arg1, PlayerController arg2, bool arg3, JuneLibLootEngineModificationAPI.ModifyLuckArgs arg4)
        {
            StatModifier[] mods = null;
            if (arg1 is PassiveItem passive)
            {
                mods = passive.passiveStatModifiers;
            } else if (arg1 is PlayerItem playeritem)
            {
                mods = playeritem.passiveStatModifiers;

            } else if (arg1 is Gun gun)
            {
                mods = gun.passiveStatModifiers;
            }

            if (mods != null && mods.Length > 0)
            {
                foreach (var mod in mods)
                {
                    if (mod != null && mod.statToBoost == PlayerStats.StatType.Curse && mod.amount > 0)
                    {
                        arg4.WeightMult *= WeightMult;
                        return;
                    }
                }
            }
        }
    }
}
