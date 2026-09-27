using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

namespace ScriptLibrary.InputManager
{
    public abstract class InputReader : MonoBehaviour
    {
        public List<InputAction> playerMovementAction;

        /// <summary>
        /// To be used in a MonoBehaviour Object and called by other objects for accessing variables
        /// </summary>
        /// <param name="reference"> Input key to be monitored </param>
        /// <param name="callbackContextPerformedFunction"> function that's called during input </param>
        /// <param name="callbackContextCanceledFunction"> function that's called when input is canceled, optional </param>
        /// <example>
        /// <code>
        /// [SerializeField] private InputActionReference move;
        /// public Vector2 MovementInput;
        /// 
        /// RegisterAction(
        ///     reference: move,
        ///     callbackContextFunction: ctx => Move = ctx.ReadValue<Vector2>());
        /// </code>
        /// </example>
        protected virtual void RegisterAction(InputActionReference reference,
            Action<InputAction.CallbackContext> callbackContextPerformedFunction, Action<InputAction.CallbackContext> callbackContextCanceledFunction = null)
        {
            InputAction action = reference.action; // get the action from the action reference

            //enable and add the fun
            action.Enable();
            action.performed += callbackContextPerformedFunction;
            action.canceled += callbackContextCanceledFunction;

            //add to the list for disabling later
            playerMovementAction.Add(reference.action);
        }


        protected virtual void OnDisable()
        {
            //loop through function and disable all the 
            for (int i = 0; i < playerMovementAction.Count; i++)
            {
                playerMovementAction[i].Disable();
            }
        }

        protected void CallDelegatesOnClick(InputAction.CallbackContext callbackContext, Action clickActionDown,
            Action clickActionUp)
        {
            if (callbackContext.control.IsPressed())
            {
                clickActionDown?.Invoke();
            }
            else
            {
                clickActionUp?.Invoke();
            }
        }
    }
}
