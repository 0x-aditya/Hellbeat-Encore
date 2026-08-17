using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;

    public float detectionRange = 8f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;


    private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );


        if (distance <= detectionRange)
        {
            ChasePlayer(distance);
        }
        else
        {
            SetAnimationSpeed(0);
        }
    }



    void ChasePlayer(float distance)
    {

        if (distance > attackRange)
        {
            Vector3 direction =
                player.position - transform.position;


            direction.y = 0;


            transform.rotation =
                Quaternion.LookRotation(direction);


            transform.position +=
                direction.normalized *
                moveSpeed *
                Time.deltaTime;


            SetAnimationSpeed(1);
        }
        else
        {
            Attack();
        }

    }



    void Attack()
    {
        Debug.Log("Enemy Attacks!");
    }



    void SetAnimationSpeed(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }
}