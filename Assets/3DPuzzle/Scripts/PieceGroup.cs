using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PieceGroup
{
    [SerializeField] private HashSet<PieceController> pieces = new HashSet<PieceController>();

    private bool canMove = true;

    private bool canPlay = true;

    public PieceGroup()
    {

    }

    public void AddPiece(PieceController piece)
    {
        if (pieces.Contains(piece))
        {
            return;
        }

        pieces.Add(piece);
        piece.SetGroup(this);
    }

    public Vector3 GetCentralPivot()
    {
        Vector3 pivot = Vector3.zero;

        foreach (PieceController piece in pieces)
        {
            pivot += piece.transform.position;
        }

        pivot = pivot / pieces.Count;

        return pivot;
    }

    public void RemovePiece(PieceController piece)
    {
        pieces.Remove(piece);
    }

    public void MergeGroups(PieceGroup otherGroup)
    {
        if (otherGroup == this)
        {
            return;
        }

        foreach (PieceController piece in otherGroup.GetPieces())
        {
            AddPiece(piece);
        }
    }

    public void MovePiece(Vector3 targetMovement, bool b = false)
    {
        if (!canMove)
        {
            return;
        }

        CursorController.instance.ChangeCursorState(CursorController.CURSOR_STATE.MOVING);

        foreach (PieceController piece in pieces)
        {
            if (piece.gameObject.layer != 0)
            {
                piece.gameObject.layer = 0;
            }

            if (!b)
            {
                piece.transform.position += targetMovement;
            }
            else
            {
                piece.transform.position = targetMovement;
            }
        }
    }

    public void RotatePiece(Vector3 pivot, Quaternion rotation)
    {
        foreach (PieceController piece in pieces)
        {
            Vector3 dir = piece.transform.position - pivot;
            dir = rotation * dir;
            piece.transform.position = pivot + dir;
            piece.transform.rotation = rotation * piece.transform.rotation;
        }
    }

    public HashSet<PieceController> GetPieces()
    {
        return pieces;
    }

    public bool CanMove()
    {
        return canMove;
    }

    public void CanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public void ChangeGroupMaterial(Material material, PieceController exception = null)
    {
        foreach (PieceController piece in pieces)
        {
            if (piece != exception)
            {
                piece.GetComponent<MeshRenderer>().material = material;
            }
        }
    }

    public void ChangeGroupLayer(int layer)
    {
        foreach (PieceController piece in pieces)
        {
            piece.gameObject.layer = layer;
        }
    }

    public void ChangeGroupPlacedState(bool isPlaced)
    {
        foreach (PieceController piece in pieces)
        {
            piece.IsPlaced(isPlaced);
        }
    }

    public void PlayFinishAnimation()
    {
        if (!canPlay)
        {
            return;
        }

        canPlay = false;

        GameObject pivot = new GameObject("Group Pivot");
        pivot.transform.position = GetCentralPivot();

        Vector3 combinedForward = Vector3.zero;
        foreach (PieceController piece in pieces)
        {
            combinedForward += piece.transform.forward;
        }
        combinedForward.Normalize();
        pivot.transform.rotation = Quaternion.LookRotation(combinedForward, Vector3.up);

        Dictionary<PieceController, Transform> originalHierarchy = new Dictionary<PieceController, Transform>();

        foreach (PieceController piece in pieces)
        {
            originalHierarchy[piece] = piece.transform.parent;
            piece.transform.SetParent(pivot.transform);
        }

        // Squencia que mueve el grupo arriba y abajo
        Sequence upDownSequence = DOTween.Sequence().SetAutoKill(false);
        upDownSequence.AppendInterval(0.2f);
        upDownSequence.Append(pivot.transform.DOMoveY(0.3f, 0.5f).SetEase(Ease.InBack));
        upDownSequence.AppendInterval(0.1f);
        upDownSequence.Append(pivot.transform.DOMoveY(0f, 0.5f).SetEase(Ease.OutBack));

        // Sequencia que tota el grupo 360º
        Sequence rotationSequence = DOTween.Sequence().SetAutoKill(false);
        rotationSequence.AppendInterval(0.97f);
        rotationSequence.Append(pivot.transform.DOLocalRotate(new Vector3(0f, 360f, 0f), 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic));
        rotationSequence.Join(upDownSequence);

        // Sequencia que rota el grupo y lo coloca en su sitio
        Sequence sequence = DOTween.Sequence().SetAutoKill(false);
        sequence.Append(pivot.transform.DOMove(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.InQuad));

        Vector3 direction = Camera.main.transform.position - pivot.transform.position;
        direction = direction.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion finalRotation = new Quaternion(0, targetRotation.y, 0, 0);

        // Rota el grupo para que mire a camara
        sequence.Join(pivot.transform.DORotateQuaternion(finalRotation, 0.8f).SetEase(Ease.InOutQuad));
        sequence.Join(rotationSequence);        

        sequence.OnComplete(() => 
        {
            foreach (PieceController piece in pieces)
            {
                piece.transform.SetParent(originalHierarchy[piece]);
            }
            GameObject.Destroy(pivot);
            canPlay = true;
        });
    }
}
