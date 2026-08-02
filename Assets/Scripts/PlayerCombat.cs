using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        AttackInput();
    }


    void AttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("PunchLeft");
        }


        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("PunchRight");
        }
    }
}