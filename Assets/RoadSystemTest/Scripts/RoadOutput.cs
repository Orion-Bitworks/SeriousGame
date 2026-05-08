using UnityEngine;

/// <summary>
/// Representa un punto de salida del sistema de carreteras
/// </summary>
public class RoadOutput : MonoBehaviour
{
    [SerializeField] RoadDirection inputDirection;                  // Dirección desde la cual pueden llegar las bolitas
    [SerializeField] BallType acceptedBallType;                     // Tipo de bolita que acepta esta salida
    [SerializeField] GameObject enterParticle, destroyParticle;     // Prefabs de particulas de destrucción de la bolita

    [HideInInspector] public bool ballReceived = false;             // Indica si el output ya ha recibido bolitas

    /// <summary>
    /// Lo que ocurre cuando una bolita llega al punto de salida
    /// </summary>
    public void ReceiveBall(MovingBall ball)
    {
        // Añadir suma de puntos?
        if (ball.ballType == acceptedBallType)
        {
            Debug.Log("Bolita recibida en el output!");
            // Aceptada, instanciamos un efecto visual en su posición
            // Instantiate(enterParticle, transform.position, Quaternion.identity);
            ballReceived = true;

            // Notificamos a la lógica del corazón que hemos recibido una bolita si somos parte de él
            var heart = GetComponentInParent<OrganLogic>();
            if (heart != null)
                heart.NotifyOutputReceivedBall();
        }
        else {
            // Rechazada, instanciamos un efecto visual en su posición
            Debug.Log("Bolita Rechazada");
            Instantiate(destroyParticle, transform.position, Quaternion.identity);
        }
    }
}