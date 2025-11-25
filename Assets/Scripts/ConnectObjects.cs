using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectObjects : MonoBehaviour
{
    Camera cam;
    [SerializeField] LayerMask mask;
    [SerializeField] GameObject piece;
    GameObject pieceCopy;

    void Start()
    {
        cam = Camera.main;
        pieceCopy = Instantiate(piece, new Vector3(0,0,0), Quaternion.identity);
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos = cam.ScreenToWorldPoint(mousePos);

        Debug.DrawRay(transform.position, mousePos, Color.blue);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 followPos;

        if (Physics.Raycast(ray, out hit, 100, mask))
        {
            Debug.Log(hit.transform.name);

            hit.transform.GetComponent<Renderer>().material.color = Color.red;

            followPos = hit.point;
            pieceCopy.transform.rotation = hit.transform.rotation;
        }
        else
        {
            followPos = ray.GetPoint(5); //- transform.position;
        }

        

        pieceCopy.transform.position = followPos;

    }

}
