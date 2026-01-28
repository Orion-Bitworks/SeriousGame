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
        piece = GetComponent<PieceController>();

        //Cursor.lockState = CursorLockMode.Locked;
        currentCamera = Camera.main;
        

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
            RotateObject();
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

    protected virtual void RotateObject()
    {
        Vector2 mouseDelta = GetMouseLookInput();

        mouseDelta *= rotationSpeed * Time.deltaTime;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }

        if (inputManager.leftClick_ia.inProgress)
        {
            transform.Rotate(Vector3.up * (invertedControl ? 1 : -1), mouseDelta.x, Space.World);
            transform.Rotate(Vector3.right * (invertedControl ? -1 : 1), mouseDelta.y, Space.World);
        }

        if (inputManager.rightClick_ia.inProgress)
        {
            transform.Rotate(Vector3.forward * (invertedControl ? 1 : -1), mouseDelta.x, Space.World);
            transform.Rotate(Vector3.right * (invertedControl ? -1 : 1), mouseDelta.y, Space.World);
        }
    }

    private void InitializeClickInput()
    {
        inputManager = InputManager.instance;

        if (inputManager.rotateMode_ia != null)
        {
            inputManager.rotateMode_ia.canceled += OnRotationCancelled;
        }
    }

    private void OnDestroy()
    {
        inputManager.rotateMode_ia.canceled -= OnRotationCancelled;
    }

    protected virtual void OnRotationCancelled(InputAction.CallbackContext context)
    {
        piece.DisableControls();
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