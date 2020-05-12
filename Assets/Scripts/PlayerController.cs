using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //References
    public Rigidbody2D rb;
    Animator anim;
    public Vector3 respawnPosition; // Store a respawn position the player will go to whenever she dies
    LevelManager theManager; // Make reference to LevelManager
    public Vector3 originalSpawn;

    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    [Header("Movement")]
    public bool isMoving;
    public float moveSpeed;
    public float jumpHeight;
    public bool doubleJumpUsed;
    public bool canMove;

    [Header("Ground Check")]
    public Transform groundcheck;
    public float groundcheckRadius;
    public LayerMask ground;
    public bool isGrounded;

    [Header("Knockback")]
    public bool knockedBack;
    public float knockbackDuration;
    public float knockbackStrength;
    public float originKnockbackDuration;

    [Header("Attack Range")]
    public Transform attackRange;
    public float attackRangeRadius;
    public LayerMask attackable;
    public float attackRate;
    float nextAttackTime = 0f;

    [Header("Damage")]
    public int standardAttackDamage;
    public int standardAttackBoost;
    public int standardAttackKnockback;
    public int uppercutDamage;
    public int dashDamage;
    public int slamDamage;

    [Header("Uppercut")]
    public float uppercutAirBoost;
    public float uppercutKnockback;

    [Header("GroundPound & Slam")]
    public float poundDownForce;
    public float poundDownKnockback;
    public float slamKnockback;
    public bool usingGroundPound;
    public Transform groundPoundCheck;
    public bool onEnemy;

    [Header("Dash")]
    public float dashSpeed;
    public float currentDashDuration;
    public float dashDuration;
    public bool dashing;

    [Header("Colliders")]
    public GameObject normalBodyCollision;


    public StaminaBar rageMeter;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        respawnPosition = transform.position; // Wheb game starts, respawn position is equal to player's current position
        theManager = FindObjectOfType<LevelManager>();
        rageMeter = FindObjectOfType<StaminaBar>();

        currentDashDuration = dashDuration;
        doubleJumpUsed = false;
        dashing = false;
        usingGroundPound = false;
        knockedBack = false;
        canMove = true;
        originalSpawn = transform.position;
        groundPoundCheck.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Ground Dectection
        isGrounded = Physics2D.OverlapCircle(groundcheck.position, groundcheckRadius, ground);
        //If on Enemy
        onEnemy = Physics2D.OverlapCircle(groundcheck.position, groundcheckRadius, attackable);

        UpdatePlayerStats();

        if (isGrounded)
        {
            anim.SetBool("IsGrounded", true);
        }
        else
        {
            anim.SetBool("IsGrounded", false);
        }

        if (!dashing && !usingGroundPound && !knockedBack && canMove)
        {
            //Manual Movement
            float h = Input.GetAxisRaw("Horizontal");

            rb.velocity = new Vector2(moveSpeed * h, rb.velocity.y);
            if (h > 0) //Face Forward
            {
                transform.localScale = new Vector2(1f, transform.localScale.y);
            }
            else if (h < 0)
            {
                transform.localScale = new Vector2(-1f, transform.localScale.y);
            }

            if (h == 0)
            {
                anim.SetBool("IsMoving", false);
            }
            else
            {
                anim.SetBool("IsMoving", true);
            }

            //Jumping
            if (Input.GetButtonDown("Jump") && (isGrounded || onEnemy))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            }
            else if (Input.GetButtonDown("Jump") && !isGrounded && !doubleJumpUsed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                doubleJumpUsed = true;
            }
            //Reset Double Jump when Grounded
            if (isGrounded)
            {
                doubleJumpUsed = false;
            }

            //Attacking
            if (Time.time >= nextAttackTime)
            {
                if (Input.GetKey(KeyCode.J))
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }

                //Uppercut
                if (Input.GetKeyDown(KeyCode.I))
                {
                    rb.AddForce(Vector2.up * uppercutAirBoost);
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }

                //Groundpound and Slam
                if (Input.GetKeyDown(KeyCode.K))
                {
                    if (isGrounded)
                    {
                        Attack();
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                    else if (!isGrounded)
                    {
                        Attack();
                        usingGroundPound = true;
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                }
            }
            //Dash
            if (Input.GetKeyDown(KeyCode.L))
            {
                currentDashDuration = dashDuration;
                dashing = true;
            }
        }
        else if (dashing & !knockedBack)
        {
            if (currentDashDuration >= 0)
            {
                normalBodyCollision.gameObject.SetActive(false);
                currentDashDuration -= Time.deltaTime;

                if (transform.localScale.x > 0)
                {
                    rb.velocity = Vector2.right * dashSpeed;
                }
                else if (transform.localScale.x < 0)
                {
                    rb.velocity = Vector2.left * dashSpeed;
                }
            }
            else if (currentDashDuration <= 0)
            {
                normalBodyCollision.gameObject.SetActive(true);
                dashing = false;
            }
        }
        else if (usingGroundPound & !knockedBack)
        {
            if (!isGrounded && !onEnemy)
            {
                rb.velocity = Vector2.down * poundDownForce/2;
                groundPoundCheck.gameObject.SetActive(true);
                normalBodyCollision.gameObject.SetActive(false);
            }
            else if (isGrounded || onEnemy)
            {
                if (onEnemy)
                {
                    normalBodyCollision.gameObject.SetActive(false);
                }
                else if (isGrounded)
                {
                    normalBodyCollision.gameObject.SetActive(true);
                    groundPoundCheck.gameObject.SetActive(false);
                    usingGroundPound = false;
                }
            }
        }
        //Knockback
        else
        {
            rb.velocity = Vector2.zero;
            if (knockbackDuration >= 0)
            {
                canMove = false;
                anim.SetTrigger("Hurt");
                Time.timeScale = 1f;

                knockbackDuration -= Time.deltaTime;

                if (transform.localScale.x < 0)
                {
                    rb.AddForce(new Vector2(1 * knockbackStrength, knockbackStrength));
                }
                else if (transform.localScale.x > 0)
                {
                    rb.AddForce(new Vector2(-1 * knockbackStrength, knockbackStrength));
                }
            }
            else if (knockbackDuration <= 0)
            {
                knockedBack = false;
                canMove = true;
                knockbackDuration = originKnockbackDuration;
            }
        }
    }

    void Attack()
    {
        attackRange.GetComponent<AudioSource>().Play();

        //Detect Enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackRange.position, attackRangeRadius, attackable);
        
        //Apply Damage to Detected Enemies
        foreach(Collider2D enemy in hitEnemies)
        {
            //If Using basic Attack
            if (Input.GetKey(KeyCode.J))
            {
                if (this.isGrounded == false)
                {
                    this.rb.AddForce(Vector2.up * standardAttackBoost);
                }
                if (enemy.transform.position.x > this.transform.position.x)
                {
                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.right * standardAttackKnockback);
                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.up * standardAttackKnockback);
                }
                else if (enemy.transform.position.x < this.transform.position.x)
                {
                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.left * standardAttackKnockback);
                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.up * standardAttackBoost);
                }
                if (enemy.name.Contains("Melee"))
                {
                    enemy.GetComponent<EnemyController>().TakeDamage(standardAttackDamage);
                }
                else if (enemy.name.Contains("Ranged"))
                {
                    enemy.GetComponent<RangedEnemy>().TakeDamage(standardAttackDamage);
                }
            }

            //If using Uppercut
            if (Input.GetKeyDown(KeyCode.I))
            {
                enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.up * uppercutKnockback);

                if (enemy.name.Contains("Melee"))
                {
                    enemy.GetComponent<EnemyController>().TakeDamage(uppercutDamage);
                }
                else if (enemy.name.Contains("Ranged"))
                {
                    enemy.GetComponent<RangedEnemy>().TakeDamage(uppercutDamage);
                }
            }

            if (dashing)
            {
                if (enemy.name.Contains("Melee"))
                {
                    enemy.GetComponent<EnemyController>().TakeDamage(dashDamage);
                }
                else if (enemy.name.Contains("Ranged"))
                {
                    enemy.GetComponent<RangedEnemy>().TakeDamage(dashDamage);
                }
            }

            //Is using Slam
            if (Input.GetKeyDown(KeyCode.K) && isGrounded)
            {
                if (enemy.transform.position.x > this.transform.position.x)
                {
                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.right * slamKnockback);
                }
                else if (enemy.transform.position.x < this.transform.position.x)
                {
                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.left * slamKnockback);
                }
                if (enemy.name.Contains("Melee"))
                {
                    enemy.GetComponent<EnemyController>().TakeDamage(slamDamage);
                }
                else if (enemy.name.Contains("Ranged"))
                {
                    enemy.GetComponent<RangedEnemy>().TakeDamage(slamDamage);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!knockedBack)
        {
            currentHealth -= damage;
            rageMeter.mana.DecreaseMana(5);
            theManager.UpdateHeartMeter();

            //hurt
        }
        knockedBack = true;
        canMove = false;
    }

    public void Heal(int heal)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += heal;
        }
        else
        {
            currentHealth = maxHealth;
        }
        theManager.UpdateHeartMeter();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (usingGroundPound)
        {
            if (other.tag == "Enemy")
            {
                if (other.transform.position.x > transform.position.x)
                {
                    other.GetComponent<Rigidbody2D>().AddForce(Vector2.right * poundDownKnockback);
                    other.GetComponent<Rigidbody2D>().AddForce(Vector2.up * poundDownKnockback);
                }
                else if (other.transform.position.x < transform.position.x)
                {
                    other.GetComponent<Rigidbody2D>().AddForce(Vector2.left * poundDownKnockback);
                    other.GetComponent<Rigidbody2D>().AddForce(Vector2.up * poundDownKnockback);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Checkpoint")
        {
            if (other.GetComponent<CheckpointController>().checkpointActive == true)
            {
                respawnPosition = other.transform.position; // Set player respawn position to entered Checkpoint's position
            }
        }
    }

    public void UpdatePlayerStats()
    {
        if (rageMeter.mana.manaAmount > 20 && rageMeter.mana.manaAmount < 40)
        {
            standardAttackDamage = 2;
            dashDamage = 2;
            slamDamage = 2;
            uppercutDamage = 2;
        }
        else if (rageMeter.mana.manaAmount > 40 && rageMeter.mana.manaAmount < 60)
        {
            standardAttackDamage = 3;
            dashDamage = 3;
            slamDamage = 3;
            uppercutDamage = 3;
        }
        else if (rageMeter.mana.manaAmount > 60 && rageMeter.mana.manaAmount < 80)
        {
            standardAttackDamage = 4;
            dashDamage = 4;
            slamDamage = 4;
            uppercutDamage = 4;
        }
        else if (rageMeter.mana.manaAmount > 80)
        {
            standardAttackDamage = 5;
            dashDamage = 5;
            slamDamage = 5;
            uppercutDamage = 5;
        }
        else if (rageMeter.mana.manaAmount < 20)
        {
            standardAttackDamage = 1;
            dashDamage = 1;
            slamDamage = 1;
            uppercutDamage = 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackRange.position, attackRangeRadius);
        Gizmos.DrawWireSphere(groundcheck.position, groundcheckRadius);
    }
}
