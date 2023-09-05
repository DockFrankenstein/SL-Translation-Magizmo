using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelCameraRaycaster : GraphicRaycaster
{
    public RectTransform targetPanel;

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        var imageSize = targetPanel.rect.size;

        Vector2 clickPosition = eventData.position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetPanel, clickPosition, null, out Vector2 pos);
        pos += imageSize / 2f;

        var oldPos = eventData.position;
        eventData.position = pos;

        base.Raycast(eventData, resultAppendList);

        eventData.position = oldPos;
    }
}