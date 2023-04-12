using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class JamIdol : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(JamIdol))
        {
            Name = "Jam Idol",
            Description = "Jammon",
            LongDescription = "Jammed enemies are more plentiful",
            SpriteResource = $"{Module.SPRITE_PATH}/jammedidol.png",
            Quality = ItemQuality.D,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1);
                item.AddToSubShop(ItemBuilder.ShopType.Cursula);
            }
        };

        public void BlackPhantomnessAdder(AIActor actor)
        {
            if (actor == null)
                return;

            float randomValue = UnityEngine.Random.value;
            if (randomValue < m_chance)
            {
                actor.BecomeBlackPhantom();
            }
        }

        protected float m_chance = 0.2f;

        public override void Pickup(PlayerController player)
        {
            ETGMod.AIActor.OnBlackPhantomnessCheck += BlackPhantomnessAdder;
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            ETGMod.AIActor.OnBlackPhantomnessCheck -= BlackPhantomnessAdder;
            base.DisableEffect(player);
        }
    }
}
