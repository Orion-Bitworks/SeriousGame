using TMPro;
using UnityEngine;

public class RythmNote : MonoBehaviour
{
    public KeyCode expectedKey;
    public TextMeshPro textKey;

    private Renderer rend;
    private bool isActive = false;
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
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetKeyDown(expectedKey))
        {
            manager.NoteCompleted(this);
        }
    }
}