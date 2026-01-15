using UnityEngine;

public class PieceController : MonoBehaviour
{
    ConnectObjects controller;
    Rigidbody rb;
    [SerializeField] bool hasSnapped = false;

    //[SerializeField] ConnectionPointController[] points;
    //bool canMove = true;
    //bool canAtach = true;
    //float blockSize = 1;
    //[SerializeField] public bool snapped = false;

    private void Start()
    {
        controller = FindObjectOfType<ConnectObjects>();

        //points = GetComponentsInChildren<ConnectionPointController>();

        if (hasSnapped)
        {
            return;
        }

        EnableRigidBody();
    }

    public void SnapToPoint(ConnectionPointController c, Transform target, Transform targetParent)
    {
        //snapped = true;

        if (hasSnapped)
        {
            return;
        }

        controller.StopControl();

        hasSnapped = true;
        this.gameObject.layer = 6;

        //canAtach = false;
        //canMove = false;

        DisableRigidBody();

        // Desactiva todos los ConnectionPoints del objeto
        /*for (int i = 0; i < points.Length; i++)
        {
            points[i].DisablePoint();
        }*/

        Quaternion previousRotation = transform.rotation;

        transform.SetParent(targetParent, true);

        // 1. Rotación base: conexión perfectamente alineada
        Quaternion baseRotation = Quaternion.LookRotation(-target.forward, target.up) * Quaternion.Inverse(c.transform.localRotation);

        // 2. Eje REAL de snap (normal del target)
        //Vector3 snapAxis = -target.forward;

        // 3. Elegimos el cuadrante más cercano AL TARGET
        Quaternion finalRotation = GetBestAxialSnap(baseRotation, -target.forward, previousRotation);

        transform.rotation = finalRotation;

        Vector3 delta = c.transform.position - transform.position;
        transform.position = target.position - delta;
    }

    Quaternion GetBestAxialSnap(Quaternion baseRotation, Vector3 axis, Quaternion referenceRotation)
    {
        float[] angles = { 0f, 90f, 180f, 270f };

        Vector3 snapAxis = axis.normalized;

        // 1. Crear un sistema ortonormal alrededor del eje de snap
        Vector3 refRight;

        if (Mathf.Abs(Vector3.Dot(snapAxis, Vector3.up)) > 0.99f)
        {
            refRight = Vector3.Cross(snapAxis, Vector3.forward).normalized;
        }
        else
        {
            refRight = Vector3.Cross(snapAxis, Vector3.up).normalized;
        }

        Vector3 refForward = Vector3.Cross(refRight, snapAxis).normalized;

        // 2. Dirección de referencia proyectada en el plano del snap
        Vector3 refDir = Vector3.ProjectOnPlane(referenceRotation * refForward, snapAxis).normalized;

        Quaternion best = baseRotation;
        float bestScore = -Mathf.Infinity;

        foreach (float angle in angles)
        {
            Quaternion candidate = Quaternion.AngleAxis(angle, snapAxis) * baseRotation;

            Vector3 candDir = Vector3.ProjectOnPlane(candidate * refForward, snapAxis).normalized;

            float dot = Vector3.Dot(candDir, refDir);

            // Bias mínimo para preferir no rotar
            if (angle == 0f)
            {
                dot += 0.001f;
            }

            if (dot > bestScore)
            {
                bestScore = dot;
                best = candidate;
            }
        }

        return best;
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

    public bool HasSnapped()
    {
        return hasSnapped;
    }

    /*public bool TrySnapToPoint(ConnectionPointController c, Transform target, Transform targetParent)
    {
        if (hasSnapped) return false; 

        hasSnapped = true;  

        SnapToPoint(c, target, targetParent);
        return true;
    }*/

    //public bool HasSnapped { get; private set; } = false;

    /*public void NotifySnapped()
    {
        HasSnapped = true;
    }*/
}
