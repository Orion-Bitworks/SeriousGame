using UnityEngine;

/// <summary>
/// Representa un punto de salida del sistema de carreteras
/// </summary>
public class RoadOutput : MonoBehaviour
{
    [SerializeField] RoadDirection inputDirection;                  // Direcci�n desde la cual pueden llegar las bolitas
    [SerializeField] BallType acceptedBallType;                     // Tipo de bolita que acepta esta salida
    [SerializeField] GameObject enterParticle, destroyParticle;     // Prefabs de particulas de destrucci�n de la bolita

    [HideInInspector] public bool ballReceived = false;             // Indica si el output ya ha recibido bolitas

    /// <summary>
    /// Lo que ocurre cuando una bolita llega al punto de salida
    /// </summary>
    public void ReceiveBall(MovingBall ball)
    {
        // A�adir suma de puntos?
        if (ball.ballType == acceptedBallType)
        {
            Debug.Log("Bolita recibida en el output!");
            // Aceptada, instanciamos un efecto visual en su posici�n
            //Instantiate(enterParticle, transform.position, Quaternion.identity);
            ParticleManager.instance.SpawnParticles("SteamConeBurst", ball.transform.position, Quaternion.LookRotation(-transform.right, transform.up));
            ballReceived = true;

            // Notificamos a la l�gica del coraz�n que hemos recibido una bolita si somos parte de �l
            var heart = GetComponentInParent<OrganLogic>();
            if (heart != null)
            {

                heart.NotifyOutputReceivedBall();
            }
            else
            {
                AudioController.Instance.PlaySFX(SFX.Pipe, (int)PipeSFX.ParticleInOut);
            }
        }
        else {
            // Rechazada, instanciamos un efecto visual en su posici�n
            Debug.Log("Bolita Rechazada");
            //Instantiate(destroyParticle, transform.position, Quaternion.identity);
            ParticleManager.instance.SpawnParticles("BloodExplosion", transform.position, Quaternion.identity);
            AudioController.Instance.PlaySFX(SFX.ThreeD, (int)ThreeDSFX.Explosion);
        }
    }
}