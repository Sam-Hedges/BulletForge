using UnityEngine;

namespace BulletForge
{
    using ScriptableObjects;

    public class BFPattern : MonoBehaviour
    {
        /* Dialogue Scriptable Objects */
        [SerializeField] private BFPatternContainerSO patternContainer;
        [SerializeField] private BFPatternGroupSO patternGroup;
        [SerializeField] private BFPatternSO pattern;

        /* Filters */
        [SerializeField] private bool groupedPattern;
        [SerializeField] private bool startingPatternOnly;

        /* Indexes */
        [SerializeField] private int selectedPatternGroupIndex;
        [SerializeField] private int selectedPatternIndex;
    }
}