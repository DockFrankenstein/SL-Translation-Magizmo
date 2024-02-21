using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class PanelCamera : MonoBehaviour
    {
        public PreviewPanelTarget targetImage;
        [SerializeField] Camera cam;

        private void Reset()
        {
            cam = GetComponent<Camera>();
        }

        public Camera Cam => cam;
        public RawImage TargetImage => targetImage.Image;

        public RenderTexture Texture { get; private set; }
        public bool InFocus { get; private set; }

        private void Update()
        {
            if (Texture == null ||
                Texture.width != TargetImage.rectTransform.rect.width ||
                Texture.height != TargetImage.rectTransform.rect.height)
                ResizeRenderTexture();

            cam.Render();

            CheckForFocus();
        }

        public void ResizeRenderTexture()
        {
            var size = TargetImage.rectTransform.rect.size;
            Texture = new RenderTexture(Mathf.Max(1, (int)size.x), Mathf.Max(1, (int)size.y), 16);
            TargetImage.texture = Texture;
            cam.targetTexture = Texture;
        }

        private void CheckForFocus()
        {
            var rectTrans = TargetImage.rectTransform;
            Vector2 mousePos = rectTrans.InverseTransformPoint(Input.mousePosition);
            InFocus = rectTrans.rect.Contains(mousePos);
        }
    }
}