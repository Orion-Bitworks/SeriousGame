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

    private bool dragging = false;
    bool canGrow = false;
    bool canShrink = false;
    bool hovering = false;

    private Vector3 originalScale = new Vector3(1,1,1);
    private Vector3 shrinkScale = new Vector3(0.5f, 0.5f, 0.5f);

    private float timer = 0.2f;
    private float timePassed;

    float canvasAlpha = 1;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
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
        //originalScale = newPiece.transform.localScale;
        newPiece.transform.localScale = originalScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (hovering)
        {
            Destroy(newPiece);
        }
        else
        {
            newPiece.GetComponent<PieceController>().DisableControls();
        }

        dragging = false;
        newPiece = null;
        canShrink = false;
        canGrow = false;
        timePassed = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;

        if (dragging)
        {
            timePassed = 0;
            canShrink = true;
            canGrow = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;

        if (dragging)
        {
            timePassed = 0;
            canGrow = true;
            canShrink = false;
        }
    }
}
