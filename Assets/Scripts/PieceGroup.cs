using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PieceGroup
{
    [SerializeField] private HashSet<PieceController> pieces = new HashSet<PieceController>();

    private Material material;
    private Material clickMaterial;

    private bool canMove = true;

    private Vector3 lastParentPosition;
    private Quaternion lastParentRotation;
    private PieceController parent;

    public PieceGroup(/*Material originalMaterial, Material clickMaterial*/)
    {
        //this.material = originalMaterial;
        //this.clickMaterial = clickMaterial;
    }

    public void SetParent(PieceController piece)
    {
        parent = piece;
        lastParentPosition = piece.transform.position;
        lastParentRotation = piece.transform.rotation;
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

    public void TransformWithParent()
    {
        if (parent == null) return;

        Vector3 posDelta = parent.transform.position - lastParentPosition;
        Quaternion rotDelta = parent.transform.rotation * Quaternion.Inverse(lastParentRotation);

        Vector3 pivot = lastParentPosition;

        foreach (PieceController piece in pieces)
        {
            if (piece != parent)
            {
                Vector3 dir = piece.transform.position - pivot;
                dir = rotDelta * dir;

                piece.transform.position = pivot + dir + posDelta;
                piece.transform.rotation = rotDelta * piece.transform.rotation;
            }
        }

        lastParentPosition = parent.transform.position;
        lastParentRotation = parent.transform.rotation;
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
}
