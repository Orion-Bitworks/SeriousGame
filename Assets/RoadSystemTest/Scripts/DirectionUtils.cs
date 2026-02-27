using UnityEngine;

/// <summary>
/// Gestiona la lógica de direcciones
/// </summary>
public static class DirectionUtils
{
    /// <summary>
    /// Convierte una dirección lógica en una posición del grid
    /// </summary>
    public static Vector3Int ToVector(RoadDirection dir)
    {
        switch (dir)
        {
            case RoadDirection.Up: return new Vector3Int(0, 0, 1);
            case RoadDirection.Down: return new Vector3Int(0, 0, -1);
            case RoadDirection.Left: return new Vector3Int(-1, 0, 0);
            case RoadDirection.Right: return new Vector3Int(1, 0, 0);
        }
        return Vector3Int.zero;
    }

    /// <summary>
    /// Devuelve la dirección opuesta
    /// </summary>
    public static RoadDirection Opposite(RoadDirection dir)
    {
        return (RoadDirection)(((int)dir + 2) % 4);
    }

    /// <summary>
    /// Rota una dirección 90º en sentido horario
    /// </summary>
    public static RoadDirection Rotate90(RoadDirection dir)
    {
        return (RoadDirection)(((int)dir + 1) % 4);
    }
}
