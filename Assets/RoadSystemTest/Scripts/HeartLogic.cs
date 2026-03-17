using UnityEngine;

/// <summary>
/// Controla la lógica del corazón, activando y desactivando arterias
/// </summary>
public class HeartLogic : MonoBehaviour
{
    // Referencia a los inputs que generan flujo
    [SerializeField] RoadInput pulmArtLeft, pulmArtRight, aorta;
    // Referencia a los outputs que indican si la sangre ha llegado a cada parte del corazón
    [SerializeField] RoadOutput supVeinCava, infVeinCava, pulmVeinRightDown, pulmVeinRightUp, pulmVeinLeftDown, pulmVeinLeftUp;

    bool pulmonaryActivated = false;    // Indica si la arteria pulmonar ya ha sido activada
    bool aortaActivated = false;        // Indica si la arteria aorta ya ha sido activada

    // Desactivamos los inputs al inicio
    private void Awake()
    {
        pulmArtLeft.StopGenerating();
        pulmArtRight.StopGenerating();
        aorta.StopGenerating();
    }

    /// <summary>
    /// Notifica a los inputs que ya pueden generar flujo
    /// </summary>
    public void NotifyOutputReceivedBall()
    {
        // Si las venas cavas ya reciben flujo y las arterias pulmonares no están en marcha, las encendemos
        if (!pulmonaryActivated && supVeinCava.ballReceived && infVeinCava.ballReceived)
        {
            ActivatePulmonaryArteries();
            pulmonaryActivated = true;
        }

        // Si las arterias pulmonares ya reciben flujo y las arterias pulmonares no están en marcha, las encendemos
        if (!aortaActivated && pulmVeinRightDown.ballReceived && pulmVeinRightUp.ballReceived && pulmVeinLeftDown.ballReceived && pulmVeinLeftUp.ballReceived)
        {
            ActivateAorta();
            aortaActivated = true;
        }
    }

    /// <summary>
    /// Activa las dos arterias pulmonares para que empiecen a generar flujo
    /// </summary>
    void ActivatePulmonaryArteries()
    {
        pulmArtLeft.StartGenerating();
        pulmArtRight.StartGenerating();
    }

    /// <summary>
    /// Desactiva las dos arterias pulmonares
    /// </summary>
    public void DeactivatePulmonaryArteries()
    {
        pulmonaryActivated = false;
        pulmArtLeft.StopGenerating();
        pulmArtRight.StopGenerating();
    }

    /// <summary>
    /// Activa la arteria aorta para que empiece a generar flujo
    /// </summary>
    void ActivateAorta()
    {
        aorta.StartGenerating();
    }

    /// <summary>
    /// Desactiva la arteria aorta
    /// </summary>
    public void DeactivateAorta()
    {
        aortaActivated = false;
        aorta.StopGenerating();
    }
}   
