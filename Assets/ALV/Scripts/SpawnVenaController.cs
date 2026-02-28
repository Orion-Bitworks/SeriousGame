using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SpawnVenaController : MonoBehaviour
{
    float speed = 2f;


    public void MoleculaMovement(GameObject obj, GameObject spawn)
    {
		Rigidbody rb = obj.GetComponent<Rigidbody>();
        

        if(rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }

        rb.velocity = Vector3.right * speed;

         
    }

}
