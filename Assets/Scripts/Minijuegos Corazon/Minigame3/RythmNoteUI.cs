using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RythmNoteUI : MonoBehaviour
{
	public KeyCode expectedKey;
	public Image outerCircle;
	public Image innerCircle;
	public TextMeshProUGUI keyText;

	private float lifeTime = 1.5f;
	private float spawnTime;
	private bool expired = false;

	private NewMinigame3 manager;

	// Ventanas de precision en pixeles
	public float perfectWindow = 10f;
	public float goodWindow = 25f;

	public void Init(KeyCode key, NewMinigame3 m)
	{
		expectedKey = key;
		keyText.text = key.ToString();
		spawnTime = Time.time;
		manager = m;

		// Escala inicial del círculo grande
		outerCircle.rectTransform.localScale = Vector3.one;
	}

	void Update()
	{
		if (expired) return;

		float progress = (Time.time - spawnTime) / lifeTime;

		// Escala del circulo exterior (1 = grande, 0 = cerrado)
		float scale = Mathf.Lerp(1f, 0f, progress);
		outerCircle.rectTransform.localScale = new Vector3(scale, scale, 1f);

		// Si se cerró del todo fallo
		if (progress >= 1f)
		{
			expired = true;
			manager.RegisterMiss(this);
		}
	}

	// Devuelve el tipo de hit segun distancia real en pixeles
	public HitResult GetHitResult()
	{
		float outerSize = outerCircle.rectTransform.rect.width * outerCircle.rectTransform.localScale.x;
		float innerSize = innerCircle.rectTransform.rect.width;

		float diff = Mathf.Abs(outerSize - innerSize);

		if (diff <= perfectWindow)
			return HitResult.Perfect;

		if (diff <= goodWindow)
			return HitResult.Good;

		return HitResult.Miss;
	}

	public void ShowFeedback(HitResult result)
	{
		switch (result)
		{
			case HitResult.Perfect:
				AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.RythmPerfect);
                innerCircle.color = Color.green;
				break;

			case HitResult.Good:
				AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.RythmCorrect);
                innerCircle.color = Color.yellow;
				break;

			case HitResult.Miss:
				AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.RythmError);
                innerCircle.color = Color.red;
				break;
		}
	}
}

public enum HitResult
{
	Perfect,
	Good,
	Miss
}
