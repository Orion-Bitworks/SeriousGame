using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

//[RequireComponent(typeof(Move3DObject))]
//[RequireComponent(typeof(Rotate3DObject))]
public class PieceController : MonoBehaviour
{
    [SerializeField] Material clickMaterial;
    Material originalMaterial;

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
        group = new PieceGroup();
        group.AddPiece(this);
    }

    private void Update()
    {
        if (InputManager.instance.deletePiece_ia.triggered && canSnap)
        {
            DeletePiece();
        }
    }

    public void SnapToPoint(ConnectionPointController point, Transform target, Transform targetParent)
    {
        if (!canSnap)
        {
            return;
        }

        point.Paired(true);
        target.GetComponent<ConnectionPointController>().Paired(true);

        Debug.Log($"Snap intento: {name} -> {targetParent.name} | canSnap:{canSnap}");

        PieceController otherPiece = targetParent.GetComponent<PieceController>();
        
        Vector3 oldPos = transform.position;
        Quaternion oldRot = transform.rotation;    

        // Guarda la rotacion original de la pieza
        Quaternion previousRotation = transform.rotation;

        // Conecta los ejes forward del punto de conexión de target y de la pieza
        Quaternion baseRotation = Quaternion.LookRotation(-target.forward, target.up) * Quaternion.Inverse(point.transform.localRotation);

        // Define la rotacion final de la pieza teniendo en cuenta la anterior a la colision
        Quaternion finalRotation = GetBestAxialSnap(baseRotation, -target.forward, previousRotation);
        transform.rotation = finalRotation;

        // Coloca la pieza para que coincidan los dos puntos de conexion
        Vector3 delta = point.transform.position - transform.position;
        Vector3 snappedPos = target.position - delta;
        
        transform.position = snappedPos;

        Vector3 posDelta = transform.position - oldPos;
        Quaternion rotDelta = transform.rotation * Quaternion.Inverse(oldRot);

        transform.position = oldPos;
        transform.rotation = oldRot;

        Vector3 pivot = transform.position;

        foreach (var piece in group.GetPieces())
        {
            Vector3 dir = piece.transform.position - pivot;
            dir = rotDelta * dir;

            piece.transform.position = pivot + dir + posDelta;
            piece.transform.rotation = rotDelta * piece.transform.rotation;
        }

        ConnectPieces(targetParent.GetComponent<PieceController>());

        DisableControls();
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
        }

        if (!otherPiece.connectedPieces.Contains(this))
        {
            otherPiece.connectedPieces.Add(this);
        }

        hasSnapped = true;
        otherPiece.HasSnapped(true);

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

    public void EnableControls()
    {
        group.ChangeGroupLayer(0);
        group.ChangeGroupMaterial(clickMaterial);

        movement.EnableMovement();
        rotation.EnableRotation();

        canSnap = true;

        foreach (PieceController piece in connectedPieces)
        {
            piece.canSnap = true;
        }
    }

    public void DisableControls()
    {
        group.ChangeGroupLayer(8);
        group.ChangeGroupMaterial(originalMaterial);

        movement.DisableMovement();
        rotation.DisableRotation();

        //canSnap = false;

        foreach (PieceController piece in connectedPieces)
        {
            piece.canSnap = false;
        }

        CursorController.instance.ChangeCursorState(CursorController.CURSOR_STATE.DEFAULT);
    }

    public bool HasSnapped()
    {
        return hasSnapped;
    }

    public void HasSnapped(bool b)
    {
        hasSnapped = b;
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

        group.ChangeGroupLayer(8);
        group.ChangeGroupMaterial(originalMaterial, this);

        foreach (PieceController piece in connectedPiecesCopy)
        {
            DisconnectPiece(piece);
        }

        foreach (PieceController piece in connectedPiecesCopy)
        {
            piece.hasSnapped = false;
            piece.canSnap = false;
        }

        PieceGroupManager.RebuildGroups();
    }

    public bool CanSnap()
    {
        return canSnap;
    }

    public void CanSnap(bool canSnap)
    {
        this.canSnap = canSnap;
    }

    public void DeletePiece()
    {
        foreach (PieceController piece in group.GetPieces())
        {
            Destroy(piece.gameObject);
        }

        CursorController.instance.ChangeCursorState(CursorController.CURSOR_STATE.DEFAULT);
    }
}
