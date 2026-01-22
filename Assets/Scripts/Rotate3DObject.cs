using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rotate3DObject : MonoBehaviour
{
    private InputManager inputManager;

    private bool rotationXYAllowed;
    private bool rotationZAllowed;
    private Camera currentCamera;
    private Vector3 previousPosition;

    bool selected = false;
    PieceController piece;

    [SerializeField] private float currentFov;
    [SerializeField] private float originalFov;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool invertedControl = false;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float maxZoomIn = 10f;
    [SerializeField] private float maxZoomOut = 20f;

    private void Awake()
    {
        if (InputManager.instance == null)
        {
            StartCoroutine(WaitForManagerToInitialize());
        }
        else
        {
            InitializeClickInput();
        }
    }

    private IEnumerator WaitForManagerToInitialize()
    {
        Debug.Log("Waiting for Input Manager...");

        yield return new WaitUntil(() => InputManager.instance != null);

        Debug.Log("Input Manager created!");

        InitializeClickInput();
    }


    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        currentCamera = Camera.main;
        piece = GetComponent<PieceController>();

        originalFov = currentCamera.fieldOfView;
        currentFov = originalFov;
        maxZoomOut += originalFov;
        maxZoomIn = originalFov - maxZoomIn;
    }

    private void Update()
    {
        if (!selected || piece.HasSnapped())
        {
            return;
        }

        /*if (!rotationXYAllowed && !rotationZAllowed)
        {
            return;
        }*/


        if (inputManager.rotateMode_ia.inProgress)
        {
            Vector2 mouseDelta = GetMouseLookInput();

            mouseDelta *= rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up * (invertedControl ? 1 : -1), mouseDelta.x, Space.World);
            transform.Rotate(Vector3.right * (invertedControl ? -1 : 1), mouseDelta.y, Space.World);
            transform.position = previousPosition;
        }
        
        /*switch (rotationXYAllowed, rotationZAllowed)
        {
            case (true, false):
                transform.Rotate(Vector3.up * (invertedControl ? 1 : -1), mouseDelta.x, Space.World);
                transform.Rotate(Vector3.right * (invertedControl ? -1 : 1), mouseDelta.y, Space.World);
                transform.position = previousPosition;
                break;
            case (false, true):
                transform.Rotate(Vector3.forward * (invertedControl ? 1 : -1), mouseDelta.x, Space.World);
                transform.Rotate(Vector3.right * (invertedControl ? -1 : 1), mouseDelta.y, Space.World);
                transform.position = previousPosition;
                break;
            case (true, true):

                currentFov += mouseDelta.y;

                if (currentFov > maxZoomOut)
                {
                    currentFov = maxZoomOut;
                }
                
                if (currentFov < maxZoomIn)
                {
                    currentFov = maxZoomIn;
                }

                currentCamera.fieldOfView = currentFov;

                break;
        }*/
    }

    private void InitializeClickInput()
    {
        inputManager = InputManager.instance;

        if (inputManager.leftClick_ia != null)
        {
            inputManager.leftClick_ia.started += OnLeftClickPressed;
            inputManager.leftClick_ia.performed += OnLeftClickPressed;
            inputManager.leftClick_ia.canceled += OnLeftClickPressed;
        }

        if (inputManager.rightClick_ia != null)
        {
            inputManager.rightClick_ia.started += OnRightClickPressed;
            inputManager.rightClick_ia.performed += OnRightClickPressed;
            inputManager.rightClick_ia.canceled += OnRightClickPressed;
        }
    }

    protected virtual void OnLeftClickPressed(InputAction.CallbackContext context)
    {
        previousPosition = transform.position;

        if (context.started || context.performed)
        {
            rotationXYAllowed = true;
        }
        else if (context.canceled)
        {
            rotationXYAllowed = false;
        }
    }

    protected virtual void OnRightClickPressed(InputAction.CallbackContext context)
    {
        previousPosition = transform.position;

        if (context.started || context.performed)
        {
            rotationZAllowed = true;
        }
        else if (context.canceled)
        {
            rotationZAllowed = false;
        }
    }

    public void EnableRotation()
    {
        selected = true;
    }

    public void DisableRotation()
    {
        selected = false;
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