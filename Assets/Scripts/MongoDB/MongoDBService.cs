using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

public class MongoDBService
{
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> userCollection;

	//var users = App.DB.GetCollection<User>("users");



	public MongoDBService(string connectionString, string dbName)
    {
        //string connectionString = "mongodb+srv://a24vicmilsal_db_user:ELcSdHLfU9J2NnpH@cluster0.gfvtmme.mongodb.net/?appName=Cluster0";
        try
        {
            client = new MongoClient(connectionString);

			database = client.GetDatabase(dbName);

			Debug.Log("MongoDB conectado.");

		}
		catch
        {
            Debug.Log("Error de conexión con mongo");
        }
    
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return database.GetCollection<T>(name);

    }

}
