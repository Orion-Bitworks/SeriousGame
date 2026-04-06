using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodAnimationController : MonoBehaviour
{
    [SerializeField] BloodAnimationController nextConnection;
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
                alreadyFlowing = true;
                ParticleManager.instance.SpawnParticles("BloodFlowingParticles", transform);
            }
        }
        else if (connection.PairedWithPartner())
        {
            if (nextConnection == null || connectionEventClick.OnUI())
            {
                return;
            }

            nextConnection.CheckBloodFlow();
        }
    }
}
