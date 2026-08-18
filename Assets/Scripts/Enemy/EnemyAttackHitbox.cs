using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private int damage = 10;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    public void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRadius,
            playerLayer
        );

        foreach (Collider hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                continue;
            }

            PerfectBlock perfectBlock = hit.GetComponent<PerfectBlock>();

            // Player successfully blocked the attack
            if (perfectBlock != null && perfectBlock.IsBlockAvailable())
            {
                Debug.Log("ATTACK BLOCKED!");

                perfectBlock.CloseBlockWindow();

                return;
            }

            // Player did not block
            playerHealth.TakeDamage(damage);

            Debug.Log("Enemy hit player for " + damage + " damage!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRadius
        );
    }
}