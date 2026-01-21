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
    private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    private Vector3 originalScale;

    private void Start()
    {
        controller = GetComponent<PieceController>();
        originalMaterial = GetComponent<MeshRenderer>().material;
        moveControl = GetComponent<Move3DObject>();
        originalScale = transform.localScale;
    }

    // Cuando se pulsa el boton
    public void OnPointerDown(PointerEventData eventData)
    {
        //transform.localScale = originalScale;
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
        //transform.localScale = hoverScale;
    }

    // Cuando se deja de hacer hover
    public void OnPointerExit(PointerEventData eventData)
    {
        //transform.localScale = originalScale;
    }
}
