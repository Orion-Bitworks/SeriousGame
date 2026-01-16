using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPointController : MonoBehaviour
{
    [SerializeField] private string id = "";
    [SerializeField] private string partnerId = "Undefined";
    [SerializeField] private LayerMask layerToDetect;
    [SerializeField] private Transform parent;

    private bool pairedWithPartner = false;

    private void Start()
    {
        parent = transform.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ConnectionPoint")
        {
            PieceController piece = GetComponentInParent<PieceController>();

            piece.SnapToPoint(this, other.transform, other.transform.parent);
            CheckPairing(other.GetComponent<ConnectionPointController>());
        }
    }

    public void CheckPairing(ConnectionPointController partnerPoint)
    {
        if (partnerPoint.GetId() == partnerId)
        {
            Debug.Log(id + " ha chocado con " + partnerPoint.GetId() + ": Emparejadas!");
            pairedWithPartner = true;
        }
        else
        {
            Debug.Log(id + " ha chocado con " + partnerPoint.GetId() + ": No emparejadas...");
        }
    }

    public string GetId()
    {
        return id;
    }
}
