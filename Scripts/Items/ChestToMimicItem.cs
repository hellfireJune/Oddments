using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using JuneLib.Items;

namespace Oddments
{
    public class MimicWhistle : PlayerItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(MimicWhistle))
        {
            Name = "Strange Whistle",
            Quality = ItemQuality.D,
            SpriteResource = $"{Module.SPRITE_PATH}/mimicwhistle.png",
            Description = "Tweet!",
            LongDescription = "Turns chests into lockless chests. How lucky!",
            Cooldown = 300f,
            PostInitAction = item =>
            {
                item.AddToSubShop(ItemBuilder.ShopType.Flynt, 1f);
            }
        };

        public override bool CanBeUsed(PlayerController user)
        {
            if (!user || user.CurrentRoom == null)
            {
                return false;
            }
            IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user);
            if (nearestInteractable != null && nearestInteractable is Chest chest)
            {
                if (chest.IsLocked && !chest.IsLockBroken && chest.GetAbsoluteParentRoom() == user.CurrentRoom && !chest.IsMimic && !chest.lootTable.CompletesSynergy)
                    return base.CanBeUsed(user);
            }
            return false;
        }

        public override void DoEffect(PlayerController user)
        {
            IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user);
            if (nearestInteractable != null && nearestInteractable is Chest chest)
            {
                if (chest.IsLocked && !chest.IsLockBroken && chest.GetAbsoluteParentRoom() == user.CurrentRoom && !chest.IsMimic && !chest.lootTable.CompletesSynergy)
                {
                    chest.overrideMimicChance = 10f;
                    chest.MaybeBecomeMimic();

                    LootEngine.DoDefaultPurplePoof(chest.specRigidbody.UnitCenter);
                    chest.ForceUnlock();
                }
            }
            base.DoEffect(user);

        }
    }
}
