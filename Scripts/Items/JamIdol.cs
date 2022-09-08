using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class JamIdol : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(JamIdol))
        {
            Name = "Jam Idol",
            Description = "Jammon",
            LongDescription = "Jammed enemies are more plentiful",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/jammedidol.png",
            Quality = ItemQuality.D,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1);
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
