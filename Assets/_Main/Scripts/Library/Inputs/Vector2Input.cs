using UnityEngine;
using UnityEngine.InputSystem;

namespace ScriptLibrary.Inputs
{
    public abstract class Vector2Input : BaseInput
    {
        protected Vector2 VectorInput {get; private set; }

        protected override void OnInput(InputAction.CallbackContext context)
        {
            VectorInput = PlayerMovementAction.ReadValue<Vector2>();
        }

    }
}
