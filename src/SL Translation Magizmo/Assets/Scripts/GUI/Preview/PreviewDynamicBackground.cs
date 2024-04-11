using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Project.GUI.Preview
{
    public class PreviewDynamicBackground : PreviewElement
    {
        [Label("Background")]
        public Image image;
        [AssetPreview] public Sprite[] backgrounds;

        private void Reset()
        {
            image = GetComponent<Image>();
        }

        protected override void Awake()
        {
            base.Awake();

            if (backgrounds.Length > 0)
                image.sprite = backgrounds[0];
        }

        public override void Reload()
        {
            base.Reload();
        }

        public override void ChangeIndex(int newIndex, bool silent = false)
        {
            base.ChangeIndex(newIndex);

            if (backgrounds.Length == 0) return;
            image.sprite = backgrounds[Mathf.Clamp(newIndex, 0, backgrounds.Length - 1)];
        }
    }
}