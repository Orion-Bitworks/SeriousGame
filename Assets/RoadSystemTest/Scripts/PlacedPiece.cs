using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utilizado para recordar de qué prefab salió esta pieza (para saber que pieza era con los Undo/Redo)
/// </summary>
public class PlacedPiece : MonoBehaviour
{
    [SerializeField] public GameObject originalPrefab;   // Referencia al prefab original del que proviene esta pieza
}