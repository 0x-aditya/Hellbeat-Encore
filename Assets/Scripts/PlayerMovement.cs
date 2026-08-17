using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;


    [Header("Camera")]
    public Transform cameraTransform;


    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;

    public bool canMove = true;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        MovePlayer();
    }


    void MovePlayer()
    {
        if (!canMove)
        {
            animator.SetFloat("Speed", 0);

            if (controller.isGrounded && velocity.y < 0)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            return;
        }


        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        // Normal WASD movement relative to player
        Vector3 moveDirection =
            transform.forward * vertical +
            transform.right * horizontal;


        moveDirection.Normalize();


        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed = isRunning ? runSpeed : walkSpeed;


        controller.Move(
            moveDirection * currentSpeed * Time.deltaTime
        );


        if (moveDirection.magnitude > 0.1f)
        {
            // Only rotate when moving forward
            if (vertical > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * 100 * Time.deltaTime
                );
            }
        }


        // Animation speed
        animator.SetFloat(
            "Speed",
            moveDirection.magnitude * currentSpeed
        );


        // Gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}