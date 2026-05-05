using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using UnityEngine;

public class PlayerSessionRepository
{
	private readonly IMongoCollection<Parameters> collection;

	public PlayerSessionRepository()
	{
		collection = StartMongo.mongo.GetCollection<Parameters>("Parameters");
	}

    public async Task InsertSession(Parameters session)
    {
        try
        {
            await collection.InsertOneAsync(session);
            Debug.Log("SESIÓN ENVIADA CORRECTAMENTE");
        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR ENVIANDO SESIÓN: " + ex);
        }
    }
}
