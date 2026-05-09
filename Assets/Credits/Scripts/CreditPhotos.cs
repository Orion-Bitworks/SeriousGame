using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditPhotos : MonoBehaviour
{
    [SerializeField] private Image photoImage;
    [SerializeField] private List<Sprite> photos;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private float stayTime = 2f;

    private void Start()
    {
        StartCoroutine(PlayCredits());
    }

    private IEnumerator PlayCredits()
    {
        foreach (var photo in photos)
        {
            photoImage.sprite = photo;

            // Fade in
            yield return StartCoroutine(Fade(0f, 1f));

            // Stay
            yield return new WaitForSeconds(stayTime);

            // Fade out
            yield return StartCoroutine(Fade(1f, 0f));
        }
    }

    private IEnumerator Fade(float start, float end)
    {
        float t = 0f;
        Color c = photoImage.color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, end, t / fadeTime);
            photoImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
    }
}
