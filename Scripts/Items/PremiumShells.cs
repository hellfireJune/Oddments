using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using JuneLib.Items;
using Alexandria.Misc;

namespace Oddments
{
    public class PremiumShells : GoldMoverItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PremiumShells))
        {
            Name = "Premium Shells",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                goldShader = Module.oddBundle.LoadAsset<Shader>("GoldenShader.shader");
                item.RemovePickupFromLootTables();
            }
        };

        public static Shader goldShader;

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.SetOverrideShader(goldShader);
        }
    }
}
