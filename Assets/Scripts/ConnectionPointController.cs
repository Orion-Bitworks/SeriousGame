using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPointController : MonoBehaviour
{
    [SerializeField] LayerMask layerToDetect;
    [SerializeField] Transform parent;

    private void Start()
    {
        parent = transform.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ConnectionPoint")
        {
            //Debug.Break();
            PieceController piece = GetComponentInParent<PieceController>();

            /*if (piece.HasSnapped)
            {
                return;
            }*/

            //DisablePoint();

            Debug.Log(gameObject.name + " ha chocado con: " + other.gameObject.name);
            piece.SnapToPoint(this, other.transform, other.transform.parent);
        }
    }

    public void DisablePoint()
    {
        //this.enabled = false;
        Destroy(GetComponent<ConnectionPointController>());
    }
}
