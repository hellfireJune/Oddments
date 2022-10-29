using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;
using Alexandria.ItemAPI;

namespace Oddments
{
    public enum OddFlags
    {
        FLAG_ODDMENTS_UNLOCK_ALL,
        FLAG_SHITTY_WIN,
    }
    public static class OddmentsSaveFlags
    {
        public static GungeonFlags GetFlag(OddFlags flag)
        {
            return flags[flag.ToString()];
        }
        private static readonly List<string> m_unprocessedFlags = new List<string>()
        {
            "FLAG_ODDMENTS_UNLOCK_ALL",

            "FLAG_SHITTY_WIN"
        };
        public static Dictionary<string, GungeonFlags> flags = new Dictionary<string, GungeonFlags>();
        public static void Init()
        {
            foreach (string flag in m_unprocessedFlags)
            {
                GungeonFlags fg = AddFlag(flag);
                flags.Add(flag, fg);
            }
        }

        public static GungeonFlags AddFlag(string name)
        {
            return ETGModCompatibility.ExtendEnum<GungeonFlags>(Module.GUID, name);
        }
    }
}
