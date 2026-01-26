using UnityEngine;

public class BuildAction
{
    public Vector3Int cell;
    public GameObject prefab;
    public Quaternion rotation;
    public RoadDirection[] connections;

    public BuildAction(Vector3Int cell, GameObject prefab, Quaternion rotation, RoadDirection[] connections)
    {
        this.cell = cell;
        this.prefab = prefab;
        this.rotation = rotation;
        this.connections = (RoadDirection[])connections.Clone();
    }
}