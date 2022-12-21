using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualScreen : GraphicRaycaster
{
    public GraphicRaycaster graphicCaster; // Reference to the GraphicRaycaster of the canvas displayed on the virtual screen

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        eventData.position = PlayerController.GetGameCameraMousePosition();
        graphicCaster.Raycast(eventData, resultAppendList);
        
    }

}
