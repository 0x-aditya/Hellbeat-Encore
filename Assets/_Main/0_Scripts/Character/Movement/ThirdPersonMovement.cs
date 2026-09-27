using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 12f;
    public float gravity = -9.81f;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Animation")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;

    public bool canMove = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
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
            animator.SetFloat("Speed", 0f);
            ApplyGravity();
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Get the direction the camera is facing
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Ignore the camera's vertical rotation
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Camera-relative movement
        Vector3 moveDirection =cameraForward * vertical +cameraRight * horizontal;

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed =isRunning ? runSpeed : walkSpeed;

        // Move
        controller.Move(moveDirection *currentSpeed *Time.deltaTime);

        // Rotate character toward movement
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =Quaternion.LookRotation(moveDirection);

            transform.rotation =Quaternion.Slerp(transform.rotation,targetRotation,rotationSpeed * Time.deltaTime);
        }

        // Animation
        animator.SetFloat("Speed",moveDirection.magnitude);

        ApplyGravity();
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime
        );
    }
}