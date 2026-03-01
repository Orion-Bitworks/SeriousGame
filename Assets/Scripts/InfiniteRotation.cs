using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteRotation : MonoBehaviour
{
    [Header("Rotation axis")]
    [SerializeField] private bool x = false;
    [SerializeField] private bool y = false;
    [SerializeField] private bool z = false;

    [Header("Rotation speed")]
    [SerializeField] private float speed = 40f;

    [Header("Rotation parameters")]
    [SerializeField] private bool inverted = false;
    [SerializeField] private bool rotateOnCreation = false;

    private int value = 1;

    private bool canRotate = true;

    private Vector3 initialRotation;

    private void Start()
    {
        if (inverted)
        {
            value = -1;
        }

        initialRotation = transform.rotation.eulerAngles;

        canRotate = rotateOnCreation;
    }

    private void FixedUpdate()
    {
        if (canRotate)
        {
            transform.Rotate(new Vector3(x ? value : 0, y ? value : 0, z ? value : 0) * speed * Time.deltaTime);
        }
    }

    public void ToggleRotation(bool toggle)
    {
        canRotate = toggle;
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(initialRotation);
    }
}
