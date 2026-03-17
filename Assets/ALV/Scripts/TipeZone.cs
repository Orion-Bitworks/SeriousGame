using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum tipeZone { None, Alveolo, Vena}
public class TipeZone : MonoBehaviour
{

    public tipeZone tipe;

    private void OnTriggerEnter(Collider other)
    {
        MoleculaObject obj = other.GetComponent<MoleculaObject>();

        if(obj != null)
        {
            obj.ChangeTipe(tipe);

            if(tipe == tipeZone.Vena)
            {
                FindAnyObjectByType<SpawnVenaController>().MoleculaMovement(other.gameObject, this.gameObject);
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        MoleculaObject obj = other.GetComponent<MoleculaObject>();

        if(obj != null)
        {
            obj.SetNone();
        }
    }
}
