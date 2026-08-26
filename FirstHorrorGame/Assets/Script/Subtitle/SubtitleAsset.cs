using Sirenix.OdinInspector;
using UHFPS.Runtime;
using UnityEngine;

namespace ENZEUN.Runtime
{
    [CreateAssetMenu(fileName = "SubtitleAsset", menuName = "Scriptable Objects/SubtitleAsset")]

    public class SubtitleAsset : ScriptableObject
    {
        [BoxGroup("자막 세팅"), SerializeField]
        private GString subtitleGString;

        [BoxGroup("추적 값"), ReadOnly, ShowInInspector]
        public string localizedSubtitle { get => subtitleGString.GetLocalizedString(); }
    }
}