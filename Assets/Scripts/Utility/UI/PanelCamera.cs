using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Utility.UI
{
    public class PanelCamera : MonoBehaviour
    {
        public RawImage targetImage;
        [SerializeField] Camera cam;

        private void Reset()
        {
            cam = GetComponent<Camera>();
        }

        public Camera Cam => cam;
        public RawImage TargetImage => targetImage;

        public RenderTexture Texture { get; private set; }
        public bool InFocus { get; private set; }

        private void Update()
        {
            if (Texture == null ||
                Texture.width != targetImage.rectTransform.rect.width ||
                Texture.height != targetImage.rectTransform.rect.height)
                ResizeRenderTexture();

            cam.Render();

            CheckForFocus();
        }

        public void ResizeRenderTexture()
        {
            var size = targetImage.rectTransform.rect.size;
            Texture = new RenderTexture(Mathf.Max(1, (int)size.x), Mathf.Max(1, (int)size.y), 16);
            targetImage.texture = Texture;
            cam.targetTexture = Texture;
        }

        private void CheckForFocus()
        {
            var rectTrans = targetImage.rectTransform;
            Vector2 mousePos = rectTrans.InverseTransformPoint(Input.mousePosition);
            InFocus = rectTrans.rect.Contains(mousePos);
        }
    }
}