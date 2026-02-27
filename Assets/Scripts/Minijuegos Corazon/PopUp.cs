using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI messageText;

    [SerializeField]
    private float duration;

    public void ShowPopUp(string text)
    {
        messageText.text = text;
        panel.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideAfterSeconds());
    }

    private IEnumerator HideAfterSeconds()
    {
        yield return new WaitForSeconds(duration);
        panel.SetActive(false);
    }

}
