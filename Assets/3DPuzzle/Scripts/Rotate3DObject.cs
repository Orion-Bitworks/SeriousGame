using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rotate3DObject : MonoBehaviour
{
    private InputManager inputManager;

    bool selected = false;
    PieceController piece;

    Vector3 firstPos;
    float minDistance = 25;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;

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
    }

    private void Update()
    {
        if (!selected)
        {
            return;
        }

        if (inputManager.rotateMode_ia.inProgress)
        {
            RotateObject();
        }
        else if (!inputManager.rotateMode_ia.inProgress && !piece.GetGroup().CanMove())
        {
            piece.GetGroup().CanMove(true);
        }
    }

    public Vector3 GetMousePosInScreen()
    {
        return Input.mousePosition;
    }

    public void RotateObject()
    {
        Vector2 mouseDelta = GetMouseLookInput();

        mouseDelta *= rotationSpeed * Time.deltaTime;

        Vector3 variableAxis;

        Vector3 dragPos;

        float distance = 0;

        piece.GetGroup().CanMove(false);

        if (inputManager.leftClick_ia.triggered || inputManager.rightClick_ia.triggered)
        {
            firstPos = GetMousePosInScreen();
            //Debug.Log("Trigger Mouse Pos: " + firstPos);
        }

        if (inputManager.leftClick_ia.inProgress)
        {
            dragPos = GetMousePosInScreen();
            //Debug.Log("Dragging Mouse Pos: " + dragPos);
            distance = Vector3.Distance(firstPos, dragPos);

            variableAxis = Vector3.up;
        }
        else if (inputManager.rightClick_ia.inProgress)
        {
            dragPos = GetMousePosInScreen();
            distance = Vector3.Distance(firstPos, dragPos);

            variableAxis = Vector3.forward;
        }
        else
        {
            return;
        }

        if (distance >= minDistance)
        {
            Quaternion finalRotation = Quaternion.identity;

            Vector3 checkAxis = firstPos - dragPos;

            float posX = Mathf.Abs(checkAxis.x);
            float posY = Mathf.Abs(checkAxis.y);

            if (posX > posY)
            {
                finalRotation = Quaternion.AngleAxis(mouseDelta.x * -1, variableAxis);
            }
            else if (posY > posX)
            {
                finalRotation = Quaternion.AngleAxis(mouseDelta.y, Vector3.right);
            }

            piece.GetGroup().RotatePiece(piece.GetGroup().GetCentralPivot(), finalRotation);
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