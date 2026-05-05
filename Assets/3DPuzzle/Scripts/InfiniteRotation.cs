using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteRotation : MonoBehaviour
{
    //[Header("Rotation axis")]
    [HideInInspector, SerializeField] private bool x = false;
    [HideInInspector, SerializeField] private bool y = true;
    [HideInInspector, SerializeField] private bool z = false;

    //[Header("Rotation speed")]
    [HideInInspector, SerializeField] private float speed = 60f;

    //[Header("Rotation parameters")]
    [HideInInspector, SerializeField] private bool inverted = false;
    [HideInInspector, SerializeField] private bool rotateOnCreation = true;
    [HideInInspector, SerializeField] private bool random = false;

    private int value = 1;

    private bool canRotate = true;

    private Vector3 initialRotation;
    private Vector3 randomRotation = Vector3.zero;
    private void Start()
    {
        if (inverted)
        {
            value = -1;
        }

        if (random)
        {
            Debug.Log("He entrado en random");
            randomRotation = Random.insideUnitSphere;
        }

        initialRotation = transform.rotation.eulerAngles;

        canRotate = rotateOnCreation;
    }

    private void FixedUpdate()
    {
        if (canRotate && !random)
        {
            transform.Rotate(new Vector3(x ? value : 0, y ? value : 0, z ? value : 0) * speed * Time.deltaTime);
        }
        else if (canRotate && random)
        {
            transform.Rotate(randomRotation * GameManager.Instance.velocityMultiplier * speed * Time.deltaTime);
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

    public bool GetX()
    {
        return x;
    }

    public void SetX(bool b)
    {
        x = b;
    }

    public bool GetY()
    {
        return y;
    }

    public void SetY(bool b)
    {
        y = b;
    }

    public bool GetZ()
    {
        return z;
    }

    public void SetZ(bool b)
    {
        z = b;
    }

    public bool GetRandom()
    {
        return random;
    }

    public void SetRandom(bool b)
    {
        random = b;
    }
    public bool GetInverted()
    {
        return inverted;
    }

    public void SetInverted(bool b)
    {
        inverted = b;
    }

    public bool GetRotateOnCreation()
    {
        return rotateOnCreation;
    }

    public void SetRotateOnCreation(bool b)
    {
        rotateOnCreation = b;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
