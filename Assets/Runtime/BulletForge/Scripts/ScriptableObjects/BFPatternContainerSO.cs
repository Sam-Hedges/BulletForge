using System.Collections.Generic;
using UnityEngine;

namespace BulletForge.ScriptableObjects
{
    public class BFPatternContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SerializableDictionary<BFPatternGroupSO, List<BFPatternSO>> PatternGroups { get; set; }
        [field: SerializeField] public List<BFPatternSO> UngroupedPatterns { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            PatternGroups = new SerializableDictionary<BFPatternGroupSO, List<BFPatternSO>>();
            UngroupedPatterns = new List<BFPatternSO>();
        }

        public List<string> GetPatternGroupNames()
        {
            List<string> patternGroupNames = new List<string>();

            foreach (BFPatternGroupSO patternGroup in PatternGroups.Keys)
            {
                patternGroupNames.Add(patternGroup.GroupName);
            }

            return patternGroupNames;
        }

        public List<string> GetGroupedPatternNames(BFPatternGroupSO patternGroup, bool startingPatternsOnly)
        {
            List<BFPatternSO> groupedPatterns = PatternGroups[patternGroup];

            List<string> groupedPatternNames = new List<string>();

            foreach (BFPatternSO groupedPattern in groupedPatterns)
            {
                if (startingPatternsOnly && !groupedPattern.IsStartingPattern)
                {
                    continue;
                }

                groupedPatternNames.Add(groupedPattern.PatternName);
            }

            return groupedPatternNames;
        }

        public List<string> GetUngroupedPatternNames(bool startingPatternsOnly)
        {
            List<string> ungroupedPatternNames = new List<string>();

            foreach (BFPatternSO ungroupedPattern in UngroupedPatterns)
            {
                if (startingPatternsOnly && !ungroupedPattern.IsStartingPattern)
                {
                    continue;
                }

                ungroupedPatternNames.Add(ungroupedPattern.PatternName);
            }

            return ungroupedPatternNames;
        }
    }
}