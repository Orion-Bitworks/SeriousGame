using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using UnityEngine;

public class GameParametersMDB : MonoBehaviour
{
    public static GameParametersMDB Instance;

    private SessionTimer timer;
    public Parameters session;
    private PlayerSessionRepository repo;

    private bool repoInit = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = new SessionTimer();

        session = new Parameters();

        repoInit = false;

        timer.Start();
    }

    private void Update()
    {
        if (StartMongo.Instance.IsConnected && !repoInit)
        {
            NewRepo();
            repoInit = true;
        }
    }

    public void NewRepo()
    {
        repo = new PlayerSessionRepository();
    }

	public void SaveMinigameData(string name, int tiempo, int? intentos = null, int? movimientos = null, int? fallos = null, int? colocaciones = null)
	{
		var stats = new MinigameStats
		{
			Nombre = name,
			Tiempo = tiempo,
			Intentos = intentos,
			Movimientos = movimientos,
			Fallos = fallos,
			Colocaciones = colocaciones
		};

		session.Minigames.Add(stats);
	}

	public async Task SendData()
	{
        session.Id = ObjectId.GenerateNewId();
        session.sesionTime = timer.Stop();
        await repo.InsertSession(session);
        timer.Resume();
    }

	private void OnApplicationQuit()
	{
        SendData();
	}
}
