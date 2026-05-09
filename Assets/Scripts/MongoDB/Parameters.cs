using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Parameters
{
	//Globales
	[BsonId]
	public ObjectId Id { get; set; }
	public int sesionTime { get; set; }

	public List<MinigameStats> Minigames { get; set; } = new List<MinigameStats>();
}

public class MinigameStats
{
	public string Nombre { get; set; }
	public int? Tiempo { get; set; }
	public int? Intentos { get; set; }
	public int? Fallos { get; set; }
	public int? Movimientos { get; set; }
	public int? Colocaciones { get; set; }

}
