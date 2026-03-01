using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Move3DObject : MonoBehaviour
{
    InputManager inputManager;
    Camera cam;
    [SerializeField] LayerMask mask = 6;
    PieceController piece;

    private float pointDistance = 10f;
    private float breakSnapDistance = 1f;

    [SerializeField] private float minPointDistance = 2f;
    [SerializeField] private float maxPointDistance = 15f;

    private bool selected = false;

    void Start()
    {
        inputManager = InputManager.instance;
        piece = GetComponent<PieceController>();
        cam = Camera.main;
    }

    void Update()
    {
        AdjustPointDistance();

        if (!selected)
        {
            return;
        }

        if (piece.HasSnapped() && inputManager.separateMode_ia.inProgress)
        {
            TryMove();
            return;
        }

        MovePiece(RaycastPoint());
    }

    private Vector3 RaycastPoint()
    {
        Vector3 raycastCollision;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pointDistance, mask))
        {
            raycastCollision = hit.point;
        }
        else
        {
            raycastCollision = ray.GetPoint(pointDistance);
        }

        return raycastCollision;
    }

    public void TryMove()
    {
        float distance = Vector3.Distance(transform.position, RaycastPoint());

        CursorController.instance.ChangeCursorState(CursorController.CURSOR_STATE.SEPARATING);

        if (distance > breakSnapDistance)
        {
            piece.DisconnectAll();

            MovePiece(RaycastPoint());
        }
    }

    public void MovePiece(Vector3 moveTarget)
    {
        Vector3 followPos = moveTarget - piece.GetGroup().GetCentralPivot();

        if (piece.GetGroup() != null && piece.canSnap)
        {
            piece.GetGroup().MovePiece(followPos);
        }
    }

    public void AdjustPointDistance()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pointDistance, mask))
        {
            pointDistance = Mathf.Clamp(hit.distance + (transform.position - hit.point).magnitude, minPointDistance, maxPointDistance);
        }

        float scroll = inputManager.mouseWheel_ia.ReadValue<Vector2>().y;

        if (scroll != 0)
        {
            pointDistance += scroll * Time.deltaTime;
        }

        pointDistance = Mathf.Clamp(pointDistance, minPointDistance, maxPointDistance);
    }

    public void EnableMovement()
    {
        selected = true;
    }

    public void DisableMovement()
    {
        selected = false;
    }
}
