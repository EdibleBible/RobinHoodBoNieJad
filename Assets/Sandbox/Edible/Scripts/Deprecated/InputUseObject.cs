using UnityEngine;
using UnityEngine.InputSystem;

public class InputUseObject : MonoBehaviour
{
    public InputActionAsset globalInputActions;
    private InputAction useObject;

    private void Start()
    {
        useObject = globalInputActions.FindAction("UseObject");
        useObject.Enable();
        useObject.started += InputActionUseObject;
    }

    private void OnDisable()
    {
        useObject.started -= InputActionUseObject;
        useObject.Disable();
    }

    public void InputActionUseObject(InputAction.CallbackContext context)
    {
        gameObject.GetComponent<IUseObject>().UseObject();
    }
}
