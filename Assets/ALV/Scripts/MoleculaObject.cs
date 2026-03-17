using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculaObject : MonoBehaviour
{

    [SerializeField] private tipeZone currentZone;

    public void ChangeTipe(tipeZone tipo)
    {
        currentZone = tipo;
        GetComponent<MoleculaLive>().OnZoneChanged(tipo);
    }

    public void SetNone()
    {
        currentZone = tipeZone.None;
		GetComponent<MoleculaLive>().OnZoneChanged(currentZone);

	}

	public bool IsInalveolo()
    {
        return currentZone == tipeZone.Alveolo;
    }
    public bool IsInVein()
    {
        return currentZone == tipeZone.Vena;
    }
    public tipeZone GetZone()
    {
        return currentZone;
    }

}
