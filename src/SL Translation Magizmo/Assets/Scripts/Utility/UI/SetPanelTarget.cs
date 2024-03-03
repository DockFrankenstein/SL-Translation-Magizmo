using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class SetPanelTarget : MonoBehaviour
    {
        [SerializeField] PreviewPanelTarget target;
        [SerializeField] RawImage panel;
        [SerializeField] CanvasScaler scaler;

        private void Awake()
        {
            target.Image = panel;
        }

        private void Update()
        {
            target.ScaleFactor = scaler?.scaleFactor ?? 1f;
            target.Size = panel.rectTransform.rect.size * target.ScaleFactor;
        }
    }
}