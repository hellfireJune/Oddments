using Alexandria.ItemAPI;
using JuneLib;
using JuneLib.Items;
using JuneLib.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.Misc;

namespace Oddments
{


    public class OnDeathEffectModifierItem : OddProjectileModifierItem
    {
        public int Slot;
        public List<GameActorOnDeathEffect> Effects = new List<GameActorOnDeathEffect>()
        {
             AilmentsCore.HellfireEffect,
             AilmentsCore.AcridRoundsEffect,
             new GameActorOnDeathEffect(typeof(BlankOnDeathWhateverComponent))
             {
                    duration = 6f,
                    AffectsPlayers = false,
                    effectIdentifier = "blanksBlanksBlanks",
                    deathType = OnDeathBehavior.DeathType.PreDeath
             },

        };

        public static BulletModifierItemTemplate template = new BulletModifierItemTemplate(typeof(OnDeathEffectModifierItem))
        {
            Name = "Hellfire's Rounds",
            AltTitle = "Shellfire",
            Quality = ItemQuality.B,
            SpriteResource = $"{Module.SPRITE_PATH}/hellfirerounds2.png",
            Description = "Enough, Enough",
            AltDesc = "There's always something",
            LongDescription = "All bullets have a chance to make enemies much more volatile. Imbued with the ancient magic of a 6th chamber slug.",
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f);
                item.AddToSubShop(ItemBuilder.ShopType.Cursula, 1);
                item.MakeBulletMod();
                (item as OnDeathEffectModifierItem).Slot = 0;
                //(item as OnDeathEffectModifierItem).EffectToApply = AilmentsCore.HellfireEffect;
            },
            ProcChance = 0.085f
        };

        public static BulletModifierItemTemplate template2 = new BulletModifierItemTemplate(typeof(OnDeathEffectModifierItem))
        {
            Name = "Acrid Rounds",
            Description = "Bleck",
            LongDescription = "All bullets have a chance to make enemies much more toxic and leave behind a trail of poisonous goop. Gross.",
            PostInitAction = item =>
            {
                item.MakeBulletMod();
                item.AddToSubShop(ItemBuilder.ShopType.Goopton);
                (item as OnDeathEffectModifierItem).Slot = 1;
            },
            Quality = ItemQuality.B,
            SpriteResource = $"{Module.SPRITE_PATH}/acridrounds.png",
            ProcChance = 0.085f
        };

        public static BulletModifierItemTemplate template3 = new BulletModifierItemTemplate(typeof(OnDeathEffectModifierItem))
        {
            Name = "blank rounds",
            PostInitAction = item =>
            {
                (item as OnDeathEffectModifierItem).Slot = 2;
                //(item as OnDeathEffectModifierItem).EffectToApply = 
            },
        };

        public override bool ApplyBulletEffect(Projectile arg1)
        {
            arg1.statusEffectsToApply.Add(Effects[Slot]);
            return true;
        }

        public class HellfireExplodeOnDeath : OnDeathBehavior
        {
            public ExplosionData explosionData;

            public override void OnTrigger(Vector2 dirVec)
            {
                if (specRigidbody == null) { return; }
                Exploder.Explode(specRigidbody.UnitCenter, explosionData, Vector2.zero);
            }
        }

        public class BlankOnDeathWhateverComponent : OnDeathBehavior
        {
            public override void OnTrigger(Vector2 dirVec)
            {
                if (specRigidbody == null) { return; }
                PlayerUtility.DoEasyBlank(GameManager.Instance.AllPlayers.First(), specRigidbody.UnitCenter, EasyBlankType.FULL);
            }
        }
    }
}
