using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SaveAPI;

namespace Oddments
{
    public class PickupReplacerItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(PickupReplacerItem))
        {
            Name = "Cadueleus",
            Description = "Equalised Healing",
            LongDescription = "All red hearts and armor are replaced with mixed \"Blended Hearts\", which will grant armor if the player has full health, but will overwise only give hearts." +
            "\n\nAn old staff of the god Hermissile, now a symbol of healthcare within the Gungeon.",
            SpriteResource = $"{Module.SPRITE_PATH}/cadueleus.png",
            PostInitAction = item =>
            {
                PickupReplacerItem pickup = (PickupReplacerItem)item;
                pickup.swapFlavor = SwapType.PICKUP_CADUCEUS;

                item.SetupUnlockOnCustomFlag(CustomDungeonFlags.CADUELCEUS_FLAG, true);
                item.AddUnlockText("Find a blended heart, or beat the Abbey boss without taking damage");
            },
            Quality = ItemQuality.A,
        };
        public static OddItemTemplate template2 = new OddItemTemplate(typeof(PickupReplacerItem))
        {
            Name = "humbling bundle",
            PostInitAction = item =>
            {
                PickupReplacerItem pickup = (PickupReplacerItem)item;
                pickup.swapFlavor = SwapType.PICKUP_UPGRADE;
            }
        };
        public static OddItemTemplate template3 = new OddItemTemplate(typeof(PickupReplacerItem))
        {
            Name = "ammo upgrade",
            PostInitAction = item =>
            {
                PickupReplacerItem pickup = (PickupReplacerItem)item;
                pickup.swapFlavor = SwapType.PICKUP_AMMOUPGRADE;
            }
        };
        public static OddItemTemplate template4 = new OddItemTemplate(typeof(PickupReplacerItem))
        {
            Name = "Lead Cross",
            Description = "More Armor",
            SpriteResource = $"{Module.SPRITE_PATH}/leadcross.png",
            LongDescription = "Hearts have a chance to be replaced by armor.",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            {
                PickupReplacerItem pickup = (PickupReplacerItem)item;
                pickup.swapFlavor = SwapType.PICKUP_MOREARMOR;
            }
        };

        public override void Pickup(PlayerController player)
        {
            JuneLib.ItemsCore.AddChangeSpawnItem(ReturnNewPickup);
            base.Pickup(player);
        }
        public override void DisableEffect(PlayerController player)
        {
            JuneLib.ItemsCore.RemoveChangeSpawnItem(ReturnNewPickup);
            base.DisableEffect(player);
        }
        private readonly Dictionary<int, int> m_upgrades = new Dictionary<int, int>() { { GlobalItemIds.FullHeart, OddItemIDs.StackedHeart }, { GlobalItemIds.Key, OddItemIDs.KeyRingPickup }, { GlobalItemIds.Blank, OddItemIDs.StackedBlanks } };

        private GameObject ReturnNewPickup(PickupObject arg)
        {
            int id = -1;
            switch (swapFlavor)
            {
                case SwapType.PICKUP_MOREARMOR:
                    if (arg is HealthPickup health && UnityEngine.Random.value < 0.5f && health.healAmount > 0 && health.armorAmount <= 0)
                    {
                        id = 120;
                    }
                    break;
                case SwapType.PICKUP_CADUCEUS:
                    if (arg is HealthPickup heart && (heart.healAmount > 0.5 || heart.armorAmount > 0))
                    {
                        id = OddItemIDs.BlendedHeart;
                    }
                    break;
                case SwapType.PICKUP_UPGRADE:
                    if (m_upgrades.ContainsKey(arg.PickupObjectId) && UnityEngine.Random.value < 0.5f)
                    {
                        id = m_upgrades[arg.PickupObjectId];
                    }
                    break;
                case SwapType.PICKUP_AMMOUPGRADE:
                    if (arg is AmmoPickup && !Core.DontDropMore)
                    {
                        id = OddItemIDs.BigAmmoCratePickup;
                    }
                    break;
            }

            if (id != -1) { return PickupObjectDatabase.GetById(id).gameObject; }
            return null;
        }

        public enum SwapType
        {
            PICKUP_UPGRADE,
            PICKUP_CADUCEUS, //shoutsouts to isaac mod retribution you played 
            PICKUP_AMMOUPGRADE,
            PICKUP_MOREARMOR
        }
        public SwapType swapFlavor;
    }
}
