using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Move3DObject : MonoBehaviour
{
    InputManager inputManager;
    Camera cam;
    [SerializeField] LayerMask mask;
    PieceController piece;

    private float pointDistance = 10f;
    private float breakSnapDistance = 2f;

    [SerializeField] private float minPointDistance = 2f;
    [SerializeField] private float maxPointDistance = 15f;

    private bool selected = false;
    Rigidbody rb;

    void Start()
    {
        inputManager = InputManager.instance;
        piece = GetComponent<PieceController>();
        cam = Camera.main;

        if (GetComponent<Rigidbody>())
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        if (!selected)
        {
            return;
        }

        if (piece.HasSnapped())
        {
            TryMove();
            return;
        }

        AdjustPointDistance();

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos = cam.ScreenToWorldPoint(mousePos);

        MovePiece(RaycastPoint());

    }

    private Vector3 RaycastPoint()
    {
        Vector3 raycastCollision;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pointDistance, mask))
        {
            pointDistance = Mathf.Clamp(hit.distance, minPointDistance, maxPointDistance);
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

        if (distance > breakSnapDistance)
        {
            MovePiece(RaycastPoint());
        }
    }

    public void MovePiece(Vector3 moveTarget)
    {
        Vector3 followPos = moveTarget;
        rb.velocity = (followPos - transform.position) * 50f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
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

    public void EnableMovement()
    {
        selected = true;
    }

    public void DisableMovement()
    {
        selected = false;
    }

    public void EnableRigidBody()
    {
        if (!GetComponent<Rigidbody>())
        {
            gameObject.layer = 0; // Layer 6 -> Raycast
            rb = gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.useGravity = false;
        }
    }

    public void DisableRigidBody()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            gameObject.layer = 6; // Layer 6 -> Raycast
            rb.isKinematic = true;
            Destroy(GetComponent<Rigidbody>());
        }
    }
}
