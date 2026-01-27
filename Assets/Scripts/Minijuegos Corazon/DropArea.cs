using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropArea : MonoBehaviour
{
    public string valveType;
    public bool occupied;

    [Header("Validación")]
    public Vector3 requiredEulerAngles;
    public float rotationTolerance = 5f;

    public Quaternion requiredRotation;

    void Awake()
    {
        requiredRotation = Quaternion.Euler(requiredEulerAngles);
    }
}
