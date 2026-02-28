using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPointController : MonoBehaviour
{
    [SerializeField] private string id = "";
    [SerializeField] private string partnerId = "Undefined";
    [SerializeField] private LayerMask layerToDetect;
    [SerializeField] private Transform parent;
    [SerializeField] private bool canBeRegistered = false;

    private bool pairedWithPartner = false;
    PieceController piece;

    private void Start()
    {
        if (canBeRegistered)
        {
            ScoreManager.instance.RegisterConnectionPoint(this);
        }
        
        piece = GetComponentInParent<PieceController>();
        parent = transform.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ConnectionPoint")
        {
            piece.SnapToPoint(this, other.transform, other.transform.parent);
            CheckPairing(other.GetComponent<ConnectionPointController>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ConnectionPoint")
        {
            //piece.DisconnectPiece(other.GetComponent<PieceController>());
            //CheckPairing(other.GetComponent<ConnectionPointController>());
        }
        //piece.UnParent();
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
}
