using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDrop : MonoBehaviour
{
    
    public static DragAndDrop currentlySelected = null;

    Vector3 offset; //Diferencia entre la posición del objeto y la del ratón
    public string destinationTag = "DropArea"; //Tag que tienen que tener los objetos donde dropearan los objetos
    public bool placed = false;
    public Transform CurrentDropArea; //Referencia a la DropArea donde está colocado
    private Vector3 initialPosition;

    
    public SelectObject selectObj;

    //Referencias a los scripts de minijuegos
    public Minigame1 minigame1Instance;
    public Minigame2 minigame2Instance;
    public string valveType; //Indicativo de las valvulas


    public bool locked = false;

    private void Start()
    {
        initialPosition = transform.position; //Al iniciar, guarda la posición inicial del objeto.
        selectObj = GetComponent<SelectObject>();


    }

    //inicio del drag
    void OnMouseDown() 
    {
        if (locked) return; // Bloqueja TOT el clic

        AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.Select);

        minigame1Instance.movimientos++;

        //Liberar la DropArea actual si estaba colocado
        //Liberar la DropArea actual si estaba colocado
        if (placed && CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();
            if (drop != null) drop.occupied = false;

            placed = false; // 👈 IMPORTANTE
            CurrentDropArea = null;

            //Actualizar contador
            if (minigame1Instance != null && minigame1Instance.gameObject.activeSelf)
                minigame1Instance.objectsRemaining();

            if (minigame2Instance != null && minigame2Instance.gameObject.activeSelf)
                minigame2Instance.objectsRemaining();
        }

        //Gestión de selección exclusiva con currentlySelected
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.selectObj?.Deselect();
        }

        //Selecciona este objeto desde SelectObject.Select() y se guarda como currentlySelected
        selectObj.Select();
        currentlySelected = this;


        //Asegura que cualquier otro objeto seleccionado a través de ObjectSelector se deseleccione si no es este
        if (ObjectSelector.currentlySelected != null && ObjectSelector.currentlySelected.gameObject != gameObject)
        {
            ObjectSelector.currentlySelected.Deselect();
        }

        ObjectSelector.currentlySelected = selectObj;
        selectObj.Select();

        //Calcula el drag
        offset = transform.position - MouseWorldPosition();
        GetComponent<Collider>().enabled = false;


    }


    //Mover el objeto
    void OnMouseDrag()
    {
        if (locked) return; // No permet moure

        transform.position = MouseWorldPosition() + offset; //El objeto sigue la posición del ratón en el mundo respetando el offset inicial
    }


    //Soltar el objeto
    void OnMouseUp()
    {
        if (locked) return;

        AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.Place);

        var rayOrigin = Camera.main.transform.position;
        var rayDirection = MouseWorldPosition() - Camera.main.transform.position;
        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo))
        {
            if (hitInfo.transform.CompareTag(destinationTag))
            {
                DropArea drop = hitInfo.transform.GetComponent<DropArea>();

                if (drop != null && !drop.occupied)
                {
                    transform.position = hitInfo.transform.position;
                    CurrentDropArea = hitInfo.transform;
                    ParticleManager.instance.SpawnParticles("PlacementDust", hitInfo.transform.position, Quaternion.identity);
                    drop.occupied = true;

                    if (!placed)
                    {
                        placed = true;

                        // Només notificar al minijoc ACTIU
                        if (minigame1Instance != null && minigame1Instance.gameObject.activeSelf)
                            minigame1Instance.objectsRemaining();

                        if (minigame2Instance != null && minigame2Instance.gameObject.activeSelf)
                            minigame2Instance.objectsRemaining();
                    }
                }
                else
                {
                    transform.position = initialPosition;
                }
            }
            else
            {
                transform.position = initialPosition;
            }
        }
        else
        {
            transform.position = initialPosition;
        }

        GetComponent<Collider>().enabled = true;
    }


    Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition; //Coge la posición del ratón en pantalla
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z; //Ajusta z usando la distancia del objeto a la cámara
        return Camera.main.ScreenToWorldPoint(mouseScreenPos); //Convierte esa posición de pantalla a coordenadas del mundo
    }
}
