using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    [CreateAssetMenu(fileName = "New Panel Target", menuName = "Scriptable Objects/Preview Panel Target")]
    public class PreviewPanelTarget : ScriptableObject
    {
        public RawImage Image { get; set; }
        public bool IsFocused { get; set; }
    }
}