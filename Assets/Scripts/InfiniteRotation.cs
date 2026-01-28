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

    private Vector3 angleRotation;

    private float axisX = 0;
    private float axisY = 0;
    private float axisZ = 0;

    private void Start()
    {
        //angleRotation = new Vector3(x ? 1 : 0, y ? 1 : 0, z ? 1 : 0);
    }

    private void Update()
    {
        transform.Rotate(new Vector3(x ? 1 : 0, y ? 1 : 0, z ? 1 : 0) * speed * Time.deltaTime);
    }
}
