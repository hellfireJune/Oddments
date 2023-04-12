using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using UnityEngine;
using System.Collections;

namespace Oddments
{
    public class RandomBonusActiveUseItem : PassiveItem
    {
        /* Chaff Grenade, Proximity Mine, C4, Molotov, iBomb, Bullet Time, Aged Bell, Singularity, Defcoy, Shadow Clone, Explosive Decoy, Melted Rock, Smoke Bomb, Box, Fortune's Favor, Jar of Bees, Potion of Lead Skin, 
         * Double vision, Relodestone, Poison Vial, Potion of Gun Friendship, Portable Turret, Knife Shield, Grappling Hook, Stuffed Star, Boomerang, Portable Table Device, Daruma, Partially Eaten Cheese, Ring of Ethereal Form, Ticket, 
         * Magazine Rack, Charm Horn, Sense of Direction, Iron coin, Coolant, Elder Blank,
        Air Strike?, Napalm Strike?, Big Boy?*/
        private static readonly List<int> validFakeItems = new List<int>() { 108, 109, 439, 525 };
        public static OddItemTemplate template = new OddItemTemplate(typeof(RandomBonusActiveUseItem))
        {
            PostInitAction = item =>
            {
                foreach (int i in validFakeItems)
                {
                    PickupObjectDatabase.GetById(i).SetTag("useable_if_fake_item");
                }
            }
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnUsedPlayerItem += UsedItem;
        }

        public override void DisableEffect(PlayerController player)
        {
            player.OnUsedPlayerItem -= UsedItem;
            base.DisableEffect(player);
        }

        private void UsedItem(PlayerController arg1, PlayerItem arg2)
        {
            if (arg1 == null || arg2 == null || IsOnCooldown) { return; }
            if (UseRandomOtherItem(arg1))
            {
                StartCoroutine(Cooldown());
            }
        }
        public static bool UseRandomOtherItem(PlayerController arg1)
        {
            List<PickupObject> items = AlexandriaTags.GetAllItemsWithTag("useable_if_fake_item");
            items = BraveUtility.Shuffle(items);

            foreach (PickupObject item in items)
            {
                if (item && item is PlayerItem pitem
                    && RealFakeItemHelper.UseFakeItem(arg1, pitem))
                {
                    arg1.BloopItemAboveHead(pitem.sprite);
                    return true;
                }
            }
            return false;
        }

        public bool IsOnCooldown;

        public IEnumerator Cooldown()
        {
            IsOnCooldown = true;
            yield return new WaitForSeconds(10f);
            IsOnCooldown = false;
            yield break;
        }
    }

    public class RandomPlayerItemItem : PlayerItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(RandomPlayerItemItem));

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            RandomBonusActiveUseItem.UseRandomOtherItem(user);
        }
    }
}
