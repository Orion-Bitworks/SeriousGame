using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private PieceController piece;

    private bool onUI = true;

    private bool beingDragged = false;

    private Sequence sequence;

    private Vector3 scaleUI;

    [SerializeField] private Transform originalParent;
    [SerializeField] private Transform dragParent;

    private void Start()
    {
        piece = GetComponent<PieceController>();
        scaleUI = piece.transform.localScale;
    }

    private void Update()
    {
        if (!beingDragged)
        {
            return;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        bool isOverUI = IsPointerOverWorldUI(eventData);

        if (isOverUI && !onUI)
        {
            MoveToUI();
        }
        else if (!isOverUI && onUI)
        {
            MoveOutUI();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        beingDragged = true;

        MoveOutUI();
        piece.EnableControls();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        beingDragged = false;

        if (!InputManager.instance.rotateMode_ia.inProgress)
        {
            piece.DisableControls();
            piece.CanSnap(false);
        }

        if (onUI)
        {
            piece.transform.SetParent(originalParent);

            Sequence positionSequence = DOTween.Sequence();
            positionSequence = DOTween.Sequence();
            positionSequence.Append(piece.transform.DOMove(originalParent.position, 0.5f).SetEase(Ease.InOutBack, 0.5f));
        }
    }

    public void MoveToUI()
    {
        onUI = true;
        piece.UnRegisterConnectionPoints();
        piece.DisableConnectionPoints();

        KillSequence();

        if (!beingDragged)
        {
            piece.transform.SetParent(originalParent);
        }
        else
        {
            piece.transform.SetParent(dragParent);
        }

        sequence = DOTween.Sequence();
        sequence.Append(piece.transform.DOScale(scaleUI, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(piece.transform.DOMoveZ(originalParent.transform.position.z, 0.5f));
    }

    public void MoveOutUI()
    {
        onUI = false;
        piece.RegisterConnectionPoints();
        piece.EnableConnectionPoints();

        KillSequence();

        piece.transform.SetParent(null);

        sequence = DOTween.Sequence();
        sequence.Append(piece.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
    }

    bool IsPointerOverWorldUI(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            if (r.gameObject.CompareTag("WorldUI"))
            {
                return true;
            }
        }

        return false;
    }

    public void KillSequence()
    {
        if (sequence != null)
        {
            sequence.Kill();
        }
    }

    public void ResetPiece()
    {
        beingDragged = false;

        onUI = true;
        piece.DisableControls();

        piece.UnRegisterConnectionPoints();
        piece.DisableConnectionPoints();

        piece.transform.SetParent(originalParent);
        piece.transform.position = originalParent.position;
        piece.transform.rotation = Quaternion.identity;
        piece.transform.localScale = Vector3.zero;

        Sequence resetSequence = DOTween.Sequence();
        resetSequence.AppendInterval(2f);
        resetSequence.Append(piece.transform.DOScale(scaleUI, 0.5f).SetEase(Ease.OutBack));
    }
}
