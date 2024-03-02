using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
        public RawImage TargetRawImage => targetImage.Image;

        public RenderTexture Texture { get; private set; }
        public bool InFocus { get; private set; }

        Vector2 prevSize;

        private void LateUpdate()
        {
            if (prevSize != targetImage.Size)
                ResizeRenderTexture();

            cam.Render();

            CheckForFocus();
        }

        public void ResizeRenderTexture()
        {
            prevSize = targetImage.Size;
            Texture = new RenderTexture(Mathf.Max(1, (int)targetImage.Size.x), Mathf.Max(1, (int)targetImage.Size.y), 16);
            TargetRawImage.texture = Texture;
            cam.targetTexture = Texture;
        }

        private void CheckForFocus()
        {
            var rectTrans = TargetRawImage.rectTransform;
            Vector2 mousePos = rectTrans.InverseTransformPoint(Input.mousePosition);

            var list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition,
            }, list);

            InFocus = targetImage.IsFocused =
                list.Where(x => !(x.module is PanelCameraRaycaster)).FirstOrDefault().gameObject == TargetRawImage.gameObject;
        }
    }
}