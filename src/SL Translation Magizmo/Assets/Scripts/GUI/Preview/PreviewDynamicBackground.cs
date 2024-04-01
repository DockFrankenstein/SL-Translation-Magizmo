using UnityEngine;
using UnityEngine.UI;

namespace Project.GUI.Preview
{
    public class PreviewDynamicBackground : PreviewElement
    {
        [Label("Background")]
        public Image image;
        public Sprite[] backgrounds;

        private void Reset()
        {
            image = GetComponent<Image>();
        }

        public override void ChangeIndex(int newIndex, bool silent = false)
        {
            base.ChangeIndex(newIndex);

            if (backgrounds.Length == 0) return;
            image.sprite = backgrounds[Mathf.Clamp(newIndex, 0, backgrounds.Length - 1)];
        }
    }
}