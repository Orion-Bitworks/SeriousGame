using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ROTATION_STATE { STATIC, ROTATING_X, ROTATING_Y }

public class Rotate3DObject : MonoBehaviour
{
    private InputManager inputManager;

    bool selected = false;
    PieceController piece;

    private ROTATION_STATE state = ROTATION_STATE.STATIC;

    Vector3 firstPos;
    float minDistance = 20;

    private bool dragStarted = false;

    int basicDirection;

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

            if (dragStarted)
            {
                dragStarted = false;
            }

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

        int directionMultiplier = 0;

        piece.GetGroup().CanMove(false);

        if (inputManager.leftClick_ia.triggered || inputManager.rightClick_ia.triggered)
        {
            firstPos = GetMousePosInScreen();
            AudioController.Instance.PlaySFX(SFX.ThreeD, (int)ThreeDSFX.Rotate);
        }

        dragPos = GetMousePosInScreen();
        distance = Vector3.Distance(firstPos, dragPos);

        if (inputManager.leftClick_ia.inProgress)
        {
            variableAxis = Vector3.up;
        }
        else if (inputManager.rightClick_ia.inProgress)
        {
            variableAxis = Vector3.forward;
        }
        else
        {
            return;
        }

        CursorController.instance.ChangeCursorState(CursorController.CURSOR_STATE.ROTATING);

        if (!dragStarted)
        {
            if (distance <= minDistance)
            {
                return;
            }

            dragStarted = true;
        }

        Quaternion finalRotation = Quaternion.identity;

        Vector3 checkAxis = firstPos - dragPos;

        float posX = Mathf.Abs(checkAxis.x);
        float posY = Mathf.Abs(checkAxis.y);
        // DEcide estado dependiendo de en que eje se ha movido mas el mouse

        //Debug.Log("MouseDelta" + mouseDelta);
        //Debug.Log("MouseDelta" + mouseDelta.magnitude);
        //Debug.Log("checkAxis" + checkAxis);
        Vector3 direction = dragPos.normalized;

        if (state != ROTATION_STATE.STATIC && directionMultiplier != basicDirection)
        {
            ResetRotationState(dragPos, directionMultiplier);
        }

        switch (state)
        {
            case ROTATION_STATE.STATIC:
                if (posX > posY)
                {
                    state = ROTATION_STATE.ROTATING_X;
                }
                else
                {
                    state = ROTATION_STATE.ROTATING_Y;
                }
                break;
            case ROTATION_STATE.ROTATING_X:

                if (Mathf.Abs(mouseDelta.y) > Mathf.Abs(mouseDelta.x))
                {
                    ResetRotationState(dragPos, directionMultiplier);
                    state = ROTATION_STATE.ROTATING_Y;
                }
                else
                {
                    UpdateDirectionMultiplierX(direction, ref directionMultiplier);

                    finalRotation = Quaternion.AngleAxis(mouseDelta.x * -1, variableAxis);
                }

                break;
            case ROTATION_STATE.ROTATING_Y:
                if (Mathf.Abs(mouseDelta.x) > Mathf.Abs(mouseDelta.y))
                {
                    ResetRotationState(dragPos, directionMultiplier);
                    state = ROTATION_STATE.ROTATING_X;
                }
                else
                {
                    UpdateDirectionMultiplierY(direction, ref directionMultiplier);

                    finalRotation = Quaternion.AngleAxis(mouseDelta.y, Vector3.right);
                }
                break;
        }

        piece.GetGroup().RotatePiece(piece.GetGroup().GetCentralPivot(), finalRotation);
    }

    private void UpdateDirectionMultiplierY(Vector3 direction, ref int directionMultiplier)
    {
        if (direction.y > 0)
        {
            directionMultiplier = 1;
        }
        else if (direction.y < 0)
        {
            directionMultiplier = -1;
        }
        else
        {
            directionMultiplier = 0;
        }
    }

    void UpdateDirectionMultiplierX
        (Vector3 direction, ref int directionMultiplier)
    {
        if (direction.x > 0)
        {
            directionMultiplier = 1;
        }
        else if (direction.x < 0)
        {
            directionMultiplier = -1;
        }
        else
        {
            directionMultiplier = 0;
        }

    }

    private void ResetRotationState(Vector3 dragPos, int directionId)
    {
        state = ROTATION_STATE.STATIC;
        firstPos = dragPos;
        basicDirection = directionId;

        dragStarted = false;
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
        state = ROTATION_STATE.STATIC;
        dragStarted = false;
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