using UnityEngine;

public class PunchStateBehaviour : StateMachineBehaviour
{
    public float hitTime = 0.3f;

    private bool hasHit;


    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        hasHit = false;


        PlayerMovement movement = animator.GetComponent<PlayerMovement>();

        PlayerCombat combat = animator.GetComponent<PlayerCombat>();


        if (movement != null)
            movement.canMove = false;


        if (combat != null)
            combat.isAttacking = true;
    }


    public override void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        float normalizedTime = stateInfo.normalizedTime % 1;


        if (normalizedTime >= hitTime && !hasHit)
        {
            AttackHitbox attack =
                animator.GetComponentInChildren<AttackHitbox>();

            if (attack != null)
            {
                attack.DealDamage();
            }


            hasHit = true;
        }
    }


    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        PlayerMovement movement = animator.GetComponent<PlayerMovement>();

        PlayerCombat combat = animator.GetComponent<PlayerCombat>();


        if (movement != null)
            movement.canMove = true;


        if (combat != null)
            combat.isAttacking = false;
    }
}