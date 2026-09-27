using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement movement;

    public bool isAttacking = false;

    [SerializeField] private float attackCooldown = 0.25f;

    private float lastAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        AttackInput();
    }


    void AttackInput()
    {
        if (isAttacking)
            return;

        if (Time.time - lastAttackTime < attackCooldown)
            return;


        if (Input.GetMouseButtonDown(0))
        {
            //RegisterHit.Instance.RegisterHitEvent();
            animator.SetTrigger("Attack1");
            lastAttackTime = Time.time;
        }


        if (Input.GetMouseButtonDown(1))
        {
            //RegisterHit.Instance.RegisterHitEvent();
            animator.SetTrigger("Block1");
            lastAttackTime = Time.time;
        }
    }
}

