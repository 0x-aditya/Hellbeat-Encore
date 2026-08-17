using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 25;

    public float attackRange = 1.5f;

    public LayerMask enemyLayer;


    public void DealDamage()
    {
        Collider[] enemies = Physics.OverlapSphere(
            transform.position,
            attackRange,
            enemyLayer
        );


        foreach (Collider enemy in enemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
   


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}