using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public static CursorController instance;

    public enum CURSOR_STATE { DEFAULT, MOVING, ROTATING, SEPARATING };

    [SerializeField] private Texture2D defaultTexture;
    [SerializeField] private Texture2D movementTexture;
    [SerializeField] private Texture2D rotationTexture;
    [SerializeField] private Texture2D separatingTexture;

    [SerializeField] private Vector2 clickPoistion = Vector2.zero;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Cursor.SetCursor(defaultTexture, clickPoistion, CursorMode.Auto);
    }

    public void ChangeCursorState(CURSOR_STATE state)
    {
        Vector2 centerClickPosition;

        switch (state)
        {
            case CURSOR_STATE.DEFAULT:
                Cursor.SetCursor(defaultTexture, clickPoistion, CursorMode.Auto);
                break;
            case CURSOR_STATE.MOVING:
                centerClickPosition = new Vector2(movementTexture.width * 0.5f, movementTexture.height * 0.5f);
                Cursor.SetCursor(movementTexture, centerClickPosition, CursorMode.Auto);
                break;
            case CURSOR_STATE.ROTATING:
                centerClickPosition = new Vector2(rotationTexture.width * 0.5f, rotationTexture.height * 0.5f);
                Cursor.SetCursor(rotationTexture, centerClickPosition, CursorMode.Auto);
                break;
            case CURSOR_STATE.SEPARATING:
                centerClickPosition = new Vector2(separatingTexture.width * 0.5f, separatingTexture.height * 0.5f);
                Cursor.SetCursor(separatingTexture, centerClickPosition, CursorMode.Auto);
                break;
        }
    }
}
