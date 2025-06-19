using Alexandria.ItemAPI;
using Alexandria.DungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using GungeonAPI;
using HarmonyLib;
using JuneLib.Items;
using JuneLib;
using SaveAPI;
using System.Reflection;
using Alexandria.EnemyAPI;

namespace Oddments
{
    [BepInDependency("etgmodding.etg.mtgapi")]
    [BepInDependency("alexandria.etgmod.alexandria")]
    [BepInDependency("blazeykat.etg.junelib")]
    [BepInPlugin(GUID, MOD_NAME, VERSION)]
    public class Module : BaseUnityPlugin
    {
        public const string MOD_NAME = "Oddments";
        // Change this next update so that Modded Bug Fix doesn't try to patch the already fixed code!
        public const string VERSION = "0.0.9";
        public static readonly string TEXT_COLOR = "#ffa944";
        public static readonly string PREFIX = "odmnts";
        public static readonly string ASSEMBLY_NAME = "Oddments";
        public static readonly string SPRITE_PATH = $"{ASSEMBLY_NAME}/Resources/Sprites/Items";
        public static string FilePathFolder;
        public static AssetBundle oddBundle;
        public const string GUID = "blazeykat.etg.oddments";

        public void Start()
        {
            Debug.Log("Made you look");
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager g)
        {
            try
            {


                new Harmony(GUID).PatchAll();
                FilePathFolder = this.FolderPath();
                oddBundle = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("oddments", true);
                //Log($"tuah", TEXT_COLOR);

                AilmentsCore.Init();

                ETGMod.Assets.SetupSpritesFromAssembly(Assembly.GetExecutingAssembly(), $"{ASSEMBLY_NAME}/Resources/Sprites/MTGSprites");


                ItemBuilder.Init();
                SaveAPIManager.Setup("oddments");
                PrefixHandler.AddPrefixForAssembly(PREFIX);
                JuneSaveManagerCore.Init();

                OddSparksDoer.InitPrefabs();
                LightningModifier.Init();

                ItemTemplateManager.Init();
                GunMaker.InitGuns();
                Synergies.Init();
                //EnemiesCore.Init();

                //ShrineFactory.Init();
                //ShrapnelAbilityBase.InitSetupStuffYaddaYadda();
                SynergyChanceModificationItem.InitBase();

                //DerringerShrine.Add();
                //DungeonHandler.debugFlow = true;

                //FloorGenFunnyTests.Init();
                //VoidFieldsTest.Init();
                //FortuneMagic.Init();
                CommandsBox.Init();
                //EnemyTools.ManualAddOB(typeof(TestOverrideEnemyStuff.TestOverrideBehavior));

                string modName = MOD_NAME;
                if (JuneSaveManagerCore.Flipments)
                {
                    var reverse = modName.Reverse();
                    modName = "";
                    foreach (var chr in reverse) {
                        modName += chr;
                    }
                    modName = modName.ToTitleCaseInvariant();
                }
                if (JuneSaveManagerCore.DeluxeEdition)
                {
                    modName += " (Deluxe Edition)";
                }
                string startedSuccessfully = $"{modName} v{VERSION} started successfully.";
                if (JuneSaveManagerCore.InnapropriateLanguage) { startedSuccessfully = $"{modName} v{VERSION} just freakin started successfully!"; }
                Log(startedSuccessfully, TEXT_COLOR);
                Log($"- \"{BraveUtility.RandomElement(SPLASH_TEXT)}\"", TEXT_COLOR);
            }
            catch (Exception e) 
            { 
                ETGModConsole.Log(e.ToString());
                Log($"{MOD_NAME} v{VERSION} failed to start", TEXT_COLOR);
                Log("oops!");
            }
        }
        /* Curr Changelog:
         * Fixed decanter always being active
         * fixed caduceus rounds fucking shit up
         * Bunny fixed stuff and idr what all of it was
         * 
         * Non-included 2/3:
         * Yari Ammolet, Sawblade Ammolet, [DecanterItem], [MassDevolveItem], [ClockhandItem], [QuickFreezeItem], [FullHealOnNewFloorItem],
         * Jet, Potion of Haste-loading, [PopPopItem], [GunUpgradeItem], [moneyh], Stacked Pickups, [DoubleRoomClearItem], [BiggerAmmoDropsItem], [DoubleAFairAmountOfItemDropsItem],
         * [RandomPlayerItemItem], [SpawnFriendlyGrenadeItem], [ArmorGeneratingOnRoomClearItem], [overloader], [splitter], Lead Reliquary, [GrimoireItem], [RandomizedPricesItem],
         * [OmegaCoreItem], Starmetal Shield, [SlinkingBulletsItem], Sales 101, [blank shells], [BuffFromCurrencyItemx2], [MoneyFromActivatingTeleportersItem], 
         * [ItemThatPrintsMoneyItem], [Shelleton Hand], [Shelltan's Touch], [tapeflower], [gene splicer], [inf ammo pickup], [inf ammo pickup replacer]
         * add the bee pack and the thing that shoots multiple guns and then maybe hold off on new items for now
         * 
         * Non-included:
         * [RightfulCurtsy], Majestic Censer, Chrome Splash, [RoomClearOnlyHeals], [BulletPowerDrainItem], [BenthicBloomItem], [BlendedHeartPickup], Eternal Idol, Cellophane, Tractor Beam, Black Cat, [Lemegeton], [FuckyBarrelItem]
         * guns: Spid-AR and King Worm
         * Added 1 new shrine
         * 
         * Entirely Unfinished:
         * [ArrayBulletModifierItem], [VenusianRoundsItem], Sanguine Hook, Rocket Ouroboros, [KnockoutRoundsItem], Whet Stone, Wax Stone, Bullet on the Cob
         * Guns: Unnamed Test Gun and Plasma Pistol
         * Shrapnel Giant (It's so over)
         */

