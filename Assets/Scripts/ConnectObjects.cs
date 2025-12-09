using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectObjects : MonoBehaviour
{
    Camera cam;
    [SerializeField] LayerMask mask;
    [SerializeField] PieceController piece;
    PieceController pieceCopy;

    bool canMove = true;
    bool snapped = false;
    Rigidbody rb;

    ConnectionPointController[] points;

    void Start()
    {
        cam = Camera.main;
        CreateNewPiece();
    }

    public void CreateNewPiece()
    {
        Vector3 mousePos = Input.mousePosition;
        pieceCopy = Instantiate(piece, mousePos, Quaternion.identity);
        pieceCopy.gameObject.layer = 0;
        pieceCopy.GetComponent<Rotate3DObject>().enabled = false;

        //rb = pieceCopy.AddComponent<Rigidbody>();
        //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        //rb.useGravity = false;

        //points = pieceCopy.GetComponentsInChildren<ConnectionPointController>();

        //canMove = true;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos = cam.ScreenToWorldPoint(mousePos);

        Debug.DrawRay(transform.position, mousePos, Color.blue);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        /*for (int i = 0; i < points.Length; i++)
        {
            if (points[i].GetTargetPos() != Vector3.zero && canMove)
            {
                canMove = false;
                
                pieceCopy.transform.position = points[i].GetTargetPos();
                pieceCopy.transform.parent = points[i].GetParent();
                pieceCopy.transform.localPosition = (pieceCopy.transform.position - pieceCopy.transform.parent.transform.localPosition).normalized;

                Debug.Log("Detectado");

                rb.isKinematic = true;
                pieceCopy.transform.rotation = Quaternion.identity;
                pieceCopy = null;
            }
        }*/

        /*if (canMove)
        {
            Vector3 followPos;

            if (Physics.Raycast(ray, out hit, 100f, mask))
            {

                followPos = hit.point;
                pieceCopy.transform.rotation = hit.transform.rotation;
            }
            else
            {
                followPos = ray.GetPoint(10);
                pieceCopy.transform.rotation = Quaternion.identity;
            }

            rb.velocity = (followPos - pieceCopy.transform.position) * 50f;
        }*/

        //pieceCopy.CheckIfCollided();

        if (pieceCopy != null && !pieceCopy.snapped)
        {
            if (Physics.Raycast(ray, out hit, 100f, mask))
            {
                pieceCopy.MovePiece(hit.point);
                //followPos = hit.point;
                pieceCopy.transform.rotation = hit.transform.rotation;
            }
            else
            {
                pieceCopy.MovePiece(ray.GetPoint(10));
                //followPos = ray.GetPoint(10);
                //pieceCopy.transform.rotation = Quaternion.identity;
            }
        }
    }

    public void StopControl()
    {
        pieceCopy = null;
    }

    public void DisableLayer()
    {
        mask = 0;
    }

    public LayerMask GetHitMask()
    {
        return mask;
    }
}
