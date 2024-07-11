using UnityEngine;
using qASIC.Input;
using Project.UI;
using qASIC.Options;

namespace Project.GUI.Preview
{
    public class TranslationPreviewCamera : MonoBehaviour
    {
        public PanelCamera cam;
        public float scrollSpeed = 0.2f;

        [SerializeField] float defaultZoom;
        [SerializeField] Vector2 zoomMinMax;

        [Label("Input")]
        [SerializeField][MapItemType(MapItemType.InputBinding)] InputMapItemReference i_drag;

        [Option]
        public static bool UseZoomLimit { get; set; } = true;

        [Option]
        public static float ScrollMultiplier { get; set; } = 1f;

        private void Awake()
        {
            cam.Cam.orthographicSize = defaultZoom;
        }

        private void LateUpdate()
        {
            ReadInput();
            HandleDrag();
            HandleScroll();
        }

        void ReadInput()
        {
            _scroll = 0f;
            if (cam.InFocus)
            {
                if (i_drag.GetInputDown())
                    BeginDrag();

                _scroll = Input.mouseScrollDelta.y * ScrollMultiplier;
            }

            if (i_drag.GetInputUp())
                EndDrag();
        }

        public void ResetPosition()
        {
            transform.position = new Vector3(0f, 0f, transform.position.z);
            cam.Cam.orthographicSize = defaultZoom;
        }

        #region Drag
        bool _drag;
        Vector3 _dragMousePos;

        void BeginDrag()
        {
            _drag = true;
            _dragMousePos = Input.mousePosition;
        }

        void EndDrag()
        {
            _drag = false;
        }

        void HandleDrag()
        {
            if (!_drag) return;

            var newPos = Input.mousePosition;
            var difference = _dragMousePos - newPos;

            transform.position += new Vector3(difference.x * cam.Cam.orthographicSize / Screen.width * 2f * Screen.width / cam.Cam.pixelWidth * cam.Cam.aspect,
                                              difference.y * cam.Cam.orthographicSize / Screen.height * 2f * Screen.height / cam.Cam.pixelHeight);

            _dragMousePos = newPos;
        }
        #endregion

        #region Scroll
        float _scroll;

        void HandleScroll()
        {
            var size = cam.Cam.orthographicSize * Mathf.Pow(scrollSpeed, -_scroll);

            if (UseZoomLimit)
                size = Mathf.Clamp(size, zoomMinMax.x, zoomMinMax.y);

            cam.Cam.orthographicSize = size;
        }
        #endregion
    }
}
