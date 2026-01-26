using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{

    Vector3 offset; //Distancia entre objeto y mouse
    public string destinationTag = "DropArea"; //Area donde se suelta el objeto

    public bool placed = false; //Comprobar si las piezas estan colocadas
    public Transform CurrentDropArea;
    private Vector3 initialPosition;
    public ProgressManager progressManager;

    [Header("Informacion Minijuego")]
    public string valveType;
    

    private void Start()
    {
        initialPosition = transform.position; 
    }


    //Al hacer click al objeto
    void OnMouseDown()
    {
        if (placed && CurrentDropArea != null) //Si el objeto esta colocado i hay una dropArea asignada
        {
            DropArea drop = CurrentDropArea.GetComponent<DropArea>(); //Obtiene el componente DropArea del objeto

            if(drop != null)  //Verifica que se haya encontrado un objeto DropArea
            {
                drop.occupied = false; //Si existe, entonces desocupala para que se pueda poner otro objeto
            }
            placed = false; //Marca que el objeto no esta colocado
            CurrentDropArea = null; //El objeto ya no tiene dropArea
            progressManager.objectsRemaining();
        }
        offset = transform.position - MouseWorldPosition(); //Calcula la distancia del objeto y del mouse
        transform.GetComponent<Collider>().enabled = false; //Desactiva el collider del objeto
    
    }

    //cuando arrastras el mouse
    void OnMouseDrag()
    {
        transform.position = MouseWorldPosition() + offset; //Mover el objeto a la posicion del mouse en el mundo
    }

    //al soltar el mouse
    void OnMouseUp()
    {
        var rayOrigin = Camera.main.transform.position;
        var rayDirection = MouseWorldPosition() - Camera.main.transform.position;

        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo))
        {
            if (hitInfo.transform.CompareTag(destinationTag))
            {
                DropArea drop = hitInfo.transform.GetComponent<DropArea>();

                if (drop != null && !drop.occupied) // Si no está ocupado
                {
                    transform.position = hitInfo.transform.position;

                    CurrentDropArea = hitInfo.transform; // Actualizamos referencia a la DropArea


                    drop.occupied = true; //Marcamos la DropArea ocupada

                    if (!placed)
                    {
                        placed = true;
                        progressManager.objectsRemaining();
                    }
                }
                else
                {
                    transform.position = initialPosition; //Si toca una area ocupada, vuelve a su sitio (no permite 2 objetos en una misma area)
                }
            }
            else
            {
                transform.position = initialPosition; // Si no tocó ninguna DropArea
            }
        }

        GetComponent<Collider>().enabled = true;
    }


    //Passa la posicion del mouse a coordenadas del mundo
    Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition; //Obtiene la pos del mouse
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z; //Deja la z estatica
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);  //Convierte la pos del mouse a mundo
    }

    
}
