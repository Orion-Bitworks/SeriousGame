using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    [SerializeField] float magnitude = 1f;
    [SerializeField] float speed = 5f;

    private Vector3 startPos;
    private Vector2 screenCenter;

    void Start()
    {
        startPos = transform.localPosition;
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        Vector2 mouseOffset = (Vector2)Input.mousePosition - screenCenter;

        mouseOffset.x /= screenCenter.x;
        mouseOffset.y /= screenCenter.y;

        Vector3 targetPos = startPos + new Vector3(mouseOffset.x * magnitude, mouseOffset.y * magnitude, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * speed);
    }
}
