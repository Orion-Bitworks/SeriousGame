using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public InputActionAsset map;

    public InputAction leftClick_ia;
    public InputAction rightClick_ia;
    public InputAction mouseLook_ia;
    public InputAction mouseWheel_ia;
    public InputAction rotateMode_ia;
    public InputAction separateMode_ia;
    public InputAction deletePiece_ia;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);

        EnableInput();

        leftClick_ia = map.FindActionMap("In3DGame").FindAction("Left Click");
        rightClick_ia = map.FindActionMap("In3DGame").FindAction("Right Click");
        mouseLook_ia = map.FindActionMap("In3DGame").FindAction("Mouse Look");
        mouseWheel_ia = map.FindActionMap("In3DGame").FindAction("Mouse Wheel");
        rotateMode_ia = map.FindActionMap("In3DGame").FindAction("Rotate Mode");
        separateMode_ia = map.FindActionMap("In3DGame").FindAction("Separate Mode");
        deletePiece_ia = map.FindActionMap("In3DGame").FindAction("Delete");
    }

    private void OnEnable()
    {
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    public void EnableInput()
    {
        map.Enable();
    }

    public void DisableInput()
    {
        map.Disable();
    }
}
