using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    private Vector3 direction;

    // Call this when spawning the projectile
    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
        transform.forward = direction; // face the direction
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
