using UnityEngine;
using UnityEngine.EventSystems;

public class MapController : MonoBehaviour
{
    private GlobalLogicController globalLogic;

    void Awake()
    {
        globalLogic = FindObjectOfType<GlobalLogicController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        globalLogic.ClickReceivedFromMap(this);
    }

    /*void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        
    }*/
}
