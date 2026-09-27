using System;
using UnityEngine;

public class SwordHitParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particle;
    private void OnCollisionEnter(Collision collision)
    {
        var contact = collision.contacts[0];
        var particle = Instantiate(_particle, contact.point, Quaternion.LookRotation(contact.normal));
        particle.Play();
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }
}
