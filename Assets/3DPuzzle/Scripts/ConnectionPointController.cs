using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ConnectionPointController : MonoBehaviour
{
    [SerializeField] private string id = "";
    [SerializeField] private string partnerId = "Undefined";
    [SerializeField] private LayerMask layerToDetect;
    [SerializeField] private bool canBeRegistered = false;

    private bool pairedWithPartner = false;
    private bool isEnabled = false;
    PieceController piece;

    [SerializeField] private bool paired = false;
    [SerializeField] private string pairId = "";

    private void Start()
    {
        piece = GetComponentInParent<PieceController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isEnabled)
        {
            return;
        }

        if (other.gameObject.tag == "ConnectionPoint")
        {
            ConnectionPointController otherPoint = other.GetComponent<ConnectionPointController>();

            if (!paired && !otherPoint.Paired() && otherPoint.piece.IsPlaced() && otherPoint.IsEnabled()) 
            {
                ParticleManager.instance.SpawnParticles("SnappingParticles", transform);

                piece.SnapToPoint(this, other.transform, other.transform.parent);

                pairId = otherPoint.GetId();
                otherPoint.SetPairId(id);

                CheckPairing(otherPoint);
                otherPoint.CheckPairing(this);

                paired = true;
                otherPoint.Paired(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isEnabled)
        {
            return;
        }

        ConnectionPointController otherPoint = other.GetComponent<ConnectionPointController>();

        if (other.gameObject.tag == "ConnectionPoint" && paired && (otherPoint.GetId() == pairId)/* && otherPoint.IsEnabled()*/)
        {
            ResetValues();
            //piece.DisconnectPiece(other.GetComponent<PieceController>());
            //CheckPairing(other.GetComponent<ConnectionPointController>());
        }
    }

    private void OnDestroy()
    {
        ScoreManager.instance.UnregisterConnectionPoint(this);
    }

    public void CheckPairing(ConnectionPointController partnerPoint)
    {
        if (partnerPoint.GetId() == partnerId)
        {
            Debug.Log(id + " ha chocado con " + partnerPoint.GetId() + ": Bien emparejadas!");
            pairedWithPartner = true;
        }
    }

    public string GetId()
    {
        return id;
    }

    public bool PairedWithPartner()
    {
        return pairedWithPartner;
    }

    public bool CanBeRegistered()
    {
        return canBeRegistered;
    }

    public void CanBeRegistered(bool canBeRegistered)
    {
        this.canBeRegistered = canBeRegistered;
    }

    public bool Paired()
    {
        return paired;
    }

    public void Paired(bool paired)
    {
        this.paired = paired;
    }

    public void SetPairId(string id)
    {
        pairId = id;
    }

    public PieceController GetPiece()
    {
        return piece;
    }

    public void ResetValues()
    {
        paired = false;
        pairedWithPartner = false;
        pairId = "";
        Debug.Log(id + " ha dejado de estar conectado");
    }

    public void Register()
    {
        if (canBeRegistered)
        {
            ScoreManager.instance.RegisterConnectionPoint(this);
        }
    }

    public void UnRegister()
    {
        if (canBeRegistered)
        {
            ResetValues();
            ScoreManager.instance.UnregisterConnectionPoint(this);
        }
    }

    public bool IsEnabled()
    {
        return isEnabled;
    }

    public void Enable()
    {
        isEnabled = true;
    }

    public void Disable()
    {
        isEnabled = false;
    }
}
