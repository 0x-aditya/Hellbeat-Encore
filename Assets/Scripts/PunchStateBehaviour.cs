using UnityEngine;

public class PunchStateBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement movement = animator.GetComponent<PlayerMovement>();
        PlayerCombat combat = animator.GetComponent<PlayerCombat>();

        if (movement != null)
            movement.canMove = false;

        if (combat != null)
            combat.isAttacking = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement movement = animator.GetComponent<PlayerMovement>();
        PlayerCombat combat = animator.GetComponent<PlayerCombat>();

        if (movement != null)
            movement.canMove = true;

        if (combat != null)
            combat.isAttacking = false;
    }
}