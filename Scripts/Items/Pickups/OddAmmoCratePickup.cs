using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class OddAmmoCratePickup : PickupObject, IPlayerInteractable
    {
        public static PickupTemplate pickup = new PickupTemplate(typeof(OddAmmoCratePickup))
        {
            Name = "Big Ammo Crate",
            PostInitAction = item =>
            {
                OddItemIDs.BigAmmoCratePickup = item.PickupObjectId;
            },
            CustomCost = 25,
        };

        public static PickupTemplate pickup2 = new PickupTemplate(typeof(OddAmmoCratePickup))
        {
            Name = "Endless Ammo Crate",
            PostInitAction = item =>
            {
                OddItemIDs.InfAmmoPickup = item.PickupObjectId;

                OddAmmoCratePickup oddAmmoCratePickup = item as OddAmmoCratePickup;
                oddAmmoCratePickup.type = OddAmmoType.INFINITE;
            },
            CustomCost = 25,
        };

        public enum OddAmmoType
        {
            BIG,
            INFINITE,
        }
        public OddAmmoType type = OddAmmoType.BIG;

        public float InfAmmoDuration = 30f;

        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            RoomHandler room = player.CurrentRoom;
            if (room == null) { return; }
            m_hasBeenPickedUp = true;
            /*player.CurrentItem.CurrentDamageCooldown = Mathf.Max(player.CurrentItem.CurrentDamageCooldown - 800, 0);
            player.CurrentItem.CurrentRoomCooldown = Mathf.Max(player.CurrentItem.CurrentRoomCooldown - 7, 0);
            AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", base.gameObject);
            if (!GameUIRoot.Instance.BossHealthBarVisible)
            {
                tk2dSpriteCollectionData encounterIconCollection = player.CurrentItem.sprite.Collection;
                G*ameUIRoot.Instance.notificationController.DoCustomNotification("ACTIVE RECHARGED", $"{player.CurrentItem.DisplayName} Recharged", encounterIconCollection, player.CurrentItem.sprite.spriteId);
            }*/
            if (type == OddAmmoType.BIG)
            {
                Core.DontDropMore = true;
                FloorRewardData currentRewardData = GameManager.Instance.RewardManager.CurrentRewardData;
                LootEngine.DoAmmoClipCheck(currentRewardData, out LootEngine.AmmoDropType ammoDropType);
                for (int i = -1; i <= 1; i += 2)
                {
                    int id = ammoDropType == LootEngine.AmmoDropType.DEFAULT_AMMO ? GlobalItemIds.AmmoPickup : GlobalItemIds.SpreadAmmoPickup;
                    GameObject ammo = PickupObjectDatabase.GetById(id).gameObject;
                    LootEngine.SpawnItem(ammo, room.GetBestRewardLocation(new IntVector2(1, 1), specRigidbody.UnitCenter).ToCenterVector3(0f), Vector2.zero, 0f, doDefaultItemPoof: true, disableHeightBoost: true);
                }
                Core.DontDropMore = false;
            } else if (type == OddAmmoType.INFINITE) {
                player.InfiniteAmmo.SetOverride("odmnts_InfAmmoPickup", true, InfAmmoDuration);

                if (!GameUIRoot.Instance.BossHealthBarVisible)
                {
                    GameUIRoot.Instance.notificationController.DoCustomNotification("INFINITE AMMO", $"Infinite ammo for {(int)InfAmmoDuration} seconds", sprite.Collection, sprite.spriteId);
                }
            }
            Destroy(gameObject);
        }

        protected void Start()
        {
            GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(this);
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!sprite)
            {
                return 1000f;
            }
            Bounds bounds = sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.white, 0.1f);
            base.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.black, 0.1f);
            sprite.UpdateZDepth();
        }

        public void Interact(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                RoomHandler.unassignedInteractableObjects.Remove(this);
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, true);
            Pickup(interactor);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private bool m_hasBeenPickedUp;
    }
}
