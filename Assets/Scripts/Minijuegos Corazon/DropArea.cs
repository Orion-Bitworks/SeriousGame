using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropArea : MonoBehaviour
{
    public string valveType;
    public bool occupied;

    [Header("Validación")]
    public Vector3 requiredEulerAngles; // Rotación que debe tener la pieza
    public float rotationTolerance = 5f; // Margen de grados (si la rotacion tiene que ser 180 y la pieza esta 5 grados mas o menos de la pedida, lo aceptara igual)

    public Quaternion requiredRotation; //Rotacion que tiene que tener la pieza para estar correcta

    void Awake()
    {
        requiredRotation = Quaternion.Euler(requiredEulerAngles);
    }
}
