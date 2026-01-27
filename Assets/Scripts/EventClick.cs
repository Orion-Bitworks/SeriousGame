using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private PieceController piece;
    private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    private Vector3 originalScale;

    private void Start()
    {
        piece = GetComponent<PieceController>();
        originalScale = transform.localScale;
    }

    // Cuando se pulsa el boton
    public void OnPointerDown(PointerEventData eventData)
    {
        //transform.localScale = originalScale;
        piece.EnableControls();
    }

    // Cuando se suelta el boton
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!InputManager.instance.rotateMode_ia.inProgress)
        {
            piece.DisableControls();
        }
    }

    // Cuando se hace click
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    // Cuando se hace hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        //transform.localScale = hoverScale;
    }

    // Cuando se deja de hacer hover
    public void OnPointerExit(PointerEventData eventData)
    {
        //transform.localScale = originalScale;
    }
}
