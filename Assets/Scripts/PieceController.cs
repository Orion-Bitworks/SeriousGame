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

        //transform.position = targetPos;

        //transform.localPosition = (transform.position - transform.parent.transform.localPosition).normalized;

        for (int i = 0; i < points.Length; i++)
        {
            points[i].DisablePoint();
        }

        //Vector3 offset = /*target.forward*/ new Vector3(0,0,1) * (blockSize * 0.5f);
        //Vector3 offset = target.forward * (blockSize / 2);
        Vector3 offset = Vector3.forward * 0.5f;

        //Vector3 offset = new Vector3(0, 0, Mathf.Round(target.forward.z) * (blockSize / 2));
        Vector3 worldOffset = target.TransformDirection(offset);
        Vector3 targetLocalOffset = targetParent.InverseTransformDirection(worldOffset);

        Debug.Log(offset);

        //transform.SetParent(targetParent);
        transform.SetParent(targetParent);
        //transform.position = target.localPosition + offset;
        
        transform.localRotation = Quaternion.Euler(target.localRotation.x, transform.localRotation.y, target.localRotation.z);
        //transform.localPosition = target.localPosition + offset;

        transform.localPosition = target.localPosition + targetLocalOffset;

        //Debug.Log(transform.localPosition);


        //transform.rotation = targetParent.rotation;

        
    }

    public bool TrySnapToPoint(ConnectionPointController c, Transform target, Transform targetParent)
    {
        if (HasSnapped) return false; // YA se llamó desde otro punto

        HasSnapped = true;  // Se marca INMEDIATAMENTE

        SnapToPoint(c, target, targetParent);
        return true;
    }

    public void MovePiece(Vector3 moveTarget)
    {
        if (canMove)
        {
            Vector3 followPos = moveTarget;
            rb.velocity = (followPos - transform.position) * 50f;
            transform.rotation = Quaternion.identity;
        }
    }

    public bool HasSnapped { get; private set; } = false;

    public void NotifySnapped()
    {
        HasSnapped = true;
    }
}
