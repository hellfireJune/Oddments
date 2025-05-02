using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.NPCAPI;
using Alexandria.Misc;

namespace Oddments
{
    public class RandomizedPricesItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(RandomizedPricesItem))
        {
            Name = "Dealmakers",
            Description = "[New]!",
            LongDescription = "Randomizes prices for items in shops" + "\n\n",
            SpriteResource = $"{Module.SPRITE_PATH}/dealmaker.png",
            Quality = ItemQuality.C,
            PostInitAction = item =>
            {
                ShopDiscount discount = new ShopDiscount()
                {
                    IdentificationKey = "odmnts:randomized_prices_item",
                    ItemIsValidForDiscount = sitem =>
                    {
                        ItemBeingModified = sitem;
                        return true;
                    },
                    CanDiscountCondition = () =>
                    {
                        return IsFlagSetAtAll(typeof(RandomizedPricesItem));
                    },
                    CustomPriceMultiplier = ReturnFunnyDiscounts,
                };
                CustomDiscountManager.DiscountsToAdd.Add(discount);
            }
        };

        public override void Pickup(PlayerController player)
        {
            player.AddFlagsToPlayer(typeof(RandomizedPricesItem));
            base.Pickup(player);
        }

        public static ShopItemController ItemBeingModified;

        public static float ReturnFunnyDiscounts()
        {
            if (ItemBeingModified == null)
            {
                //this shouldnt happen
                return 1f;
            }
            SpecilDiscountHandler handler = ItemBeingModified.gameObject.GetOrAddComponent<SpecilDiscountHandler>();
            if (handler.overrideCost == null)
            {
                handler.Init();
            }

            return ((float)handler.overrideCost) / ItemBeingModified.CurrentPrice;
        }

        public class SpecilDiscountHandler : BraveBehaviour
        {
            public void Init()
            {
                overrideCost = UnityEngine.Random.Range(10, 75);
            }

            public int? overrideCost = null;
        }
    }
}
