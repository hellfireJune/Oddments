using Alexandria.ItemAPI;
using JuneLib;
using JuneLib.Items;
using JuneLib.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class HellfireRoundsItem : OddStatusEffectModifierItem
    {
        public static StatusEffectItemTemplate template = new StatusEffectItemTemplate(typeof(HellfireRoundsItem))
        {
            Name = "Hellfire's Rounds",
            Quality = ItemQuality.B,
            SpriteResource = $"{Module.SPRITE_PATH}/hellfirerounds.png",
            Description = "Enough, Enough",
            LongDescription = "All bullets have a chance to make enemies much more volatile. Imbued with the ancient magic of a 6th chamber slug.",
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);
                item.AddToSubShop(ItemBuilder.ShopType.Cursula, 1);
                item.MakeBulletMod();
            },

            EffectToApply = new GenericStatusEffects.GameActorOnDeathEffect(typeof(HellfireExplodeOnDeath))
            {
                TintColor = Color.red,
                AppliesTint = true,
                duration = 6f,
                effectIdentifier = "explosiveHellfire",
                deathType = OnDeathBehavior.DeathType.PreDeath,

                PostApplyAction = onDeath =>
                {
                    HellfireExplodeOnDeath deathExplode = (HellfireExplodeOnDeath)onDeath;
                    deathExplode.explosionData = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
                }
            },
            ProcChance = 0.085f
        };

        public class HellfireExplodeOnDeath : OnDeathBehavior
        {
            public ExplosionData explosionData;

            public override void OnTrigger(Vector2 dirVec)
            {
                if (specRigidbody == null ) { return; }
                Exploder.Explode(specRigidbody.UnitCenter, explosionData, Vector2.zero);
            }
        }
    }
}
