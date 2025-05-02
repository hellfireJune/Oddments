using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public static class JuneSaveManagerCore
    {
        public static readonly int NUMBER_OF_THE_BEAST = 276167616;
        public static int SeedToSet = -1;

        public static void Personalize()
        {
            int seed = (int)SaveAPI.SaveAPIManager.GetPlayerMaximum(SaveAPI.CustomTrackedMaximums.PERSONALIZATION_SEED);
            UnityEngine.Debug.Log(seed);
            UnityEngine.Debug.Log(SaveAPI.AdvancedGameStatsManager.HasInstance);
            if (seed == 0)
            {
                seed = BraveRandom.GenerationRandomRange(1, NUMBER_OF_THE_BEAST);
                SeedToSet = seed;//SaveAPI.SaveAPIManager.UpdateMaximum(SaveAPI.CustomTrackedMaximums.PERSONALIZATION_SEED, seed);
            }

            PersonalizationRandom = new Random(seed);

            //ulong UserID = Steamworks.SteamUser.GetSteamID().m_SteamID;
            DoPinkBlood = GetRandomBool(7);

            InnapropriateLanguage = GetRandomBool(14);
            Flipments = GetRandomBool(100);
            DeluxeEdition = GetRandomBool(10);
            AltTextA = GetRandomBool(2);
            AltTextB = GetRandomBool(3);
            AltTextC = GetRandomBool(5);
            AltTextD = GetRandomBool(10);

            UnityEngine.Debug.Log(seed);
            UnityEngine.Debug.Log($"For June's eyes only: {DoPinkBlood}, {InnapropriateLanguage}, {Flipments}, {DeluxeEdition}, {AltTextA}, {AltTextB}, {AltTextC}, {AltTextD}");
            Flipments = true; DeluxeEdition = true;
        }

        public static bool GetRandomBool(int num)
        {
            return PersonalizationRandom.Next(num) == 0;
        }

        public static void Init()
        {
            Personalize();

            OddUnlockMethods.Init();
            UnlockCommands.Init();
        }

        public static Dictionary<int, string> unlocksDict = new Dictionary<int, string>();

        public static void AddUnlockText(this PickupObject self, string text)
        {
            unlocksDict[self.PickupObjectId] = text;
        }

        public static System.Random PersonalizationRandom;

        //Personalization Quirks
        public static bool DoPinkBlood;

        public static bool InnapropriateLanguage;
        public static bool Flipments;
        public static bool DeluxeEdition;
        public static bool AltTextA;
        public static bool AltTextB;
        public static bool AltTextC;
        public static bool AltTextD;
    }
}
