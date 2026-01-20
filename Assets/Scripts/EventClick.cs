using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Material clickMaterial;
    Material originalMaterial;

    private Move3DObject moveControl;
    private PieceController controller;

    private void Start()
    {
        controller = GetComponent<PieceController>();
        originalMaterial = GetComponent<MeshRenderer>().material;
        moveControl = GetComponent<Move3DObject>();
    }

    // Cuando se pulsa el boton
    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<MeshRenderer>().material = clickMaterial;
        controller.CanSnap(true);
        moveControl.EnableRigidBody();
        moveControl.EnableMovement();
    }

    // Cuando se suelta el boton
    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponent<MeshRenderer>().material = originalMaterial;
        controller.CanSnap(false);
        moveControl.DisableRigidBody();
        moveControl.DisableMovement();
    }

    // Cuando se hace click
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    // Cuando se hace hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    // Cuando se deja de hacer hover
    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
