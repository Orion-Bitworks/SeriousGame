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

    private Vector3 originalScale;
    private Vector3 shrinkScale = new Vector3(0.2f, 0.2f, 0.2f);

    private float timer = 0.2f;
    private float timePassed;

    float canvasAlpha = 1;

    private void Update()
    {
        if (timePassed < timer && canGrow)
        {
            newPiece.transform.localScale = Vector3.Lerp(shrinkScale, originalScale, timePassed / timer);
            timePassed += Time.deltaTime;
        }

        if (timePassed < timer && canShrink)
        {
            newPiece.transform.localScale = Vector3.Lerp(originalScale, shrinkScale, timePassed / timer);
            timePassed += Time.deltaTime;
        }

        if (timePassed < timer && hovering && dragging)
        {
            prefabPiece.transform.localScale = Vector3.Lerp(originalScale, shrinkScale, timePassed / timer);
            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(canvasAlpha, 0, timePassed / timer);
            timePassed += Time.deltaTime;
        }

        if (timePassed < timer && !hovering && dragging)
        {
            prefabPiece.transform.localScale = Vector3.Lerp(shrinkScale, originalScale, timePassed / timer);
            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(canvasAlpha, 1, timePassed / timer);
            timePassed += Time.deltaTime;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        newPiece = Instantiate(prefabPiece, transform.position, Quaternion.identity);
        newPiece.GetComponent<PieceController>().EnableControls();
        Destroy(newPiece.GetComponent<InfiniteRotation>());
        originalScale = newPiece.transform.localScale;
        newPiece.transform.localScale = shrinkScale;
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
