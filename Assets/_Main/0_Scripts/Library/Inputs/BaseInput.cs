using UnityEngine;
using UnityEngine.InputSystem;

namespace ScriptLibrary.Inputs
{
    public abstract class BaseInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputAction;
        protected InputAction PlayerMovementAction;

        protected virtual void Awake()
        {
            PlayerMovementAction = inputAction.action;
            if (PlayerMovementAction == null)
            {
                Debug.LogError("playerMovementAction is null");
            }
        }

        protected virtual void OnEnable()
        {
            PlayerMovementAction.Enable();
            PlayerMovementAction.performed += OnInput;
            PlayerMovementAction.canceled += OnInput;
        }

        protected virtual void OnDisable()
        {
           
            PlayerMovementAction.performed -= OnInput;
            PlayerMovementAction.canceled -= OnInput;
        }

        protected abstract void OnInput(InputAction.CallbackContext context);

    }
}