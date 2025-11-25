using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public InputActionAsset map;

    public InputAction leftClick_ia;
    public InputAction rightClick_ia;
    public InputAction mouseLook_ia;

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

        leftClick_ia = map.FindActionMap("MouseControl").FindAction("Left Click");
        rightClick_ia = map.FindActionMap("MouseControl").FindAction("Right Click");
        mouseLook_ia = map.FindActionMap("MouseControl").FindAction("Mouse Look");
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
