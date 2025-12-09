using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    Animator anim;
    [SerializeField] Transform model;

    [Header("Round Stats")]
    public int currentWave = 1;
    public int currentPoints = 0;
    public float currentMatchTime_Seconds = 0;
    public float currentMatchTime_Minutes = 0;

    [Header("Comabt Stats")]
    public float playersCurrentHealth = 100;
    public float playersMaxHealth = 100;
    public float baseDamage = 5;
    public bool playerCanBeDamaged = true; // this is used for I-Frames

    [Header("Jumping")]
    public bool playerIsJumping = false;
    public float playerMovementSpeed = 5f;
    public float crouchSpeedMultiplier = 0.5f;
    public float jumpForce = 7f;

    [Header("Jumping VFX")]
    public GameObject jumpVfx;
    public Transform jumpVfxPoint;

    [Header("Dashing VFX")]
    public GameObject DashVfx;
    public Transform DashVfxPoint;

    [Header("Dodge Bools")]
    public bool playerIsDodging = false;
    public bool canPlayerDodge = true;

    [Header("Dodge movement settings")]
    public float dodgeSpeed = 12f;
    public float dodgeDuration = 0.25f;
    private float dodgeTime = 0f;
    private float dodgeDirection = 0f;

    [Header("Dodge Cooldown")]
    public float dodgeCoolDownTime = 5f;
    private float dodgeCooldownTimer = 0f;

    [Header("Ground Combat VFX")]
    public bool isAttacking = false;
    public GameObject slashVfx;
    public Transform slashVfxPoint;

    [Header("Ground Combat Combo")]
    public int nextComboStep = 1; // 1 --> 2 --> 3 --> 1
    public bool canCombo = false;

    [Header("Air Attack VFX Slash")]
    public GameObject airSlash_Projectile;
    public Transform airSlash_VfxPoint;

    [Header("Air Attack VFX Spin")]
    public GameObject jumpingSpinningVfx;
    public Transform jumpingSpinningVfxPoint;


    [Header("Player UI")]
    public GameObject deathScreen;
    public TextMeshProUGUI currentHealth_Text;
    public TextMeshProUGUI timer_Seconds_Text;
    public TextMeshProUGUI timer_Minutes_Text;
    public TextMeshProUGUI points_Text;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        PlayerMovement();
        PlayerCombat();

        UpdateAnimations();

        DodgeCooldown();

        Timer();
        UpdatUI();

        Die();
    }
    private void UpdatUI()
    {
        currentHealth_Text.text = playersCurrentHealth.ToString("n0") + (" / ") + playersMaxHealth.ToString("n0");
        points_Text.text = currentPoints.ToString();

        // KEEPS IT CLEAN - KEEPS THE 0 IN FRONT OF SINGLE DIGIT NUMBERS
        if (currentMatchTime_Seconds <= 9)
        {
            timer_Seconds_Text.text = "0" + currentMatchTime_Seconds.ToString("0");
        }
        else
        {
            timer_Seconds_Text.text = currentMatchTime_Seconds.ToString("0");
        }

        if (currentMatchTime_Minutes <= 9)
        {
            timer_Minutes_Text.text = "0" + currentMatchTime_Minutes.ToString("0");
        }
        else
        {
            timer_Minutes_Text.text = currentMatchTime_Minutes.ToString("0");
        }
    }

    private void PlayerMovement()
    {
        if (isAttacking)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);  // freeze horizontal movement
            return; // skip all movement logic
        }

        float speed = playerMovementSpeed;
        float moveX = 0f;

        // DODGE OVERRIDE
        if (playerIsDodging)
        {
            dodgeTime += Time.deltaTime;

            rb.velocity = new Vector3(dodgeDirection * dodgeSpeed, rb.velocity.y, 0);

            if (dodgeTime >= dodgeDuration)
            {
                playerIsDodging = false;
            }

            return; // don't run normal movement during dodge
        }

        // DODGE INPUT
        if (Input.GetKeyDown(KeyCode.Space) && canPlayerDodge)
        {
            playerCanBeDamaged = false;
            DashVfx.SetActive(true);
            playerIsDodging = true;
            canPlayerDodge = false;
            dodgeTime = 0f;

            // Check direction based on model facing
            dodgeDirection = (model.localEulerAngles.y == 90) ? 1 : -1;

            anim.CrossFade("Dash", 0.1f);
        }

        // NORMAL MOVEMENT
        // Move Left
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -speed;
            model.localRotation = Quaternion.Euler(0, -90, 0);
        }

        // Move Right
        if (Input.GetKey(KeyCode.D))
        {
            moveX = speed;
            model.localRotation = Quaternion.Euler(0, 90, 0);
        }

        // Apply horizontal movement
        rb.velocity = new Vector3(moveX, rb.velocity.y, 0);

        // Jump
        if (Input.GetKeyDown(KeyCode.W) && !playerIsJumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
            playerIsJumping = true;
        }
    }
    private void PlayerCombat()
    {
        // COMBO CLICK
        if (Input.GetKeyDown(KeyCode.Mouse0) && !playerIsDodging)
        {
            if (!playerIsJumping)
            {
                HandleGroundCombo();
            }
            else
            {
                HandleJumpAttack();
            }
        }
    }

    private void HandleGroundCombo()
    {
        if (nextComboStep == 1 && !isAttacking)
        {
            isAttacking = true;
            anim.CrossFade("Attack_Ground_01", 0.1f);
            canCombo = false;
        }
        else if (nextComboStep == 2 && canCombo)
        {
            anim.CrossFade("Attack_Ground_02", 0.1f);
            canCombo = false;
        }
        else if (nextComboStep == 3 && canCombo)
        {
            anim.CrossFade("Attack_Ground_03", 0.1f);
            canCombo = false;
        }
    }

    private void HandleJumpAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        rb.isKinematic = true;

        anim.CrossFade("Attack_Jumping_01", 0.1f);
    }

    public void EnableCombo()
    {
        canCombo = true;

        // we handle the combo step in here so we do combo in the correct order
        if (nextComboStep == 1)
            nextComboStep = 2;
        else if (nextComboStep == 2)
            nextComboStep = 3;
        else
        {
            nextComboStep = 1;
            isAttacking = false; // we set false here, so we can do attack 1 - only 1 time, some odd reason after loop it was spammable
        }
    }

    public void DisableCombo()
    {
        canCombo = false;
        nextComboStep = 1; // resets if you dont combo in time
    }

    private void UpdateAnimations()
    {
        // Movement animations
        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;
        anim.SetBool("isMoving", isMoving);

        // Jumping animations
        anim.SetBool("isJumping", playerIsJumping);

    }

    // VFX MOVEMENT
    public void SpawnDashVfx_AnimatinEvent()
    {
        GameObject vfx = Instantiate(DashVfx, DashVfxPoint);
        vfx.transform.SetParent(null);
        Destroy(vfx, 2f);
    }
    public void SpawnJumpVfx_AnimatinEvent()
    {
        GameObject vfx = Instantiate(jumpVfx, jumpVfxPoint);
        vfx.transform.SetParent(null);
        Destroy(vfx, 2f);
    }


    // COMBAT VFX GROUND
    public void SpawnGroundSlashVFX_AnimationEvent()
    {
        // SPAWNS GROUND SLASH VFX 
        GameObject vfx = Instantiate(slashVfx, slashVfxPoint);
        vfx.GetComponent<DamageCollider>().playerDamage = baseDamage;
        vfx.transform.SetParent(null);
        Destroy(vfx, 2f);
    }

    // COBAT VFX AIR
    public void SpawnAirProjectileVFX_AnimationEvent()
    {
        // SPAWNS THE PROJECTILE
        GameObject proj = Instantiate(airSlash_Projectile, airSlash_VfxPoint.position, Quaternion.identity);
        proj.GetComponent<DamageCollider>().playerDamage = baseDamage * 0.85f; // AIR ATTACKS, SINCE ARE PROJECTILES DEAL 15% less dmg (Multiply by 0.85 to be left with 85% of the total dmg)
        Vector3 dir = (model.localEulerAngles.y == 90) ? Vector3.right : Vector3.left;
        proj.GetComponent<ProjectileManager>().Initialize(dir);
    }
    public void SpawnAirSpinVFX_AnimationEvent()
    {
        // SPAWNS THE SPINNING VFX


        GameObject vfx = Instantiate(jumpingSpinningVfx, jumpingSpinningVfxPoint);
        vfx.transform.SetParent(null);
        Destroy(vfx, 2f);

    }


    // Detect ground
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            playerIsJumping = false;
            DashVfx.SetActive(false);
        }
    }

    // DODGE COOLDWON
    private void DodgeCooldown()
    {
        if (!canPlayerDodge)
        {
            dodgeCooldownTimer += Time.deltaTime;

            if (dodgeCooldownTimer >= dodgeCoolDownTime)
            {
                canPlayerDodge = true;
                dodgeCooldownTimer = 0f;
            }
        }
    }


    // TAKE DAMAGE
    public void TakeDamage(float damage)
    {
        if (playerCanBeDamaged)
            playersCurrentHealth -= damage;
    }


    // Keep track of match time
    private void Timer()
    {
        if (currentMatchTime_Seconds < 60)
            currentMatchTime_Seconds += Time.deltaTime;
        else
        {
            currentMatchTime_Seconds = 0;
            currentMatchTime_Minutes++;
        }
    }

    private void Die()
    {
        if (playersCurrentHealth <= 0)
        {
            // KILL PLAYER
            deathScreen.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
