using System;

public class SessionTimer
{
	private DateTime startTime;
	private bool running;

	public void Start()
	{
		startTime = DateTime.UtcNow;
		running = true;
	}

	public void Resume()
	{
		running = true;
	}

	public int Stop()
	{
		if (!running) return 0;

		running = false;
		return (int)(DateTime.UtcNow - startTime).TotalSeconds;
	}
}
