using UnityEngine;
using TMPro;

namespace Project.GUI.Preview
{
    public class VersionElement : PreviewElement
    {
        [Label("Text")]
        [SerializeField] TMP_Text text;

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        public override void Reload()
        {
            text.text = manager.CurrentVersion.version.ToString();
        }
    }
}