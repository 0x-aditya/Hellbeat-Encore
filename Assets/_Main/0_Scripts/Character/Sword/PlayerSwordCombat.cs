using UnityEngine;

public class PlayerSwordCombat : MonoBehaviour
{
    private Animator animator;

    [Header("Sword Attack")]
    [SerializeField] private float attackCooldown = 0.28f;

    private float lastAttackTime;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        AttackInput();
    }

    private void AttackInput()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("SlayerSword");

            lastAttackTime = Time.time;
        }
    }
}