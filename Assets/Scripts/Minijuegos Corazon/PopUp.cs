using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public GameObject panel; //Panell del pop up
    public TextMeshProUGUI messageText; //missatge del pop up

    [SerializeField]
    private float duration;//Quant durara la visibilitat d'aquest pop up

    //Metode per mostrar el pop up amb text i una duració
    public void ShowPopUp(string text, float duration)
    {
        this.duration = duration;
        messageText.text = text;
        panel.SetActive(true);

        StopAllCoroutines(); //Parar qualsevol corrutina per no tenir conflictes
        StartCoroutine(HideAfterSeconds()); //Inicia la corrutina perque, després d'un cert temps, desaparegui
    }

    //Corrutina per esperar uns segons avans de que desaparegui
    private IEnumerator HideAfterSeconds()
    {
        yield return new WaitForSecondsRealtime(duration); //Temps real
        panel.SetActive(false);
    }

}
