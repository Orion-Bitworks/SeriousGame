using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvasOnScreen : MonoBehaviour
{
    [SerializeField] Vector2 screenOffset = new Vector2(0.5f, 0.5f);

    void Update()
    {
        float x = screenOffset.x * Screen.width;
        float y = screenOffset.y * Screen.height;
        Vector3 screenPos = new Vector3 (x, y, 3f);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
    }
}
