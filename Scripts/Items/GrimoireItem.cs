using Alexandria.ItemAPI;
using Alexandria.StatAPI;
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
        public static OddItemTemplate template = new OddItemTemplate(typeof(GrimoireItem))
        {
            Name = "Grimoire",
            AltTitle = "Ancient Grimoire",
            SpriteResource = $"{Module.SPRITE_PATH}/grimoire.png",
            Description = "Ancient Evil",
            LongDescription = "Increases your rate of fire, but you're more likely to find cursed items.\n\nThe creator banished this away to never see the light of the Gungeon again, but here it remains.",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.RateOfFire, 0.25f, StatModifier.ModifyMethod.ADDITIVE);
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
            }
        };
        //give minor firerate up and 1 curse up

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

        public static float WeightMult = 5f;

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
                    if (StatAPIManager.IsNegativeCurse(mod))
                    {
                        arg4.WeightMult *= WeightMult;
                        return;
                    }
                }
            }
        }
    }
}
