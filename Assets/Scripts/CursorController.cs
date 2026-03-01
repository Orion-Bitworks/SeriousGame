using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D defaultTexture;
    [SerializeField] private Texture2D movementTexture;
    [SerializeField] private Texture2D rotationTexture;
    [SerializeField] private Texture2D separatingTexture;

    [SerializeField] private Vector2 clickPoistion = Vector2.zero;

    private void Start()
    {
        Cursor.SetCursor(defaultTexture, clickPoistion, CursorMode.Auto);
    }
}
