using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : ScriptLibrary.Inputs.Vector2Input
{
    private static readonly int XMovement = Animator.StringToHash("XMovement");

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;

    private Animator animator;
    private CharacterController _controller;
    private Vector3 _velocity;
    private float Horizontal => VectorInput.x;
    private float Vertical => VectorInput.y;

    [NonSerialized] public bool canMove = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("No animator component found for: " + gameObject.name);
        }

        if (_cameraTransform == null)
        {
            Debug.LogError("No camera transform found for: " + gameObject.name);
        }
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (!canMove)
        {
            animator.SetFloat(XMovement, 0f);
            ApplyGravity();
            return;
        }

        // Get the direction the camera is facing
        Vector3 cameraForward = _cameraTransform.forward;
        Vector3 cameraRight = _cameraTransform.right;

        // Ignore the camera's vertical rotation
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Camera-relative movement
        Vector3 moveDirection =cameraForward * Vertical + cameraRight * Horizontal;

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed =isRunning ? runSpeed : walkSpeed;

        // Move
        _controller.Move(moveDirection *currentSpeed *Time.deltaTime);

        // Rotate character toward movement
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =Quaternion.LookRotation(moveDirection);

            transform.rotation =Quaternion.Slerp(transform.rotation,targetRotation,rotationSpeed * Time.deltaTime);
        }

        // Animation
        animator.SetFloat(XMovement,moveDirection.magnitude);

        ApplyGravity();
    }

    void ApplyGravity()
    {
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime
        );
    }
}