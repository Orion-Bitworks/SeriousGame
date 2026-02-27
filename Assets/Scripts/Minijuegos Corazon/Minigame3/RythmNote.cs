using TMPro;
using UnityEngine;

public class RythmNote : MonoBehaviour
{
    public KeyCode expectedKey;
    public TextMeshPro textKey;

    private Renderer rend;
    private bool isActive = false;
    private bool canBePressed = false;

    private Minigame3 manager;

    public void Init(KeyCode key, Minigame3 m)
    {
        expectedKey = key;
        manager = m;

        if (textKey != null)
            textKey.text = key.ToString();

        rend = GetComponent<Renderer>();
        SetActiveNote(false);
    }

    public void SetActiveNote(bool value)
    {
        isActive = value;

        if (rend != null)
            rend.material.color = value ? Color.green : Color.white;

        if (value)
            StartCoroutine(EnablePressNextFrame());
    }

    System.Collections.IEnumerator EnablePressNextFrame()
    {
        yield return null;
        canBePressed = true;
    }

    void Update()
    {
        if (!isActive || !canBePressed) return;

        // ❌ Si es prem qualsevol tecla incorrecta
        if (Input.anyKeyDown && !Input.GetKeyDown(expectedKey))
        {
            Debug.Log("ERROR: Has premut una tecla incorrecta! Tocava: " + expectedKey);
            return;
        }

        // ✅ Si es prem la correcta
        if (Input.GetKeyDown(expectedKey))
        {
            canBePressed = false;
            manager.NoteCompleted(this);
        }
    }
}