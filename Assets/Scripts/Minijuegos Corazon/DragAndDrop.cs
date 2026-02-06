using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDrop : MonoBehaviour
{
    
    private static DragAndDrop currentlySelected = null;

    Vector3 offset; //Diferencia entre la posición del objeto y la del ratón
    public string destinationTag = "DropArea"; //Tag que tienen que tener los objetos donde dropearan los objetos
    public bool placed = false;
    public Transform CurrentDropArea; //Referencia a la DropArea donde está colocado
    private Vector3 initialPosition;

    
    private SelectObject selectObj;

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
        if (GetComponent<DragAndDrop>().locked) return; // Si está bloqueado, ignorar clic

        //Liberar la DropArea actual si estaba colocado
        if (placed && CurrentDropArea != null)
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>();
            if (drop != null) drop.occupied = false;
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
        transform.position = MouseWorldPosition() + offset; //El objeto sigue la posición del ratón en el mundo respetando el offset inicial
    }


    //Soltar el objeto
    void OnMouseUp()
    {
        //Calcula el ray para detectar la zona de drop
        var rayOrigin = Camera.main.transform.position;
        var rayDirection = MouseWorldPosition() - Camera.main.transform.position;
        RaycastHit hitInfo;

        //Si el raycast choca con algo
        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo))
        {
            if (hitInfo.transform.CompareTag(destinationTag)) //Lo compara con el tag y comprueba que sea o no
            {
                //Comprueba que el objeto golpeado tenga un componente DropArea y que no esté ocupado
                DropArea drop = hitInfo.transform.GetComponent<DropArea>();
                if (drop != null && !drop.occupied)
                {
                    transform.position = hitInfo.transform.position;
                    CurrentDropArea = hitInfo.transform;
                    drop.occupied = true;

                    if (!placed) //si esta colocado
                    {
                        placed = true; //lo marca como true
                        minigame1Instance.objectsRemaining();
                        minigame2Instance.objectsRemaining();
                        selectObj.Deselect(); 

                        ObjectSelector.currentlySelected = null;
                        selectObj.Deselect();
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
        //Una vez termina el drag and drop, vuelve a habilitar el Collider
        GetComponent<Collider>().enabled = true;
    }

    Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition; //Coge la posición del ratón en pantalla
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z; //Ajusta z usando la distancia del objeto a la cámara
        return Camera.main.ScreenToWorldPoint(mouseScreenPos); //Convierte esa posición de pantalla a coordenadas del mundo
    }
}
