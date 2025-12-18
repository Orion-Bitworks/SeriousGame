using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class PieceController : MonoBehaviour
{
    [SerializeField] ConnectionPointController[] points;
    ConnectObjects controller;
    bool canMove = true;
    bool canAtach = true;
    Rigidbody rb;
    float blockSize = 1;

    [SerializeField] public bool snapped = false;

    private void Start()
    {
        controller = FindObjectOfType<ConnectObjects>();

        points = GetComponentsInChildren<ConnectionPointController>();

        if (snapped)
        {
            return;
        }

        rb = gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.useGravity = false;
    }

    public void SnapToPoint(ConnectionPointController c, Transform target, Transform targetParent)
    {
        snapped = true;

        if (!canAtach)
        {
            return;
        }
        controller.StopControl();

        this.gameObject.layer = 6;

        canAtach = false;
        canMove = false;
        rb.isKinematic = true;

        Destroy(GetComponent<Rigidbody>());

        for (int i = 0; i < points.Length; i++)
        {
            points[i].DisablePoint();
        }

        Quaternion previousRotation = transform.rotation;

        Vector3 offset = Vector3.forward * 0.5f;

        Vector3 worldOffset = target.TransformDirection(offset);
        Vector3 targetLocalOffset = targetParent.InverseTransformDirection(worldOffset);

        transform.SetParent(targetParent, true);

        // 1. Rotación base: conexión perfectamente alineada
        Quaternion baseRotation = Quaternion.LookRotation(-target.forward, target.up) * Quaternion.Inverse(c.transform.localRotation);

        // 2. Eje REAL de snap (normal del target)
        Vector3 snapAxis = -target.forward;

        // 3. Elegimos el cuadrante más cercano AL TARGET
        Quaternion finalRotation = GetBestAxialSnap(baseRotation, snapAxis, previousRotation);

        transform.rotation = finalRotation;

        Vector3 delta = c.transform.position - transform.position;
        transform.position = target.position - delta;

    }

    Quaternion GetBestAxialSnap(Quaternion baseRotation, Vector3 axis, Quaternion referenceRotation)
    {
        float[] angles = { 0f, 90f, 180f, 270f };

        Vector3 snapAxis = axis.normalized;

        // 1. Crear un sistema ortonormal alrededor del eje de snap
        Vector3 refRight = Vector3.Cross(snapAxis, Mathf.Abs(Vector3.Dot(snapAxis, Vector3.up)) > 0.99f ? Vector3.forward : Vector3.up).normalized;

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

    float SnapAngle90(float angle)
    {
        angle = (angle + 360f) % 360f;
        return Mathf.Round(angle / 90f) * 90f;
    }

    public bool TrySnapToPoint(ConnectionPointController c, Transform target, Transform targetParent)
    {
        if (HasSnapped) return false; 

        HasSnapped = true;  

        SnapToPoint(c, target, targetParent);
        return true;
    }

    public void MovePiece(Vector3 moveTarget)
    {
        if (canMove)
        {
            Vector3 followPos = moveTarget;
            rb.velocity = (followPos - transform.position) * 50f;
            //transform.rotation = Quaternion.identity; <==========================================================
            //transform.rotation = Quaternion.identity;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public bool HasSnapped { get; private set; } = false;

    public void NotifySnapped()
    {
        HasSnapped = true;
    }
}
