using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Project.UI
{
    public class PanelCameraRaycaster : GraphicRaycaster
    {
        public PreviewPanelTarget targetPanel;

        public override int sortOrderPriority => targetPanel.IsFocused ? 1 : -1;

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            var imageSize = targetPanel.Image.rectTransform.rect.size;

            var clickPosition = eventData.position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetPanel.Image.rectTransform, clickPosition, null, out var pos);
            pos += imageSize / 2f;

            var oldPos = eventData.position;
            eventData.position = pos;

            base.Raycast(eventData, resultAppendList);

            eventData.position = oldPos;
        }
    }
}