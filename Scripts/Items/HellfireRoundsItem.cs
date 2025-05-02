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
    public class HellfireRoundsItem : OddStatusEffectModifierItem
    {
        public static StatusEffectItemTemplate template = new StatusEffectItemTemplate(typeof(HellfireRoundsItem))
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
            },
            EffectToApply = AilmentsCore.HellfireEffect,
            ProcChance = 0.085f
        };

        public static StatusEffectItemTemplate template2 = new StatusEffectItemTemplate(typeof(HellfireRoundsItem))
        {
            Name = "Acrid Rounds",
            Description = "Bleck",
            LongDescription = "All bullets have a chance to make enemies much more toxic and leave behind a trail of poisonous goop. Gross.",
            PostInitAction = item =>
            {
                item.MakeBulletMod();
                item.AddToSubShop(ItemBuilder.ShopType.Goopton);
            },
            Quality = ItemQuality.B,
            SpriteResource = $"{Module.SPRITE_PATH}/acridrounds.png",
            EffectToApply = AilmentsCore.AcridRoundsEffect,

            ProcChance = 0.085f
        };

        public static StatusEffectItemTemplate template3 = new StatusEffectItemTemplate(typeof(HellfireRoundsItem))
        {
            Name = "blank rounds",
            EffectToApply = new GameActorOnDeathEffect(typeof(BlankOnDeathWhateverComponent))
            {
                duration = 6f,
                AffectsPlayers = false,
                effectIdentifier = "blanksBlanksBlanks",
                deathType = OnDeathBehavior.DeathType.PreDeath
            }
        };

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
