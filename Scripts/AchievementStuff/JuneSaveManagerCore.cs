using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public static class JuneSaveManagerCore
    {
        public static void Init()
        {
            OddUnlockMethods.Init();
            UnlockCommands.Init();
        }

        public static Dictionary<int, string> unlocksDict = new Dictionary<int, string>();

        public static void AddUnlockText(this PickupObject self, string text)
        {
            unlocksDict[self.PickupObjectId] = text;
        }
    }
}
