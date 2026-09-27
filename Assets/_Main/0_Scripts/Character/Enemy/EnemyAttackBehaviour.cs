using UnityEngine;

public class EnemyAttackBehaviour : StateMachineBehaviour
{
    [Range(0f, 1f)]
    public float hitTime = 0.3f;

    private bool hasHit;

    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        hasHit = false;
    }

    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (!hasHit && stateInfo.normalizedTime >= hitTime)
        {
            hasHit = true;

            EnemyAttackHitbox hitbox =
                animator.GetComponent<EnemyAttackHitbox>();

            if (hitbox != null)
            {
                hitbox.DealDamage();
            }
        }
    }
}