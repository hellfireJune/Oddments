using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;
using System.Collections;
using Alexandria.ItemAPI;

namespace Oddments
{
    public class InfBlankAmmosItem : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(InfBlankAmmosItem))
        {
            Name = "Empty Chamber",
            Description = "For Reloading Empty Rounds",
            LongDescription = "Reloading an empty gun will spawn a mini-blank.\n\nEmpty paragraph where silly lore would be",
            SpriteResource = $"{Module.SPRITE_PATH}/emptychamber.png",
            Quality = ItemQuality.A,
            PostInitAction = item =>
            {
                item.AddToSubShop(ItemBuilder.ShopType.OldRed);
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1.5f);
            }
        };

        public override void Pickup(PlayerController player)
        {
            player.OnReloadedGun += SpawnBlank;
            base.Pickup(player);
        }

        private float m_softCooldown = 10f;
        private bool m_onCooldown = false;
        private void SpawnBlank(PlayerController arg1, Gun arg2)
        {
            if (arg2.ClipShotsRemaining == 0)
            {
                if (!m_onCooldown)
                {
                    StartCoroutine(CooldownCoroutine());
                    arg1.DoEasyBlank(arg1.specRigidbody.UnitCenter, EasyBlankType.MINI);
                }
            }
        }

        public IEnumerator CooldownCoroutine()
        {
            m_onCooldown = true;
            float elapsed = 0f;
            while (elapsed < m_softCooldown)
            {
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            m_onCooldown = false;
            yield break;
        }
    }
}
