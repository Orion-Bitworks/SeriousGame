using TMPro;
using UnityEngine;

public class RythmNote : MonoBehaviour
{
    public KeyCode expectedKey; //Tecla que s'esperi que el jugador premi
   
    public TextMeshPro textKey;
    private Renderer rend; //Per canviar de color

    private bool isActive = false; //Indica si la nota esta activa en el joc
    private bool canBePressed = false; //Indica si la nota es pot premer

    private Minigame3 manager;

    void Update()
    {
        if (!isActive || !canBePressed) return; //Nomes processa la nota si esta activa i es pot premer

        // Si es prem qualsevol tecla incorrecta
        if (Input.anyKeyDown && !Input.GetKeyDown(expectedKey))
        {
            Debug.Log("Has apretado una tecla incorrecta! Tocaba: " + expectedKey);
            return;
        }

        // Si es prem la tecla correcte
        if (Input.GetKeyDown(expectedKey))
        {
            canBePressed = false;
            manager.NoteCompleted(this);
        }
    }

    //Inicialitza la nota amb la tecla que s'espera premer
    public void Init(KeyCode key, Minigame3 m)
    {
        expectedKey = key;
        manager = m;

        if (textKey != null) //Mostra la tecla al text si existeix
        {
            textKey.text = key.ToString();
        }

        rend = GetComponent<Renderer>();
        SetActiveNote(false);
    }

    //Activa o desactiva la nota visual
    public void SetActiveNote(bool value)
    {
        isActive = value;

        if (rend != null) //verd si esta activa, blanc si no ho esta
        {
            rend.material.color = value ? Color.green : Color.white;
        }
            

        if (value) //si s'activa, permet premer a la seguent actualització del frame
        {
            StartCoroutine(EnablePressNextFrame());
        }
            
    }

    //Corrutina per habilitar la tecla nomes al seguent frame
    System.Collections.IEnumerator EnablePressNextFrame()
    {
        yield return null;
        canBePressed = true;
    }



    
}