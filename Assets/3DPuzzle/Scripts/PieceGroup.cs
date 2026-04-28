using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        Vector3 sceneCenter = new Vector3(0f, 10.2f, 4f);

        Vector3 combinedForward = Vector3.zero; // Combinacion de todos los ejes forward de las piezas
        Vector3 combinedUp = Vector3.zero; // Combinacion de todos los ejes up de las piezas
        foreach (PieceController piece in pieces)
        {
            combinedForward += piece.transform.up; // <======================== UP es el FORWARD real, porque la pieza esta rotada -90º
            combinedUp += piece.transform.forward; // <======================== FORWARD es el UP real, porque la pieza esta rotada -90º
        }
        combinedForward.Normalize();
        combinedUp.Normalize();

        Vector3 combinedRight = Vector3.Cross(combinedUp, combinedForward).normalized; // Right perpendicular a los otros dos
        combinedUp = Vector3.Cross(combinedForward, combinedRight).normalized; // Up ajustado para que quede perpendicular a los otros dos

        pivot.transform.rotation = Quaternion.LookRotation(-combinedForward, combinedUp);

        Dictionary<PieceController, Transform> originalHierarchy = new Dictionary<PieceController, Transform>();

        foreach (PieceController piece in pieces)
        {
            originalHierarchy[piece] = piece.transform.parent;
            piece.transform.SetParent(pivot.transform);
        }

        // Squencia que mueve el grupo arriba y abajo
        Sequence upDownSequence = DOTween.Sequence().SetAutoKill(false);
        upDownSequence.AppendInterval(0.2f);
        upDownSequence.Append(pivot.transform.DOMoveY(sceneCenter.y + 0.3f, 0.5f).SetEase(Ease.InBack));
        upDownSequence.AppendInterval(0.1f);
        upDownSequence.Append(pivot.transform.DOMoveY(sceneCenter.y + -0.3f, 0.5f).SetEase(Ease.OutBack));

        // Sequencia que tota el grupo 360º
        Sequence rotationSequence = DOTween.Sequence().SetAutoKill(false);
        rotationSequence.AppendInterval(0.97f);
        rotationSequence.Append(pivot.transform.DOLocalRotate(new Vector3(pivot.transform.localRotation.x, 360f, 0f), 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic));
        rotationSequence.Join(upDownSequence);

        // Sequencia que rota el grupo y lo coloca en su sitio
        Sequence sequence = DOTween.Sequence().SetAutoKill(false);
        sequence.Append(pivot.transform.DOMove(sceneCenter, 1f).SetEase(Ease.InQuad));

        Vector3 direction = Camera.main.transform.position - pivot.transform.position;
        direction = direction.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion finalRotation = new Quaternion(0, targetRotation.y, 0, 0);

        // Rota el grupo para que mire a camara
        sequence.Join(pivot.transform.DORotateQuaternion(finalRotation, 1f).SetEase(Ease.InOutQuad));
        sequence.Join(rotationSequence);        

        sequence.OnComplete(() => 
        {
            foreach (PieceController piece in pieces)
            {
                piece.transform.SetParent(originalHierarchy[piece]);
            }
            GameObject.Destroy(pivot);
            canPlay = true;

            StartPipeAnimation();
        });
    }

    public void StartPipeAnimation()
    {
        List<AnimationPivotController> animationPivots = new List<AnimationPivotController>();

        foreach (PieceController piece in pieces)
        {
            AnimationPivotController[] pivots = piece.GetComponentsInChildren<AnimationPivotController>();

            foreach (AnimationPivotController pivot in pivots)
            {
                animationPivots.Add(pivot);
            }
        }

        animationPivots.Sort((obj1, obj2) => obj1.GetPriority().CompareTo(obj2.GetPriority()));

        /*foreach (AnimationPivotController pivot in animationPivots)
        {
            pivot.StartAnimation();
        }*/

        for (int i = 0; i < animationPivots.Count; i++)
        {
            float animationDelay = (i + 1) * 0.2f;
            animationPivots[i].StartAnimation(animationDelay);
        }
    }

    public void RetrievePipesAnimation()
    {
        List<AnimationPivotController> animationPivots = new List<AnimationPivotController>();

        foreach (PieceController piece in pieces)
        {
            AnimationPivotController[] pivots = piece.GetComponentsInChildren<AnimationPivotController>();

            foreach (AnimationPivotController pivot in pivots)
            {
                animationPivots.Add(pivot);
            }
        }

        animationPivots.Sort((obj1, obj2) => obj2.GetPriority().CompareTo(obj1.GetPriority()));

        /*foreach (AnimationPivotController pivot in animationPivots)
        {
            pivot.StartAnimation();
        }*/

        for (int i = 0; i < animationPivots.Count; i++)
        {
            float animationDelay = (i + 1) * 0.2f;
            animationPivots[i].StartExitAnimation(animationDelay);
        }
    }
}
