using UnityEngine;

[CreateAssetMenu(fileName = "BallMaterials", menuName = "Game/Ball Materials")]
public class BallMaterialsConfig : ScriptableObject
{
    public Material NotO2FromPipe;
    public Material NotO2FromHeart;
    public Material O2FromPipe;
    public Material O2FromHeart;

    public void RegisterAll()
    {
        BallTypeMaterials.RegisterMaterial(BallType.NotO2FromPipe, NotO2FromPipe);
        BallTypeMaterials.RegisterMaterial(BallType.NotO2FromHeart, NotO2FromHeart);
        BallTypeMaterials.RegisterMaterial(BallType.O2FromPipe, O2FromPipe);
        BallTypeMaterials.RegisterMaterial(BallType.O2FromHeart, O2FromHeart);
    }
}