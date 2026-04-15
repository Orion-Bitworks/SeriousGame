using UnityEngine;

[CreateAssetMenu(menuName = "Organs/Level Organ Map")]
public class LevelOrganMap : ScriptableObject
{
    public LevelID level;
    public OrganType[] organs;   // Lista de órganos requeridos en este nivel
}