        public static void Log(string text, string color="#FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }

        public static readonly List<string> SPLASH_TEXT = new List<string>()
        {
            "It's O-D-D-M-E-N-T-S!",
            "Also play Something Wicked",
            "Also play Community Redux",
            "Trans Rights!",
            "Trans Wrongs!",
            "As Seen on TV!",
            "The One and Only!",
            "Opens the Door!",
            "Malwareless!",
            "Also play Planetside of Gunymede",
            "Eclipsed by the moon!",
            "All that I ever will do!",
            "Fa-fa-fa, fa-fa-fa-fa-fa-fa far better!",
            "Pull out feathers, tear off cellophane!",
            "Also play Once More Into the Breach",
            "splash text here",
            "The Wild!",
            "The Useless!",
            "The Dead!",
            "The Untameable!",
            "Never Over!",
            "So @#%$ing special!",
            "Hail to the Thief",
            "Burn the Witch!",
            "Welcome to Dark Souls, &%@#$!",
            "Download Prismatism",
            "Something I can't explain!",
            "I climb up the stalk",
            "Minecraft s*x porn",
            "Sparkle on! It's wednesday! Don't forget to be yourself!",
            "Chub dislikes smoke",
            "USE BOMBS WISELY",
            "balls wide",
            $"{ new NullReferenceException()}",
            "I call this one, dissappearing tongue!",
            "We've got everything, everything, that you like!",
            "This is how the other hand lives!",
            "7 new items",
            "Bad to the Bone!",
            "Kneel before the overlord!",
            "The greatest the world has ever seen",
            "Admission free",
            "Keep that dial locked to 66.6 Hellfire!",
            "With yours truly, Radio Rahim",
            "The race is about to begin",
            "Still here, I stayed",
            "...and forty feet remain!",
            "Don't give me that good-do-good bulls#$&",
            "Covered in security",
            "The fire is gone",
            "Red as all hellfire",
            "From now on I will be like this, all of the time",
            "With good thumbnail art now!",
            "I will destroy this place if I have to",
            "Status, what's your transmission?",
            "Say my name...",
            "Yo, waddup",
            "Chicken Jockey!",
            "MORE MOUSE BITES!",
            "Mmmm... glurched up eggs."
        };


        /*
         * "- !" means sprite done
         */

        /*To-Do List:
         * Shield Blanks, Singularity Band,
         * Lil' Bombshee
         * Knockback-er thingy
         * Gills
         * Glass crown (all items turn to glass)
         * Contract from below
         * Boss Dice
         * D7
         * Shop Dice
         * Item Dice
         * Room Dice(?)
         * Portable slot machine
         * Giga-Drill
         * Pot Crown
         * Shield doubler
         * Oddments
         * Dice Dice
         * Bottle of NullReferenceException 
         * MISSINGNO
         * TMTRAINER
         * Oddments
         * Bulletlets (friendly bullet kins which act like bullets sorta)
         * Chaos goops (all goops are randomised)
         * Rules Card
         * Golden Hat
         * Carrion Crawler
         * Table Tech Hell
         * Scrap Box
         * Keeper's Box
         * Twisted Bullets (Enemies have a chance to teleport and take damage on bullet hit)
         * Hellbox (Opening it in hell spawns shit)
         * Hell's Spoils (Bloody crown isaac)
         * Poker Chip (50% chance to double chest payout 50% chance to remove them)
         * Obol (Sewer Token)
         * (Item that doubles hegemoney credit payout)
         * Book of synergies Vol. 1 & Vol. 2 (Adds synergies to more common items like master rounds and starter weapons)
         * Curse (All jammed mode)
         * Mega Coin
         * Ammo Slug (charge -> ammo)
         * Snow Slug (ammo -> charge)
         * Rapidfire (all semiauto, massive fire rate up)
         * Autoclicker (auto reload)
         * Overloader (over reload)
         * Lunarbloom (increased healing, healing -> casings on new floor)
         * Turret Friend
         * Mood Missiles
         * Ring of Missiles
         * Sad Bullets (some sort of debuff)
         * Sticky Bullets (sinus infection)
         * Necromancer's Scroll (familiars debuff enemies)
         * Panic Necklace (firerate up on damage)
         * Sweetheart necklace (bees on fire on damage)
         * Shark Tooth Necklace (enemies take 15% more bonus damage)
         * Obsidian rose (weaker fire immunity, increases the radius of any fire goop spawned)
         * Powderkeg's Head (balrog's head)
         * Dad's wallet
         * Rogue Planet
         * Chaos Heart
         * Yum Heart
         * Shield Generator
         * Book of Chest Tomfoolery (trapped chests)
         * Ring of Gripmaster Friendship 
         * Lens of Alchemy (See all status effects on enemies, minor death mark effect)
         * Crossed Heart (ring of pain)
         * Pyre rounds (acrid rounds but fire goops)
         * Blank Shells (^^ but blank)
         * Fuse crown (more fuses, but chests spawn goodies on splodsion)
         * Take damage after clearing first 5 rooms with item equipped, immense stat up
         * Titan's Armour
         * Titan's Fists
         * Training Dummy
         * Scarecrow's Head - !
         * status effect item that makes enemies drop money on hit
         * Ad Thing Active
         * Random Prices Things
         * 
         * That one gel gun from aperture tag
         * Curseblaster
         * M9 Evil Gun
         * Gun Gun
         * Gun Sugar/Tzu
         * Lights out
         * Heartbreaker's Rifle
         * Heartbreaker's Shotgun
         * Misery Missiles
         * Gloomer
         * Weaver Rifle
         * daily heavy gun
         * bugunlon
         * Chrome AR
         * 
         * Mega Secret Rooms
         * Ultra Secret Rooms/IAMERROR rooms
         * Giant fuckin pots
         * Polymorph goop
         * No-Roll goop
         * No-Roll pots
         * Acceleration goop
         * Bouncy Goop
         * Wishing Well
         * Pickup Chests
         * Backup Chests
         * Love Chests
         * Haunted Chests
         * Hate Chests
         * Crushers
         * Hybrid chests (double keys, choice item)
         * Lottery chests
         * Chest games
         * Backups
         * Trickster Challenge Shrines
         * Promotium Goop
         * Elites
         * Elite Shrine
         * Rift Wyrmling
         * Demon fight rooms
         * Jam Creep
         * Jam Traps
         * Cursed Pylons
         * Damage Rooms
         * Library (master round deposit)
         * Shrine of Heartbreak (lose maxHP on damage sometimes)
         * Pain merchant (buy stuff take damage)
         * Artillery Drones
         * Artillery Shops
         * Death Merchant (buying an item makes you take extreme damage for the next couple rooms)
         * Shop you buy stuff with curse
         * Jammed Casings (think cursed pickups from NT)
         * Wishful Shrine - !
         * Companion room ring of pain
         * 
         * Gooptons extended arsenal: Acrid Rounds
         * Cursula's extended arsenal:
         * Trorc's extended arsenal: Echo Detonator (echo gun from that one NT thing i did but also as some passive thing), SCAR-H (hitscan AR)
         * Old Red's extended arsenal:
         * Flynt's extended arsenal:
         * 
         * Shrapnelgiant's Abilities: Parry, Firedash, Giantshot, that ms. chalice invuln special thing, a regular invuln special thing, radial damage with fire
         * 
         * Graveyard / Gravedigger room
         * Cursed Crystal Caves
         */

        /*The Maybe-do list (things which would only appear with other mods:
         * OMITB:
         * Cursed Shield (has a chance to negate damage and add curses to the current floor instead)
         * Curse Dice
         * Damned Soul
         * Corruption
         * Star of the bottomless pit (Revenant's mask something wicked)
         * Shade's tome (the oddments of OMITB)
         * 
         * Planetside of Gunymede:
         * Blue Skell (the blue key of something wicked)
         * remeber that one thing i pitched to bunny that one time but he didnt want to do because it felt unnecessary? well this mod is unnecessary lets fuckin add it
         * Umbral Candle
         * oddments variant for this one too
         */
    }
}
