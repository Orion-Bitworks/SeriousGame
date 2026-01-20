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

    private void Start()
    {
        originalMaterial = GetComponent<MeshRenderer>().material;
        moveControl = GetComponent<Move3DObject>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<MeshRenderer>().material = clickMaterial;
        moveControl.EnableRigidBody();
        moveControl.EnableMovement();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponent<MeshRenderer>().material = originalMaterial;
        moveControl.DisableRigidBody();
        moveControl.DisableMovement();
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
