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
            OddmentsSaveFlags.Init();
            UnlockCommands.Init();
        }

        public static Dictionary<int, string> unlocksDict = new Dictionary<int, string>();

        public static void AddUnlockText(this PickupObject self, string text)
        {
            unlocksDict[self.PickupObjectId] = text;
        }
        //all of this is stolen spapi code

        public static DungeonPrerequisite SetupUnlockOnFlag(this PickupObject self, GungeonFlags flag, bool requiredFlagValue)
        {
            if (self.encounterTrackable == null)
            {
                return null;
            }
            return self.encounterTrackable.SetupUnlockOnFlag(flag, requiredFlagValue);
        }

        public static DungeonPrerequisite SetupUnlockOnFlag(this EncounterTrackable self, GungeonFlags flag, bool requiredFlagValue)
        {
            return self.AddPrerequisite(new DungeonPrerequisite
            {
                prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
                saveFlagToCheck = flag,
                requireFlag = requiredFlagValue
            });
        }

        public static DungeonPrerequisite SetupUnlockOnStat(this PickupObject self, TrackedStats stat, float comparisonValue, DungeonPrerequisite.PrerequisiteOperation comparisonOperation)
        {
            if (self.encounterTrackable == null)
            {
                return null;
            }
            return self.encounterTrackable.SetupUnlockOnStat(stat, comparisonValue, comparisonOperation);
        }

        public static DungeonPrerequisite SetupUnlockOnStat(this EncounterTrackable self, TrackedStats stat, float comparisonValue, DungeonPrerequisite.PrerequisiteOperation comparisonOperation)
        {
            return self.AddPrerequisite(new DungeonPrerequisite
            {
                prerequisiteType = DungeonPrerequisite.PrerequisiteType.COMPARISON,
                statToCheck = stat,
                prerequisiteOperation = comparisonOperation,
                comparisonValue = comparisonValue
            });
        }

        public static DungeonPrerequisite SetupUnlockOnMaximum(this PickupObject self, TrackedMaximums maximum, float comparisonValue, DungeonPrerequisite.PrerequisiteOperation comparisonOperation)
        {
            if (self.encounterTrackable == null)
            {
                return null;
            }
            return self.encounterTrackable.SetupUnlockOnMaximum(maximum, comparisonValue, comparisonOperation);
        }

        public static DungeonPrerequisite SetupUnlockOnMaximum(this EncounterTrackable self, TrackedMaximums maximum, float comparisonValue, DungeonPrerequisite.PrerequisiteOperation comparisonOperation)
        {
            return self.AddPrerequisite(new DungeonPrerequisite
            {
                prerequisiteType = DungeonPrerequisite.PrerequisiteType.MAXIMUM_COMPARISON,
                maxToCheck = maximum,
                prerequisiteOperation = comparisonOperation,
                comparisonValue = comparisonValue
            });
        }

        public static DungeonPrerequisite AddPrerequisite(this PickupObject self, DungeonPrerequisite prereq)
        {
            return self.encounterTrackable.AddPrerequisite(prereq);
        }

        public static DungeonPrerequisite AddPrerequisite(this EncounterTrackable self, DungeonPrerequisite prereq)
        {
            if (GameStatsManager.Instance.GetFlag((GungeonFlags)OddmentsSaveFlags.GetFlag(OddFlags.FLAG_ODDMENTS_UNLOCK_ALL)))
                return null;
            if (!string.IsNullOrEmpty(self.ProxyEncounterGuid))
            {
                self.ProxyEncounterGuid = "";
            }
            if (self.prerequisites == null)
            {
                self.prerequisites = new DungeonPrerequisite[] { prereq };
            }
            else
            {
                self.prerequisites = self.prerequisites.Concat(new DungeonPrerequisite[] { prereq }).ToArray();
            }
            EncounterDatabaseEntry databaseEntry = EncounterDatabase.GetEntry(self.EncounterGuid);
            if (!string.IsNullOrEmpty(databaseEntry.ProxyEncounterGuid))
            {
                databaseEntry.ProxyEncounterGuid = "";
            }
            if (databaseEntry.prerequisites == null)
            {
                databaseEntry.prerequisites = new DungeonPrerequisite[] { prereq };
            }
            else
            {
                databaseEntry.prerequisites = databaseEntry.prerequisites.Concat(new DungeonPrerequisite[] { prereq }).ToArray();
            }
            return prereq;
        }
    }
}
