using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float detectionRange = 8f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;

    [Header("Combat")]
    public int attackDamage = 10;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (BeatHandler.Instance != null)
        {
            BeatHandler.Instance.OnBeat += OnBeat;
        }
    }

    private void OnDisable()
    {
        if (BeatHandler.Instance != null)
        {
            BeatHandler.Instance.OnBeat -= OnBeat;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(
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

    private void ChasePlayer(float distance)
    {
        if (distance > attackRange)
        {
            Vector3 direction = player.position - transform.position;

            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            transform.position +=
                direction.normalized *
                moveSpeed *
                Time.deltaTime;

            SetAnimationSpeed(1);
        }
        else
        {
            // Stop moving when close enough to attack.
            SetAnimationSpeed(0);

            FacePlayer();
        }
    }

    private void OnBeat()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // Enemy can ONLY attack on a beat.
        if (distance <= attackRange)
        {
            Attack();
        }
    }

    private void Attack()
    {
        Debug.Log(gameObject.name + " ATTACKS ON BEAT!");

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        PerfectBlock playerBlock = player.GetComponent<PerfectBlock>();

        if (playerBlock != null)
        {
            playerBlock.OpenBlockWindow();
        }

        if (PerfectBlockUI.Instance != null)
        {
            PerfectBlockUI.Instance.ShowPrompt();
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void SetAnimationSpeed(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }
}