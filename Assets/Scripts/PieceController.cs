using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Move3DObject))]
//[RequireComponent(typeof(Rotate3DObject))]
public class PieceController : MonoBehaviour
{
    [SerializeField] Material clickMaterial;
    Material originalMaterial;

    Rigidbody rb;
    [SerializeField] bool hasSnapped = false;
    public bool canSnap = false;

    public Transform parentPiece;

    private Move3DObject movement;
    private Rotate3DObject rotation;

    [SerializeField] private PieceGroup group;

    [SerializeField] private List<PieceController> connectedPieces = new List<PieceController>();

    private void Awake()
    {
        originalMaterial = GetComponent<MeshRenderer>().material;
        movement = GetComponent<Move3DObject>();
        rotation = GetComponent<Rotate3DObject>();

        PieceGroupManager.RegisterPiece(this);
    }

    private void Start()
    {
        group = new PieceGroup(/*originalMaterial, clickMaterial*/);
        group.AddPiece(this);
    }

    public void SnapToPoint(ConnectionPointController point, Transform target, Transform targetParent)
    {
        if (hasSnapped)
        {
            return;
        }

        if (!canSnap)
        {
            return;
        }

        //hasSnapped = true;

        DisableControls();

        // Guarda la rotacion original de la pieza
        Quaternion previousRotation = transform.rotation;

        ConnectPieces(targetParent.GetComponent<PieceController>());

        //transform.SetParent(targetParent, true);
        //parentPiece = targetParent;

        //parentPiece.GetComponent<PieceController>().GetChildrenPieces().Add(transform);

        // Conecta los ejes forward del punto de conexión de target y de la pieza
        Quaternion baseRotation = Quaternion.LookRotation(-target.forward, target.up) * Quaternion.Inverse(point.transform.localRotation);

        // Define la rotacion final de la pieza teniendo en cuenta la anterior a la colision
        Quaternion finalRotation = GetBestAxialSnap(baseRotation, -target.forward, previousRotation);
        transform.rotation = finalRotation;

        // Coloca la pieza para que coincidan los dos puntos de conexion
        Vector3 delta = point.transform.position - transform.position;
        transform.position = target.position - delta;
    }

    Quaternion GetBestAxialSnap(Quaternion baseRotation, Vector3 snapAxis, Quaternion referenceRotation)
    {
        float[] angles = { 0f, 90f, 180f, 270f }; // Angulos en que se puede rotar

        snapAxis = snapAxis.normalized;

        // Usamos la rotacion previa como base
        Vector3 refForward = (referenceRotation * Vector3.forward).normalized;

        // Si el forward esta alineado con el eje de snap, usamos right
        if (Mathf.Abs(Vector3.Dot(refForward, snapAxis)) > 0.99f)
        {
            refForward = (referenceRotation * Vector3.right).normalized;
        }

        // Proyectamos al plano perpendicular al eje de snap
        refForward = Vector3.ProjectOnPlane(refForward, snapAxis).normalized;

        Vector3 refDir = Vector3.ProjectOnPlane(referenceRotation * refForward, snapAxis).normalized; // Direccion de referencia para la rotación

        Quaternion newRotation = baseRotation;
        float bestScore = -Mathf.Infinity;

        // Rota los ejes y decide la rotación que mas se parezca a su rotacion original
        foreach (float angle in angles)
        {
            Quaternion candidate = Quaternion.AngleAxis(angle, snapAxis) * baseRotation;

            Vector3 candidateDirection = Vector3.ProjectOnPlane(candidate * refForward, snapAxis).normalized;

            float dot = Vector3.Dot(candidateDirection, refDir); // Valor que, cuando mas grande es, mas parecidas son las rotaciones

            // Evita que se confunda entre rotación 0 y 360
            if (angle == 0f)
            {
                dot += 0.001f;
            }

            if (dot > bestScore)
            {
                bestScore = dot;
                newRotation = candidate;
            }
        }

        return newRotation;
    }

    public void ConnectPieces(PieceController otherPiece)
    {
        if (!connectedPieces.Contains(otherPiece))
        {
            connectedPieces.Add(otherPiece);
            hasSnapped = true;
        }

        if (!otherPiece.connectedPieces.Contains(this))
        {
            otherPiece.connectedPieces.Add(this);
            otherPiece.HasSnapped(true);
        }

        group.MergeGroups(otherPiece.GetGroup());
    }

    public void DisconnectPiece(PieceController otherPiece)
    {
        connectedPieces.Remove(otherPiece);
        otherPiece.connectedPieces.Remove(this);

        if (connectedPieces.Count == 0)
        {
            hasSnapped = false;
        }
    }

    public void MovePiece(Vector3 moveTarget)
    {
        if (!hasSnapped && rb != null)
        {
            Vector3 followPos = moveTarget;
            rb.velocity = (followPos - transform.position) * 50f;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void EnableRigidBody()
    {
        if (GetComponent<Rigidbody>())
        {
            //canSnap = true;
            //GetComponent<MeshRenderer>().material = clickMaterial;
            //gameObject.layer = 0;
            rb = GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.useGravity = false;
            rb.isKinematic = false;
        }
    }

    public void DisableRigidBody()
    {
        if (GetComponent<Rigidbody>())
        {
            //canSnap = false;
            //GetComponent<MeshRenderer>().material = originalMaterial;
            //gameObject.layer = 6; // Layer 6 -> Raycast
            rb.isKinematic = true;
            //Destroy(GetComponent<Rigidbody>());
        }
    }

    public void EnableControls()
    {
        //EnableRigidBody();
        canSnap = true;
        //GetComponent<MeshRenderer>().material = clickMaterial;
        gameObject.layer = 0;
        movement.EnableMovement();
        rotation.EnableRotation();
    }

    public void DisableControls()
    {
        //DisableRigidBody();
        canSnap = false;
        //GetComponent<MeshRenderer>().material = originalMaterial;
        gameObject.layer = 6; // Layer 6 -> Raycast
        movement.DisableMovement();
        rotation.DisableRotation();
    }

    public void UnParent()
    {
        transform.parent = null;
        hasSnapped = false;
    }

    public bool HasSnapped()
    {
        return hasSnapped;
    }

    public void HasSnapped(bool b)
    {
        hasSnapped = b;
    }

    public void SwitchWithParent()
    {
        if (!transform.parent)
        {
            return;
        }

        PieceController controller = parentPiece.GetComponent<PieceController>();

        UnParent();

        if (parentPiece.parent)
        {
            controller.SwitchWithParent();
        }

        controller.hasSnapped = true;
        parentPiece.SetParent(transform, true);
        controller.parentPiece = transform;
        parentPiece = null;
    }

    public PieceController GetPieceToSnap()
    {
        if (transform.parent)
        {
            return transform.parent.GetComponent<PieceController>().GetPieceToSnap();
        }
        else
        {
            return this;
        }
    }

    public PieceGroup GetGroup()
    {
        return group;
    }

    public void SetGroup(PieceGroup newGroup)
    {
        group = newGroup;
    }

    public List<PieceController> ConnectedPieces()
    {
        return connectedPieces;
    }

    public void DisconnectAll()
    {
        List<PieceController> connectedPiecesCopy = new List<PieceController>(connectedPieces);

        foreach (PieceController piece in connectedPiecesCopy)
        {
            DisconnectPiece(piece);
        }

        PieceGroupManager.RebuildGroups();
    }
}
