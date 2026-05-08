using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodAnimationController : MonoBehaviour
{
    [SerializeField] BloodAnimationController nextConnection;
    [SerializeField] List<AnimationPivotController> animationPivots = new List<AnimationPivotController>();
    [SerializeField] List<BloodAnimationController> requiredConnections = new List<BloodAnimationController>();
    ConnectionPointController connection;
    EventClick connectionEventClick;

    private bool alreadyFlowing = false;

    private void Start()
    {
        connection = GetComponent<ConnectionPointController>();

        if (nextConnection != null)
        {
            connectionEventClick = nextConnection.GetComponentInParent<EventClick>();
        }
    }

    public void CheckBloodFlow()
    {
        if (connection == null)
        {
            Debug.LogError("No connection point assigned.");
            return;
        }

        if (!connection.PairedWithPartner())
        {
            if (!alreadyFlowing)
            {
                Debug.Log("Entrando en particulas");
                alreadyFlowing = true;
                ParticleManager.instance.SpawnParticles("BloodParticles", transform);
                AudioController.Instance.PlaySFX(SFX.ThreeD, (int)ThreeDSFX.BloodFlow);
            }
        }
        else if (connection.PairedWithPartner())
        {
            if (nextConnection != null && !connectionEventClick.OnUI())
            {
                nextConnection.CheckBloodFlow();
            }
            
            if (animationPivots.Count > 0)
            {
                foreach (BloodAnimationController required in requiredConnections)
                {
                    if (!required.CorrectConnection())
                    {
                        return;
                    }
                }

                foreach (AnimationPivotController pivot in animationPivots)
                {
                    pivot.StartInvertedAnimation();
                }
            }
        }
    }

    public bool CorrectConnection()
    {
        return connection.PairedWithPartner();
    }

    public void AlreadyFlowing(bool state)
    {
        alreadyFlowing = state;
    }
}
