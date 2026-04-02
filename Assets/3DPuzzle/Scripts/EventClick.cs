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

    [SerializeField] private Transform originalParent;

    private void Start()
    {
        piece = GetComponent<PieceController>();
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
            KillSequence();

            sequence = DOTween.Sequence();
            sequence.Append(piece.transform.DOMove(originalParent.position, 0.5f).SetEase(Ease.InOutBack, 0.5f));
        }
    }

    public void MoveToUI()
    {
        onUI = true;
        piece.UnRegisterConnectionPoints();
        piece.DisableConnectionPoints();

        KillSequence();

        piece.transform.SetParent(originalParent);

        sequence = DOTween.Sequence();
        sequence.Append(piece.transform.DOScale(new Vector3(100f, 100f, 100f), 0.5f));
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
        sequence.Append(piece.transform.DOScale(Vector3.one, 0.5f));
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
}
