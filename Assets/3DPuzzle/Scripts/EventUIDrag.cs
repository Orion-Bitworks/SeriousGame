using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventUIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject prefabPiece;
    private GameObject newPiece;

    bool hovering = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        newPiece = Instantiate(prefabPiece, transform.position, Quaternion.identity);
        newPiece.GetComponent<PieceController>().EnableControls();
        

        ConnectionPointController[] connections = newPiece.GetComponentsInChildren<ConnectionPointController>();

        foreach (ConnectionPointController point in connections)
        {
            if (!point.CanBeRegistered())
            {
                point.CanBeRegistered(true);
            }
        }

        Destroy(newPiece.GetComponent<InfiniteRotation>());
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!newPiece)
        {
            return;
        }

        if (hovering)
        {
            Destroy(newPiece);
        }
        else
        {
            newPiece.GetComponent<PieceController>().DisableControls();
            newPiece.GetComponent<PieceController>().CanSnap(false);
        }

        CursorController.instance.ChangeCursorState(CursorController.CURSOR_STATE.DEFAULT);

        newPiece = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
