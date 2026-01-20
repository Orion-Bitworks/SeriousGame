using UnityEngine;

public class PieceController : MonoBehaviour
{
    ConnectObjects controller;
    Rigidbody rb;
    [SerializeField] bool hasSnapped = false;

    private void Start()
    {
        controller = FindObjectOfType<ConnectObjects>();

        if (hasSnapped)
        {
            return;
        }

        //EnableRigidBody();
    }

    public void SnapToPoint(ConnectionPointController point, Transform target, Transform targetParent)
    {
        if (hasSnapped)
        {
            return;
        }

        controller.StopControl();

        hasSnapped = true;

        // Permite a los raycast colisionar con la pieza
        gameObject.layer = 6; // Layer 6 -> Raycast

        //DisableRigidBody();
        GetComponent<Rotate3DObject>().enabled = false;

        // Guarda la rotacion original de la pieza
        Quaternion previousRotation = transform.rotation;

        transform.SetParent(targetParent, true);

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

        Vector3 refRight; // Vector Right temporal

        // Da direccion a vector Right dependiendo de si snapAxis es perpendicular a Up o no, completando las tres direcciones Right, Up y Forward
        if (Mathf.Abs(Vector3.Dot(snapAxis, Vector3.up)) > 0.99f)
        {
            refRight = Vector3.Cross(snapAxis, Vector3.forward).normalized; // Perpendicular a snapAxis (en este caso es paralelo a Up) y Forward
        }
        else
        {
            refRight = Vector3.Cross(snapAxis, Vector3.up).normalized; // Perpendicular a snapAxis (en este caso es paralelo a Forward) y Up
        }

        Vector3 refForward = Vector3.Cross(refRight, snapAxis).normalized; // Vector Forward temporal

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

    public void DisableRigidBody()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            rb.isKinematic = true;
            Destroy(GetComponent<Rigidbody>());
        }
    }

    public void EnableRigidBody()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.useGravity = false;
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

    public void UnParent()
    {
        transform.parent = null;
    }

    public bool HasSnapped()
    {
        return hasSnapped;
    }
}
