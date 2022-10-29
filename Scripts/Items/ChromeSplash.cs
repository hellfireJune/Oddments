using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuneLib;

namespace Oddments
{
    public class ChromeSplash : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ChromeSplash))
        {
            Name = "Chrome Splash",
            Quality = ItemQuality.B,
            Cooldown = 600f,
            CooldownType = ItemBuilder.CooldownType.Damage,
            PostInitAction = _ =>
            {
                ChargeDict.Add(ItemQuality.D, 150);
                ChargeDict.Add(ItemQuality.C, 350);
                ChargeDict.Add(ItemQuality.B, 750);
                ChargeDict.Add(ItemQuality.A, 1250);
                ChargeDict.Add(ItemQuality.S, 0);
            }
        };

        private int LastGun = -1;
        public override void Update()
        {
            base.Update();
            if (LastOwner && LastOwner.CurrentGun != null)
            {
                Gun gun = LastOwner.CurrentGun;
                bool worthChanging = ChargeDict.ContainsKey(gun.quality) && gun.CanBeDropped;
                if ((LastGun == -1 || LastGun != LastOwner.CurrentGun.PickupObjectId)
                    && worthChanging)
                {
                    LastGun = LastOwner.CurrentGun.PickupObjectId;
                    ItemQuality quality = LastOwner.CurrentGun.quality;
                    InitCharge(quality);
                }
            }
        }

        public override void DoEffect(PlayerController user)
        {
            Gun gun = user.CurrentGun;
            if (gun)
            {
                user.inventory.RemoveGunFromInventory(gun);
                Gun newgun = LootEngine.GetItemOfTypeAndQuality<Gun>(RegHelpers.Next(gun.quality), GameManager.Instance.RewardManager.GunsLootTable);
                user.inventory.AddGunToInventory(newgun, true);
                base.DoEffect(user);
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            Gun gun = user.CurrentGun;
            if (gun)
            {
                bool worthChanging = ChargeDict.ContainsKey(gun.quality) && gun.CanBeDropped;
                if (gun.quality == ItemQuality.S
                    && worthChanging)
                {
                    return false;
                }
            }
            else return false;
            return base.CanBeUsed(user);
        }

        public static Dictionary<ItemQuality, float> ChargeDict = new Dictionary<ItemQuality, float>();
        public void InitCharge(ItemQuality quality)
        {
            this.damageCooldown = ChargeDict[quality];
            this.remainingDamageCooldown = ChargeDict[quality];
        }
    }
}
