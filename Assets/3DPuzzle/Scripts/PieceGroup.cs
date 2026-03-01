using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PieceGroup
{
    [SerializeField] private HashSet<PieceController> pieces = new HashSet<PieceController>();

    private bool canMove = true;

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
        CursorController.instance.ChangeCursorState(CursorController.CURSOR_STATE.ROTATING);

        foreach (var piece in pieces)
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
}
