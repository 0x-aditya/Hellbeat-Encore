using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRadius = 1.5f;
    public int damage = 10;

    [Header("Layers")]
    public LayerMask playerLayer;

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

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Enemy hit player for " + damage + " damage!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}