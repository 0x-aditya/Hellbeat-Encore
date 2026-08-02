using UnityEngine.InputSystem;

namespace ScriptLibrary.Inputs
{
    public abstract class KeyPressInput : BaseInput
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerMovementAction.canceled += OnInput;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerMovementAction.canceled -= OnInput;
        }

        protected virtual void OnKeyDown(){}
        protected virtual void OnKeyUp(){}

        protected override void OnInput(InputAction.CallbackContext context)
        {
            if (context.control.IsPressed())
            {
                OnKeyDown();
            }
            else
            {
                OnKeyUp();
            }
        }
    }
}