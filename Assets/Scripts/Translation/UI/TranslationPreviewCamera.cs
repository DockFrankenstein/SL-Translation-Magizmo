using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Project.UI;
using qASIC.Input;

namespace Project.Translation.UI
{
    public class TranslationPreviewCamera : MonoBehaviour
    {
        public PanelCamera cam;

        [Label("Input")]
        [SerializeField][MapItemType(MapItemType.InputBinding)] InputMapItemReference i_drag;

        bool _drag;
        Vector3 _dragMousePos;

        private void LateUpdate()
        {
            ReadInput();
            HandleDrag();
        }

        void ReadInput()
        {
            if (cam.InFocus)
            {
                if (i_drag.GetInputDown())
                    BeginDrag();
            }

            if (i_drag.GetInputUp())
                EndDrag();
        }

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

            transform.position += new Vector3(difference.x * cam.Cam.orthographicSize / Screen.width  * 2f * Screen.width  / cam.Cam.pixelWidth * cam.Cam.aspect, 
                                              difference.y * cam.Cam.orthographicSize / Screen.height * 2f * Screen.height / cam.Cam.pixelHeight);

            _dragMousePos = newPos;
        }
    }
}
