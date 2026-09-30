using System;
using System.Collections;
using FMODUnity;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerAttack : MonoBehaviour
{
    private Animator _animator;
    private PlayerMovement _movement;

    public bool isAttacking = false;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.25f;
    [SerializeField] private int numberOfAnimations = 2;
    [SerializeField] private float swordImpactSphereCastRadius = 2f;
    [SerializeField] private Transform swordImpactCastOrigin;
    [SerializeField] private float launchVelocity = 2f;
    [SerializeField] private GameObject smashParticle;
    [Header("Audio")]
    [SerializeField] private bool playSwordAudio;
    [SerializeField] private EventReference musicEventReference;
    [SerializeField] private GameObject swordCollider;
    [SerializeField] private float colliderExistenceTime;
    private float _lastAttackTime;
    
    private CinemachineImpulseSource  _impulseSource;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        AttackInput();
    }

    private IEnumerator SpawnSwordCollider(int type)
    {
        swordCollider.SetActive(true);
        yield return new WaitForSeconds(colliderExistenceTime);
        swordCollider.SetActive(false);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void AttackInput()
    {
        if (isAttacking)
            return;

        if (Time.time - _lastAttackTime < attackCooldown)
            return;


        if (Input.GetMouseButtonDown(0))
        {
            //RegisterHit.Instance.RegisterHitEvent();
            SwordImpact();
            FindAnyObjectByType<SwordRingStrike>().Strike();
            int random = Random.Range(1, numberOfAnimations + 1);
            _animator.SetTrigger($"Attack{random}");
            StartCoroutine(SpawnSwordCollider(random));
            if (playSwordAudio)
            {
                var musicInstance = RuntimeManager.CreateInstance(musicEventReference);
                musicInstance.start();
            }
            _impulseSource.GenerateImpulse();
            _lastAttackTime = Time.time;
        }


        if (Input.GetMouseButtonDown(1))
        {
            //RegisterHit.Instance.RegisterHitEvent();
            _animator.SetTrigger("Block1");
            _lastAttackTime = Time.time;
        }
    }

    public void SwordImpact()
    {
        var smashParticleInstance = Instantiate(smashParticle,  swordImpactCastOrigin.position, smashParticle.transform.rotation);
        Destroy(smashParticleInstance, 2f);
        // find enemy and stun them slightly
        Collider[] hits = Physics.OverlapSphere(swordImpactCastOrigin.position, swordImpactSphereCastRadius);

        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb == null || rb.isKinematic) continue;

            Vector3 v = rb.linearVelocity;
            v.y = launchVelocity;
            rb.linearVelocity = v;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.25f);
        Gizmos.DrawSphere(swordImpactCastOrigin.position, swordImpactSphereCastRadius);
        Gizmos.color = new Color(1f, 0.3f, 0.1f, 1f);
        Gizmos.DrawWireSphere(swordImpactCastOrigin.position, swordImpactSphereCastRadius);

    }
}

