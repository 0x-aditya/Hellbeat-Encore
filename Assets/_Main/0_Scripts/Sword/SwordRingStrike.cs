using System;
using System.Collections.Generic;
using UnityEngine;

public class SwordRingStrike : MonoBehaviour
{
    [Header("Ring")]
    [SerializeField] private Transform center;
    [SerializeField] private float ringRadius = 2.5f;
    [SerializeField, Range(4, 64)] private int pointCount = 24;
    [SerializeField] private float pointRadius = 0.3f;
    [SerializeField] private float heightOffset = 0.5f;

    [Header("Targets")]
    [SerializeField] private LayerMask enemyMask = ~0;

    [Header("Effects")]
    [SerializeField] private ParticleSystem bloodVfx;
    [SerializeField, Min(1)] private int maxVfxPerEnemy = 4;

    [Header("Optional launch")]
    [SerializeField] private float launchVelocity = 0f;

    public event Action<Collider, Vector3> TargetStruck;

    private readonly Collider[] _buffer = new Collider[16];
    private readonly HashSet<Collider> _struck = new HashSet<Collider>();
    private readonly HashSet<Rigidbody> _launched = new HashSet<Rigidbody>();
    private readonly Dictionary<Collider, int> _vfxCounts = new Dictionary<Collider, int>();

    private Transform Center => center != null ? center : transform;

    private Vector3 GetPoint(int i)
    {
        float angle = i / (float)pointCount * Mathf.PI * 2f;
        Vector3 local = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * ringRadius;
        return Center.position + Center.rotation * local + Vector3.up * heightOffset;
    }

    public void Strike()
    {
        _struck.Clear();
        _launched.Clear();
        _vfxCounts.Clear();

        Vector3 origin = Center.position;

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 point = GetPoint(i);
            int count = Physics.OverlapSphereNonAlloc(point, pointRadius, _buffer);

            for (int j = 0; j < count; j++)
            {
                Collider enemy = _buffer[j];
                if (!enemy.CompareTag("Enemy"))
                    continue;
                Vector3 contact = GetContactPoint(enemy, point);

                Vector3 dir = contact - origin;
                dir.y = Mathf.Max(dir.y, 0.2f);

                if (_struck.Add(enemy))
                {
                    TargetStruck?.Invoke(enemy, contact);
                }

                SpawnBlood(enemy, contact, dir);
            }
        }
    }
    private static Vector3 GetContactPoint(Collider col, Vector3 from)
    {
        // Types that support ClosestPoint
        if (col is BoxCollider || col is SphereCollider || col is CapsuleCollider
            || (col is MeshCollider mc && mc.convex))
            return col.ClosestPoint(from);

        // Fallback for non-convex meshes: ray toward the collider's center
        Vector3 toCenter = col.bounds.center - from;
        float dist = toCenter.magnitude;
        if (dist > 0.0001f &&
            col.Raycast(new Ray(from, toCenter / dist), out RaycastHit hit, dist + col.bounds.extents.magnitude))
            return hit.point;

        // Last resort: closest point on the bounding box
        return col.bounds.ClosestPoint(from);
    }
    private void SpawnBlood(Collider enemy, Vector3 position, Vector3 direction)
    {
        if (bloodVfx == null) return;

        _vfxCounts.TryGetValue(enemy, out int spawned);
        if (spawned >= maxVfxPerEnemy) return;
        _vfxCounts[enemy] = spawned + 1;

        ParticleSystem p = Instantiate(bloodVfx, position, Quaternion.LookRotation(direction.normalized));
        p.Play();

        var main = p.main;
        Destroy(p.gameObject, main.duration + main.startLifetime.constantMax);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.15f, 0.1f, 0.9f);
        for (int i = 0; i < pointCount; i++)
            Gizmos.DrawWireSphere(GetPoint(i), pointRadius);
    }
}