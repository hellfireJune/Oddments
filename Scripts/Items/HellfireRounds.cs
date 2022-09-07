using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using UnityEngine;

namespace Oddments
{
    public class HellfireRounds : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(HellfireRounds))
        {
            Name = "Hellfire's Rounds",
            Quality = ItemQuality.B,
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/hellfirerounds.png",
            Description = "Enough, enough",
            LongDescription = "All bullets have a chance to make enemies much more volatile. Imbued with the ancient magic of a 6th chamber slug.",
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);
                item.AddToSubShop(ItemBuilder.ShopType.Cursula, 1);

                onDeath = new GenericStatusEffects.GameActorOnDeathEffect(typeof(ExplodeOnDeath))
                {
                    TintColor = Color.red,
                    AppliesTint = true,
                    duration = 6f,
                    effectIdentifier = "hellfire",
                    deathType = OnDeathBehavior.DeathType.PreDeath, 

                    PostInitAction = onDeath =>
                    {
                        ExplodeOnDeath deathExplode = (ExplodeOnDeath)onDeath;
                        deathExplode.explosionData = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
                    }
                };
            }
        };

        public static GenericStatusEffects.GameActorOnDeathEffect onDeath;

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += Player_PostProcessProjectile;
            base.Pickup(player);
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            if (UnityEngine.Random.value < procChance)
            {
                arg1.statusEffectsToApply.Add(onDeath);
                arg1.AdjustPlayerProjectileTint(Color.red, 3);
            }
        }

        private float procChance = 0.085f;
    }
}
