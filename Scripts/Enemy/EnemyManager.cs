using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    PlayerManager player;
    Animator anim;
    Rigidbody rb;

    [Header("Stats")]
    public float enemy_Health = 10;
    public float baseDamage = 15;

    [Header("Combat")]
    public float enemy_MovementSpeed = 2f;
    public float enemy_AttackRadius = 1.5f;
    public float attackCooldown = 1.2f;
    private float attackCooldownTimer = 0;
    bool canAttack = true;

    [Header("Combat VFX")]
    public GameObject combatVFX;
    public Transform combatVFX_Point;

    [Header("Death VFX")]
    public GameObject deathVFX;


    private void Awake()
    {
        player = FindObjectOfType<PlayerManager>(); // Finds player
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        EnemyCombat();

        ResetAttack();

        if (enemy_Health <= 0)
        {
            Die();
        }

        if (player == null)
            anim.Play("Dancing");
    }

    private void EnemyCombat()
    {
        if (player == null) return;

        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        // FACE THE PLAYER
        Vector3 direction = (player.transform.position - transform.position).normalized;

        if (direction.x > 0)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else
            transform.rotation = Quaternion.Euler(0, -90, 0);

        
        // MOVE TOWARD PLAYER
        if (distanceFromPlayer > enemy_AttackRadius)
        {
            // Walk animation
            anim.SetBool("isWalking", true);

            Vector3 move = new Vector3(direction.x, 0, 0); // 2D movement
            rb.velocity = move * enemy_MovementSpeed;
        }
        else
        {
            // Stop moving
            rb.velocity = Vector3.zero;
            anim.SetBool("isWalking", false);

            // Attack
            if (canAttack)
            {
                canAttack = false;
                anim.CrossFade("Attack_One", 0.1f);
            }
        }
    }

    private void ResetAttack()
    {
        if (canAttack)
            return;

        if (attackCooldownTimer < attackCooldown)
            attackCooldownTimer += Time.deltaTime;
        else
        {
            attackCooldownTimer = 0;
            canAttack = true;
        }
    }

    public void TakeDamage(float amount)
    {
        enemy_Health -= amount;
    }

    public void Spawn_CombatVFX()
    {
        GameObject vfx = Instantiate(combatVFX, combatVFX_Point);
        vfx.GetComponent<DamageCollider>().enemyDamage = baseDamage;
        vfx.transform.SetParent(null);
        Destroy(vfx, 2f);
    }

    private void Die()
    {
        // PLAY VFX AND DESTROY THE BODY THEN DESTROY VFX
        player.currentPoints += 60 * player.currentWave;
        anim.CrossFade("Death", 0.1f);
        GameObject vfx = Instantiate(deathVFX, this.transform);
        vfx.transform.SetParent(null);
        Destroy(vfx, 2f);
        Destroy(this.gameObject);
    }
}
