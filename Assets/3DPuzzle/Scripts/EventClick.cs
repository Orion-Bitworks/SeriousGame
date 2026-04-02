using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private PieceController piece;

    private bool onUI = true;

    private void Start()
    {
        piece = GetComponent<PieceController>();
    }

    // Cuando se pulsa el boton
    public void OnPointerDown(PointerEventData eventData)
    {
        MoveOutUI();
        piece.EnableControls();
    }

    // Cuando se suelta el boton
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!InputManager.instance.rotateMode_ia.inProgress)
        {
            piece.DisableControls();
            piece.CanSnap(false);
        }

        //CursorController.instance.ChangeCursorState(CursorController.CURSOR_STATE.DEFAULT);
    }

    // Cuando se hace click
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    // Cuando se hace hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    // Cuando se deja de hacer hover
    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void MoveToUI()
    {
        onUI = true;
        //piece.transform.SetParent(null);
        piece.transform.localScale = Vector3.one;
    }

    public void MoveOutUI()
    {
        onUI = false;
        piece.RegisterConnectionPoints();

        Sequence sequence = DOTween.Sequence().SetAutoKill(false);
        sequence.Append(piece.transform.DOScale(Vector3.one, 0.5f));

        piece.transform.SetParent(null);
        //piece.transform.localScale = Vector3.one;
    }
}
