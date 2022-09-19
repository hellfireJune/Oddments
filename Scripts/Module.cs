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

namespace Oddments
{
    [BepInDependency("etgmodding.etg.mtgapi")]
    [BepInDependency("alexandria.etgmod.alexandria")]
    [BepInPlugin(GUID, MOD_NAME, VERSION)]
    public class Module : BaseUnityPlugin
    {
        public const string MOD_NAME = "Oddments";
        public const string VERSION = "0.0.4";
        public static readonly string TEXT_COLOR = "#ffa944";
        public static readonly string PREFIX = "odmnts";
        public static readonly string ASSEMBLY_NAME = "Oddments";
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

                ItemBuilder.Init();
                //ShrineFactory.Init();

                GenericStatusEffects.InitCustomEffects();
                EasyGoopDefinitions.DefineDefaultGoops();
                ChallengeHelper.Init();
                //ShrapnelAbilityBase.InitSetupStuffYaddaYadda();
                ItemTemplateManager.Init();
                SynergyChanceModificationItem.InitBase();

                //DerringerShrine.Add();

                //IAmGoingToBreakKeepFloorGen.Init();
                //VoidFieldsTest.Init();
                //CommandsBox.Init();
            }
            catch (Exception e) { ETGModConsole.Log(e.ToString()); }
            Log($"{MOD_NAME} v{VERSION} started susccessfully.", TEXT_COLOR);

            Log($"- \"{BraveUtility.RandomElement(SPLASH_TEXT)}\"", TEXT_COLOR);
        }

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
            "Red as all hellfire"
        };

        /*
         * Curr Changelog:
         * Added Lead Heart, Coupon, Odd Rounds, Golden Magnet, Member Card
         * Siphon item now saves any items siphoned across sessions (through save and quit)
         * Fixed Weaver's Charm and Silk Boots breakin shit
         * Fixed crown of love or crown of war not giving chests collision and also breaking their anims
         * Fixed hellfire rounds apparently not ever resetting its effect upon drop
         * Also fixed hellfire rounds apparently not playing nice with bosses. what a terrible time all around.
         * Hellfire Rounds now has the "bullet_modifier" item tag set up
         * Changed the colour of the console logging stuff, and fixed the version number just being wrong lol
         * 1 new splash text
         * Made you look
         */

        /*
         * "- !" means sprite done
         */

        /*To-Do List:
         * Knockback-er thingy
         * Gills
         * Big moon
         * Blind Chests
         * Dipollets (bullets stick to eachother)
         * Repulsion field
         * Glass crown (all items turn to glass)
         * Contract from below
         * Boss Dice
         * D7
         * Shop Dice
         * Item Dice
         * Room Dice(?)
         * Portable slot machine
         * Pocket chest
         * Giga-Drill
         * Pot Crown
         * Item which you use next to a chest and then it uses the chest but you can reuse the chest after like if you find a chest of a certain quality you can make an item of said quality
         * Item which turns every non chest item into that
         * Diplopia
         * Shield doubler
         * XL (Doubles floor sizes)
         * Oddments
         * Dice Dice
         * Bottle of NullReferenceException 
         * MISSINGNO
         * TMTRAINER
         * Oddments
         * Bulletlets (friendly bullet kins which act like bullets sorta)
         * Lich Ticket
         * Chaos goops (all goops are randomised)
         * Rules Card
         * Golden Hat
         * An Altered World (genesis)
         * Danger $$$ (Get items for free with curse)
         * Stock Market
         * Carrion Crawler
         * Pact of Contrition (Red heart damage has a 50% chance to make you not lose mastery rounds)
         * Table Tech Hell
         * Gelatinous Tables
         * Prismatic Tables
         * Scrap Box
         * Keeper's Box
         * Twisted Bullets (Enemies have a chance to teleport and take damage on bullet hit)
         * Hellbox (Opening it in hell spawns shit)
         * Hell's Spoils (Bloody crown isaac)
         * Poker Chip (50% chance to double chest payout 50% chance to remove them)
         * Obol (Sewer Token)
         * Crazy Diamond
         * Stopwatch
         * (Item that doubles hegemoney credit payout)
         * Book of synergies Vol. 1 & Vol. 2 (Adds synergies to more common items like master rounds and starter weapons)
         * Bifurcated Bullets
         * Curse (All jammed mode)
         * Mega Coin
         * Chimerism
         * Ammo Slug (charge -> ammo)
         * Snow Slug (ammo -> charge)
         * Rapidfire (all semiauto, massive fire rate up)
         * Autoclicker (auto reload)
         * Overloader (over reload)
         * Lunarbloom (increased healing, healing -> casings on new floor)
         * Turret Friend
         * Trap Disabler
         * Supermuzzle
         * Mood Missiles
         * Ring of Missiles
         * Sad Bullets (some sort of debuff)
         * Sticky Bullets (sinus infection)
         * Necromancer's Scroll (familiars debuff enemies)
         * Panic Necklace (firerate up on damage)
         * Sweetheart necklace (bees on fire on damage)
         * Shark Tooth Necklace (enemies take 15% more bonus damage)
         * Obsidian rose (weaker fire immunity, increases the radius of any fire goop spawned)
         * Heart of the Powderkeg (balrog's heart)
         * Powderkeg's Head (balrog's head)
         * Dad's wallet
         * Corruption (stats up, doubles any curse related effects)
         * Rogue Planet
         * Chaos Heart
         * Yum Heart
         * Shield Generator
         * Book of Chest Tomfoolery (trapped chests)
         * Ring of Gripmaster Friendship 
         * Lens of Alchemy (See all status effects on enemies, minor death mark effect)
         * Fairy (ring of pain)
         * Crossed Heart (ring of pain)
         * Acrid rounds (hellfire rounds but goop instead of splodsion)
         * Pyre rounds (^^ but fire goops)
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
