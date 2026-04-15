using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public DragAndDrop dragAndDrop;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DropArea")){
            dragAndDrop.TrySnap(other.transform);
        }
    }
}
