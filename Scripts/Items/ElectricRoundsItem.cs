using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class ElectricRoundsItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(ElectricRoundsItem));

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += Player_PostProcessProjectile;
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            arg1.gameObject.AddComponent<LightningModifier>();
        }
    }
}
