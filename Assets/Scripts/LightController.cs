using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public static LightController instance;

    [SerializeField] GameObject light1;
    [SerializeField] GameObject light2;
    [SerializeField] GameObject light3;


	private void Awake()
	{
		instance = this;
	}

	public void TurnOnLight1(bool active)
    {
        light1.SetActive(active);
    }

    public void TurnOnLight2(bool active)
    {
		light2.SetActive(active);
	}

    public void TurnOnLight3(bool active)
    {
		light2.SetActive(active);
	}

    public void TurnOnAllLights(bool active)
    {
        if (active)
            AudioController.Instance.PlaySFX(SFX.MenuAmbient, (int)MenuAmbientSFX.SpotlightOn);

        light1.SetActive(active);
        light2.SetActive(active);
        light3.SetActive(active);
    }
}
