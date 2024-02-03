using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Project.Utility.UI
{
    public class PanelCameraRaycaster : GraphicRaycaster
    {
        public RectTransform targetPanel;

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            var imageSize = targetPanel.rect.size;

            var clickPosition = eventData.position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetPanel, clickPosition, null, out var pos);
            pos += imageSize / 2f;

            var oldPos = eventData.position;
            eventData.position = pos;

            base.Raycast(eventData, resultAppendList);

            eventData.position = oldPos;
        }
    }
}