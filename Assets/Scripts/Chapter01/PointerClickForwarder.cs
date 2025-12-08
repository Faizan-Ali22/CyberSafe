using UnityEngine;
using UnityEngine.EventSystems;
public class PointerClickForwarder : MonoBehaviour
{
     public MonitorTapHotspot hotspot;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (hotspot != null) hotspot.OnTapped();
    }
}
