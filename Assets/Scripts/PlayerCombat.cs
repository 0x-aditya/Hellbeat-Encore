using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement movement;

    public bool isAttacking = false;

    [SerializeField] private float attackCooldown = 0.05f;

    private float lastAttackTime;

    private void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        AttackInput();
    }

    private void AttackInput()
    {
        if (isAttacking)
            return;

        if (Time.time - lastAttackTime < attackCooldown)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("PunchLeft");
            lastAttackTime = Time.time;
        }

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("PunchRight");
            lastAttackTime = Time.time;
        }
    }
}