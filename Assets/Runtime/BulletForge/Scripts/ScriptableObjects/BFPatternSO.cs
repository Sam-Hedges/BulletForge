using System.Collections.Generic;
using UnityEngine;

namespace BulletForge.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class BFPatternSO : ScriptableObject
    {
        [field: SerializeField] public string PatternName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<BFPatternData> Pattern { get; set; }
        [field: SerializeField] public EPatternType PatternType { get; set; }
        [field: SerializeField] public bool IsStartingPattern { get; set; }

        public void Initialize(string dialogueName, string text, List<BFPatternData> choices, EPatternType patternType, bool isStartingDialogue)
        {
            PatternName = dialogueName;
            Text = text;
            Pattern = choices;
            PatternType = patternType;
            IsStartingPattern = isStartingDialogue;
        }
    }
}