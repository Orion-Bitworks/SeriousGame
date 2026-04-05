using UnityEngine;

public enum OrganType
{
    None,
    Heart,
}

[CreateAssetMenu(menuName = "Organs/OrganData")]
public class OrganData : ScriptableObject
{
    public OrganType organType;
    public GameObject prefab;
    public bool isPlaced = false;
}