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

    private GameObject groupPivot;
    private Dictionary<PieceController, Transform> originalParents = new();

    [SerializeField] private Transform originalParent;
    [SerializeField] private Transform dragParent;

    private bool canInteract = true;

    private void Start()
    {
        piece = GetComponent<PieceController>();
        scaleUI = piece.transform.localScale;
    }

    private void Update()
    {
        if (!beingDragged || piece.IsInteracting() || !canInteract)
        {
            return;
        }

        if (piece.GetGroup().GetPieces().Count > 1)
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
        if (!canInteract)
        {
            return;
        }

        beingDragged = true;

        MoveOutUI();
        piece.EnableControls();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!canInteract)
        {
            return;
        }

        beingDragged = false;

        if (!InputManager.instance.rotateMode_ia.inProgress)
        {
            piece.DisableControls();
            piece.CanSnap(false);
        }

        if (onUI)
        {
            Vector3 prevRotation = new Vector3(270f, 0f, piece.transform.localRotation.z);

            piece.transform.SetParent(originalParent);

            Sequence positionSequence = DOTween.Sequence();
            positionSequence = DOTween.Sequence();
            positionSequence.Append(piece.transform.DOMove(originalParent.position, 0.5f).SetEase(Ease.InOutBack, 0.5f));
            positionSequence.Join(piece.transform.DOLocalRotate(prevRotation, 0.5f).SetEase(Ease.Linear));
        }
    }

    public bool OnUI()
    {
        return onUI;
    }

    public void MoveToUI()
    {
        onUI = true;
        piece.UnRegisterConnectionPoints();
        piece.DisableConnectionPoints();

        KillSequence();

        Transform targetParent;

        if (!beingDragged)
        {
            //piece.transform.SetParent(originalParent, true);
            targetParent = originalParent;
        }
        else
        {
            //piece.transform.SetParent(dragParent, true);
            targetParent = dragParent;
        }

        Plane canvasPlane = new Plane(targetParent.forward, targetParent.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float point;
        Vector3 targetWorldPos = piece.transform.position;

        if (canvasPlane.Raycast(ray, out point))
        {
            targetWorldPos = ray.GetPoint(point);
        }

        piece.transform.SetParent(targetParent, true);

        piece.transform.position = targetWorldPos;

        sequence = DOTween.Sequence();
        sequence.Append(piece.transform.DOScale(scaleUI, 0.5f).SetEase(Ease.OutBack));
        //sequence.Join(piece.transform.DOLocalMoveZ(0, 0.5f));
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
        piece.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        piece.transform.localScale = Vector3.zero;

        Sequence resetSequence = DOTween.Sequence();
        resetSequence.AppendInterval(2f);
        resetSequence.Append(piece.transform.DOScale(scaleUI, 0.5f).SetEase(Ease.OutBack));
    }

    public void CanInteract(bool canInteract)
    {
        this.canInteract = canInteract;
    }
}
