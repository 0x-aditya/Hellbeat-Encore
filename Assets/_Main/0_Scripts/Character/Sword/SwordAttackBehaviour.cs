using UnityEngine;

public class SwordAttackBehaviour : StateMachineBehaviour
{
    [Range(0f, 1f)]
    public float hitTime = 0.3f;

    private bool hasHit;

    public override void OnStateEnter(Animator animator,AnimatorStateInfo stateInfo,int layerIndex)
    {
        hasHit = false;

        PlayerMovement movement = animator.GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.canMove = false;
        }
    }

    public override void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo,int layerIndex)
    {
        if (!hasHit && stateInfo.normalizedTime >= hitTime)
        {
            hasHit = true;

            AttackHitbox hitbox = animator.GetComponent<AttackHitbox>();

            if (hitbox != null)
            {
                hitbox.DealDamage();
            }
        }
    }

    public override void OnStateExit(Animator animator,AnimatorStateInfo stateInfo,int layerIndex)
    {
        PlayerMovement movement = animator.GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.canMove = true;
        }
    }
}