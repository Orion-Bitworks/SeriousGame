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
		if (!session.Minigames.ContainsKey(name))
			session.Minigames[name] = new MinigameStats();

		var stats = session.Minigames[name];

		stats.Tiempo = (stats.Tiempo ?? 0) + tiempo;

		if (intentos.HasValue)
			stats.Intentos = (stats.Intentos ?? 0) + intentos.Value;

		if (movimientos.HasValue)
			stats.Movimientos = (stats.Movimientos ?? 0) + movimientos.Value;

        if (fallos.HasValue)
            stats.Fallos = (stats.Fallos ?? 0) + fallos.Value;

        if (colocaciones.HasValue)
            stats.Colocaciones = (stats.Colocaciones ?? 0) + colocaciones.Value;
    }

	public async Task SendData()
	{
        session.Id = ObjectId.GenerateNewId();
        session.sesionTime = timer.Stop();
        await repo.InsertSession(session);
        timer.Resume();
    }
}
