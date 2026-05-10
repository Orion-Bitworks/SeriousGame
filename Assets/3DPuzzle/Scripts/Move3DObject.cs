using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class Move3DObject : MonoBehaviour
{
    InputManager inputManager;
    Camera cam;
    [SerializeField] LayerMask mask = 8;
    PieceController piece;

    private float pointDistance = 3f;
    private float breakSnapDistance = 1f;

    private float minPointDistance = 2f;
    private float maxPointDistance = 6f;

    private float scrollSpeed = 6f;

    private bool selected = false;

    private float targetPointDistance;

    void Start()
    {
        inputManager = InputManager.instance;
        piece = GetComponent<PieceController>();
        cam = Camera.main;
        targetPointDistance = pointDistance;
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
            piece.GetGroup().CanMove(false);
            TryMove();
            //return;
        }

        MovePiece(RaycastPoint());
    }

    private Vector3 RaycastPoint()
    {
        Vector3 raycastCollision;

        Ray ray = cam.ScreenPointToRay(CursorManager.Position);
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
            PieceGroupManager.RebuildGroups();
            piece.IsPlaced(false);
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
        Ray ray = cam.ScreenPointToRay(CursorManager.Position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pointDistance, mask))
        {
            pointDistance = Mathf.Clamp(hit.distance + (transform.position - hit.point).magnitude, minPointDistance, maxPointDistance);
        }

        float scroll = 0f;

        if (!CursorManager.UsingGamepad)
        {
            scroll = inputManager.mouseWheel_ia.ReadValue<Vector2>().y;
        }
        else
        {
            if (Gamepad.current.rightShoulder.isPressed)
            {
                scroll += 1f * scrollSpeed;
            }

            if (Gamepad.current.leftShoulder.isPressed)
            {
                scroll -= 1f * scrollSpeed;
            }
        }

        if (scroll != 0)
        {
            targetPointDistance += scroll * Time.deltaTime;
        }

        targetPointDistance = Mathf.Clamp(targetPointDistance, minPointDistance, maxPointDistance);

        pointDistance = Mathf.Lerp(pointDistance, targetPointDistance, Time.deltaTime * 10f);

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
