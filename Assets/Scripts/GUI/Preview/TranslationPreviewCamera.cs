using UnityEngine;
using qASIC.Input;
using Project.Utility.UI;

namespace Project.GUI.Preview
{
    public class TranslationPreviewCamera : MonoBehaviour
    {
        public PanelCamera cam;
        public float scrollSpeed = 0.2f;

        [Label("Input")]
        [SerializeField][MapItemType(MapItemType.InputBinding)] InputMapItemReference i_drag;

        private void LateUpdate()
        {
            ReadInput();
            HandleDrag();
            HandleScroll();
        }

        void ReadInput()
        {
            if (cam.InFocus)
            {
                if (i_drag.GetInputDown())
                    BeginDrag();

                _scroll = Input.mouseScrollDelta.y;
            }

            if (i_drag.GetInputUp())
                EndDrag();
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
            cam.Cam.orthographicSize *= Mathf.Pow(scrollSpeed, -_scroll);
        }
        #endregion
    }
}
