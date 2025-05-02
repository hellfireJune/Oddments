using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments.Scripts.Items
{
    internal class SupremacyItem : PassiveItem
    {
        public static OddItemTemplate template2 = new OddItemTemplate(typeof(SupremacyItem))
        {
            Name = "Supremacy",
            AltTitle = "Armageddon",
            Quality = ItemQuality.S,
            SpriteResource = $"{Module.SPRITE_PATH}/supremacy.png",
            Description = "This is how it ends!",
            LongDescription = "Call in an air-strike every room! Fire to confirm the air strike, refreshes whenever you clear a room.",
        };

        protected void InitializeAirStrike()
        {
            PickupObject item = PickupObjectDatabase.GetById(252);
            PlayerItem pItem = UnityEngine.Object.Instantiate(item.gameObject).GetComponent<PlayerItem>();
            pItem.m_pickedUpThisRun = false;
            pItem.encounterTrackable.SuppressInInventory = true;
            pItem.encounterTrackable.IgnoreDifferentiator = true;
            pItem.m_pickedUp = true;
            //pItem.Pickup(user);

            pItem.gameObject.AddComponent<JuneLib.Items.FakeRealItemBehaviour>();
            pItem.renderer.enabled = false;
            AirStrike = (DirectionalAttackActiveItem)pItem;
        }
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += Player_PostProcessProjectile;
            player.OnRoomClearEvent +=Player_OnRoomClearEvent;
            if (!AirStrike)
            {
                InitializeAirStrike();
            }
            AirStrike.LastOwner = player;
            if (!m_pickedUpThisRun)
            {
                Player_OnRoomClearEvent(player);
            }
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player)
            {
                player.PostProcessProjectile -= Player_PostProcessProjectile;
                player.OnRoomClearEvent -=Player_OnRoomClearEvent;
            }
            base.DisableEffect(player);
        }

        private void Player_OnRoomClearEvent(PlayerController obj)
        {
            if (AirStrike)
            {
                AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", gameObject);
                AirStrike.Use(obj, out _);
            }
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            if (AirStrike && AirStrike.IsCurrentlyActive)
            {
                AirStrike.DoActiveEffect(Owner);
                AirStrike.CurrentDamageCooldown = 0;
            }
        }

        public override void Update()
        {
            base.Update();
            if (!AirStrike)
            {
                InitializeAirStrike();
            }
        }

        public DirectionalAttackActiveItem AirStrike;
    }
}
