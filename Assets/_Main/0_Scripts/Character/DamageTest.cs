using UnityEngine;

public class DamageTest : MonoBehaviour
{

    public PlayerHealth playerHealth;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            playerHealth.TakeDamage(10);
        }
    }
}