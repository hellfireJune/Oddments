using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using Alexandria;

namespace Oddments
{
    public class FortuneCookieItem : PlayerItem
    {
        public static readonly List<string> Fortunes = new List<string>()
        {
            "Amazing chest ahead",
            "All cats are good luck",
            "Help! I'm stuck in a mod!",
            "Don't ignore blue portals",
            "Use blanks wisely",
            "Stay in touch",
            "Try the debug console!",
            "Download Prismatism",
            "Try a different game",
            //"Get baked on meth",
            "Open all chests",
            "Run like hell",
            "Conserve ammo",
            "You'll die to Gull",
            "Ignore the errors",
            "Lucky numbers: 2 76 16",
            "Watch out!",
            "Beware the gundead",
            "Don't look back",
            "You're the villain",
            "Try going Abbey",
            "Try going R&G Dept.",
            "Reach mines, quickly",
            "Some chests deceive",
            "Some doors are trapdoors",
            "Get off my case",
            "Stay on the outside",
            "Dodge the bullets",
            "Don't trust Chuck",
            "We know",
            "Hellfire AOTY",
            "You will play Fiendfolio",
            "Conserve ammo",
            "Hi Turtle",
            "Get the Gun",
            "Some doors aren't doors",
            "Always take the left door",
            "No half measures",
            "No in-betweens",
            "Rolled a 1",
            "Think quick",
            "I'm the victim here",
            "No nothing",
            "No reason not to sin",
            "It's awful",
            ":horse:",
            "You're the left side",
            "MOST CERTAINLY",
            "Every door is a gamble",
            "Bullet hell awaits you",
            "Reap what you sow",
            //"You are gay",
        };

        public static ItemTemplate template = new ItemTemplate(typeof(FortuneCookieItem))
        {
            Name = "Fortune Cookie",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/fortunecookie.png",
            Description = "Your fortune is:",
            LongDescription = "Higher chance for rooms to drop rewards upon clearing them\n\nFilled with absolutely babbling nonsense no-one in their right mind would pay attention to.",
            Cooldown = 3,
            CooldownType = ItemBuilder.CooldownType.PerRoom,
            Quality = ItemQuality.B,
        };

        public override void DoEffect(PlayerController user)
        {
            string fortune = BraveUtility.RandomElement<string>(Fortunes);
            GameUIRoot.Instance.notificationController.DoCustomNotification("Your fortune is:", fortune, sprite.collection, sprite.spriteId);
            base.DoEffect(user);
        }

        private void AddRoomClearChance(RoomHandler arg1, RoomRewardAPI.ValidRoomRewardContents arg2, float arg3)
        {
            arg2.additionalRewardChance -= 0.2f;
        }
        public override void Pickup(PlayerController player)
        {
            RoomRewardAPI.OnRoomRewardDetermineContents += AddRoomClearChance;
            base.Pickup(player);
        }

        public override void OnPreDrop(PlayerController user)
        {
            RoomRewardAPI.OnRoomRewardDetermineContents -= AddRoomClearChance;
            base.OnPreDrop(user);
        }

        public override void OnDestroy()
        {
            if (LastOwner && LastOwner.HasActiveItem(this.PickupObjectId))
            {
                RoomRewardAPI.OnRoomRewardDetermineContents -= AddRoomClearChance;
            }
            base.OnDestroy();
        }
    }
}
