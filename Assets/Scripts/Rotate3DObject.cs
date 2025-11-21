using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rotate3DObject : MonoBehaviour
{
    private InputManager inputManager;

    private bool rotationAllowed;
    private Camera currentCamera;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool invertedControl = false;

    private void Awake()
    {
        if (InputManager.instance == null)
        {
            StartCoroutine(WaitForManagerToInitialize());
        }
        else
        {
            InitializeLeftClickInput();
        }
    }

    private IEnumerator WaitForManagerToInitialize()
    {
        Debug.Log("Waiting for Input Manager...");

        yield return new WaitUntil(() => InputManager.instance != null);

        Debug.Log("Input Manager created!");

        InitializeLeftClickInput();
    }


    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        currentCamera = Camera.main;
    }

    private void Update()
    {
        if (!rotationAllowed)
        {
            return;
        }

        Vector2 mouseDelta = GetMouseLookInput();

        mouseDelta *= rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up * (invertedControl ? 1 : -1), mouseDelta.x, Space.World);
        transform.Rotate(Vector3.right * (invertedControl ? -1 : 1), mouseDelta.y, Space.World);
    }

    private void InitializeLeftClickInput()
    {
        inputManager = InputManager.instance;

        if (inputManager.leftClick_ia != null)
        {
            inputManager.leftClick_ia.started += OnLeftClickPressed;
            inputManager.leftClick_ia.performed += OnLeftClickPressed;
            inputManager.leftClick_ia.canceled += OnLeftClickPressed;
        }
    }

    protected virtual void OnLeftClickPressed(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            rotationAllowed = true;
        }
        else if (context.canceled)
        {
            rotationAllowed = false;
        }
    }

    protected virtual Vector2 GetMouseLookInput()
    {
        if (inputManager.mouseLook_ia != null)
        {
            return inputManager.mouseLook_ia.ReadValue<Vector2>();
        }

        return Vector2.zero;
    }
}