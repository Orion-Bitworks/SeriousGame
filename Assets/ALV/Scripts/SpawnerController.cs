using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{

    [SerializeField] GameObject spawnVena;

    [SerializeField] GameObject molecula;
    [SerializeField] PoolMoleculas pool;

    private SpawnAreaAlbeolo spawnAlbArea;
    private SpawnVenaController spawnVenaController;
    [SerializeField] Material O2Material;
    [SerializeField] Material CO2Material;

    public float intervalo = 2f;

    bool buttonPressed;
    

    // Start is called before the first frame update
    void Start()
    { 
        spawnAlbArea = GetComponent<SpawnAreaAlbeolo>();
        spawnVenaController = GetComponent<SpawnVenaController>();

    }

    // Update is called once per frame
    void Update()
    {
    }
    /// <summary>
    /// metodo para spawnear las moleculas en el alveolo
    /// </summary>
    public void SpawnMoleculasAlv()
    {
        Vector3 pos = spawnAlbArea.GetRandomPoint();
        var obj = pool.Get();
        obj.transform.position = pos;
        obj.GetComponent<MeshRenderer>().material = O2Material;
        obj.GetComponent<MoleculaObject>().ChangeTipe(tipeZone.Alveolo);
        obj.tag = "MolAlv";

    }
    /// <summary>
    /// Metodo para spawnear las moleculas en 
    /// </summary>
    /// <param name="spawn"></param>
    public void SpawnMoleculasVena(GameObject spawn)
    {
        var obj = pool.Get();
        obj.transform.position = spawn.transform.position;
        obj.GetComponent<MoleculaObject>().ChangeTipe(tipeZone.Vena);
        obj.GetComponent<MeshRenderer>().material = CO2Material;
        obj.tag = "MolVein";
        spawnVenaController.MoleculaMovement(obj, spawn);

    }

    public void StartSpawns()
    {
        StartCoroutine(Spawners());

    }

    public void StopSpawns()
    {
        StopCoroutine(Spawners());
    }



    private IEnumerator Spawners()
    {
        if (buttonPressed == false)
            buttonPressed = true;
        else
            buttonPressed = false;

        while (buttonPressed == true)
        {
            SpawnMoleculasAlv();
            SpawnMoleculasVena(spawnVena);

            yield return new WaitForSeconds(intervalo);
        }
    }
}
