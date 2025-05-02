using Alexandria.Misc;
using JuneLib.Items;
using System.Collections.Generic;
using UnityEngine;
using SaveAPI;

namespace Oddments
{
    public class BlendedHeartPickup : HealthPickup
    {
        public static PickupTemplate template = new PickupTemplate(typeof(BlendedHeartPickup))
        {
            Name = "Blended Heart",
            Quality = ItemQuality.COMMON,
            Description = "Hybrid Healing",
            LongDescription = "Will heal two hearts if picked up with empty heart containers, but otherwise will grant 1 armour.",
            SpriteResource = $"{Module.SPRITE_PATH}/Pickups/blendedheart.png",
            CustomCost = 30,
            PostInitAction = item =>
            {
                HealthPickup pickup = (HealthPickup)item;
                pickup.armorAmount = 1;
                OddItemIDs.BlendedHeart = item.PickupObjectId;

                item.PlaceItemInAmmonomiconAfterItemById(GlobalItemIds.Blank);
                item.gameObject.GetOrAddComponent<SpecialPickupObject>().CustomSaveFlagToSetOnAcquisition = CustomDungeonFlags.CADUELCEUS_FLAG;

                var hp = PickupObjectDatabase.GetById(73) as HealthPickup;
                var shield = PickupObjectDatabase.GetById(120) as HealthPickup;
                pickup.healVFX = hp.healVFX;
                pickup.armorVFX = shield.armorVFX;
            },
            
            AutoAddToPools = true,
            ShopPoolWeight = 0.1f,
            RewardPoolWeight = 0.2f,
        };

        public override void Pickup(PlayerController player)
        {
            if (player.healthHaver.GetCurrentHealth() < player.healthHaver.GetMaxHealth())
            {
                this.healAmount = 1f;
                this.armorAmount = 0;
            }
            if (GameStatsManager.Instance.m_encounteredTrackables.ContainsKey(encounterTrackable.EncounterGuid)) {
                EncounterTrackable.SuppressNextNotification = true;
            }
            base.Pickup(player);
        }
    }
}
