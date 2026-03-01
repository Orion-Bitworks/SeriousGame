using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceGroupManager : MonoBehaviour
{
    private static HashSet<PieceController> allPieces = new HashSet<PieceController>();

    public static void RegisterPiece(PieceController newPiece)
    {
        allPieces.Add(newPiece);
    }

    public static void RebuildGroups()
    {
        HashSet<PieceController> visitedPieces = new HashSet<PieceController>();

        foreach (PieceController piece in allPieces)
        {
            if (!visitedPieces.Contains(piece))
            {
                PieceGroup newGroup = new PieceGroup();

                Stack<PieceController> piecesStack = new Stack<PieceController>();
                piecesStack.Push(piece);

                while (piecesStack.Count > 0)
                {
                    PieceController currentPiece = piecesStack.Pop();

                    if (!visitedPieces.Contains(currentPiece))
                    {
                        visitedPieces.Add(currentPiece);
                        newGroup.AddPiece(currentPiece);

                        foreach (PieceController connectedPiece in currentPiece.ConnectedPieces())
                        {
                            if (!visitedPieces.Contains(connectedPiece))
                            {
                                piecesStack.Push(connectedPiece);
                            }
                        }
                    }
                }

                if (newGroup.GetPieces().Count > 1)
                {
                    foreach (PieceController pieceInGroup in newGroup.GetPieces())
                    {
                        pieceInGroup.HasSnapped(true);
                    }
                }
            }
        }
    }
}
