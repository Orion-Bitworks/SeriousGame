using System.IO.Pipes;
using UnityEngine;

public enum RoadDirection
{
    Up,
    Right,
    Down,
    Left
}

public class RoadPiece : MonoBehaviour
{
    public Vector3Int gridPos;
    public RoadDirection[] connections;
}
