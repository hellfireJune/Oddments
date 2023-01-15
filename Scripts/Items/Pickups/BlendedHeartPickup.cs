using Alexandria.Misc;
using JuneLib.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Oddments
{
    public class BlendedHeartPickup : HealthPickup
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BlendedHeartPickup))
        {
            Name = "Blended Heart",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                HealthPickup pickup = (HealthPickup)item;
                pickup.armorAmount = 1;

                GameObject gameObject = pickup.gameObject;
                SpeculativeRigidbody specRig = gameObject.AddComponent<SpeculativeRigidbody>();
                PixelCollider collide = new PixelCollider
                {
                    IsTrigger = true,
                    ManualWidth = 13,
                    ManualHeight = 16,
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.PlayerBlocker,
                    ManualOffsetX = 0,
                    ManualOffsetY = 0
                };
                specRig.PixelColliders = new List<PixelCollider>
                {
                    collide
                };

                item.RemovePickupFromLootTables();
            }
        };

        public override void Pickup(PlayerController player)
        {
            if (player.healthHaver.GetCurrentHealth() < player.healthHaver.GetMaxHealth())
            {
                this.healAmount = 1f;
                this.armorAmount = 0;
            }
            base.Pickup(player);
        }
    }
}
