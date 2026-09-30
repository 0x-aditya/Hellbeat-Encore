using System;
using UnityEngine;

public class SwordCollisionEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        { 
            print("collided with: " + other.name);
        }
    }
}
