using System.Collections;
using System.Collections.Generic;
using System.Net;
using MongoDB.Bson;
using UnityEngine;

public class StartMongo : MonoBehaviour
{
	public static MongoDBService mongo;
    public static StartMongo Instance;

    public bool IsConnected { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        StartCoroutine(ConnectRoutine());
    }

    private IEnumerator ConnectRoutine()
    {
        while (!IsConnected)
        {
            try
            {
                Debug.Log("Intentando conectar a MongoDB...");

                mongo = new MongoDBService(
                    "mongodb+srv://a24vicmilsal_db_user:ELcSdHLfU9J2NnpH@cluster0.gfvtmme.mongodb.net/",
                    "Parameters"
                );

                // Test insert
                var testCollection = mongo.GetCollection<BsonDocument>("TestConnection");
                testCollection.InsertOne(new BsonDocument("status", "ok"));

                IsConnected = true;
                Debug.Log("Conexión a MongoDB establecida correctamente.");
            }
            catch
            {
                Debug.LogWarning("No se pudo conectar a MongoDB. Reintentando en 5 segundos...");
            }

            if (!IsConnected)
                yield return new WaitForSeconds(5f);
        }
    }
}
