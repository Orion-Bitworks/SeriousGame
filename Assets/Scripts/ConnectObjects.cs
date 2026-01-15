using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class ConnectObjects : MonoBehaviour
{
    InputManager inputManager;
    Camera cam;
    [SerializeField] LayerMask mask;
    [SerializeField] PieceController cubePiece;
    PieceController pieceCopy;

    private float pointDistance = 10f;
    [SerializeField] private float minPointDistance = 2f;
    [SerializeField] private float maxPointDistance = 15f;

    void Start()
    {
        inputManager = InputManager.instance;
        cam = Camera.main;
    }

    public void CreateNewPiece(PieceController piece)
    {
        Vector3 mousePos = Input.mousePosition;
        pieceCopy = Instantiate(piece, mousePos, Quaternion.identity);
        pieceCopy.gameObject.layer = 0;
        pieceCopy.GetComponent<Rotate3DObject>().enabled = false;
    }

    void Update()
    {
        AdjustPointDistance();

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos = cam.ScreenToWorldPoint(mousePos);

        Debug.DrawRay(transform.position, mousePos, Color.blue);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (pieceCopy != null && !pieceCopy.HasSnapped())
        {
            if (Physics.Raycast(ray, out hit, pointDistance, mask))
            {
                pieceCopy.MovePiece(hit.point);
            }
            else
            {
                pieceCopy.MovePiece(ray.GetPoint(pointDistance));
            }
        }
    }

    public void AdjustPointDistance()
    {
        float scroll = inputManager.mouseWheel_ia.ReadValue<Vector2>().y;

        if (scroll != 0)
        {
            pointDistance += scroll * Time.deltaTime;
        }

        pointDistance = Mathf.Clamp(pointDistance, minPointDistance, maxPointDistance);
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
