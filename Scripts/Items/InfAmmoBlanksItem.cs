using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;
using UnityEngine;
using System.Collections;
using Alexandria.ItemAPI;

namespace Oddments
{
    public class InfAmmoBlanksItem : BlankModificationItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(InfAmmoBlanksItem))
        {
            Name = "Blank Ammo",
            Description = "Blanks -> Ammo",
            LongDescription = "Using a blank will grant temporary infinite ammo. The rest of this ammonomicon entry has been left intentionally b-",
            SpriteResource = $"{Module.SPRITE_PATH}/blankammo.png",
            Quality = ItemQuality.A,
            PostInitAction = item =>
            {
                item.AddPassiveStatModifier(PlayerStats.StatType.AdditionalBlanksPerFloor, 1f);
                item.AddToSubShop(ItemBuilder.ShopType.OldRed);
                item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2f);
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.GetComponent<ExtendedPlayerComponent>().OnBlankModificationItemProcessed += MoreAmmoOnBlank;
        }

        private void MoreAmmoOnBlank(PlayerController arg1, SilencerInstance arg2, Vector2 arg3, BlankModificationItem arg4)
        {
            if (arg4 == this)
            {
                //arg1.StartCoroutine(InfAmmoBlankCoroutine(arg1));
                arg1.InfiniteAmmo.SetOverride("odmnts_InfAmmoBlank", true, Duration);
            }
        }

        /*public IEnumerator InfAmmoBlankCoroutine(PlayerController player)
        {
            float elapsed = 0f;
            while (elapsed < Duration)
            {
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            yield break;
        }*/

        public float Duration = 6f;
    }
}
