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
    public int reaperSlashDamage;

    [Header("Uppercut")]
    public float uppercutAirBoost;
    public float uppercutKnockback;

    [Header("GroundPound & Slam")]
    public float poundDownForce;
    public float poundDownKnockback;
    public float slamKnockback;
    public bool usingGroundPound;
    public Transform groundPoundCheck;

    [Header("Dash")]
    public float dashSpeed;
    public float currentDashDuration;
    public float dashDuration;
    public bool dashing;

    [Header("Reaper Slash Variables")]
    //Set Up
    public bool reaperSlashActivated;
    public float slowDownFactor;
    //Area of Effect
    public GameObject reaperSlashArea;
    public float reaperSlashRadius;
    public Vector2  reaperSlashTarget;
    //Dash Execution
    public bool dashTargetSelected;
    public float reaperSlashDuration;
    private float currentSlashDuration;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        respawnPosition = transform.position; // Wheb game starts, respawn position is equal to player's current position
        theManager = FindObjectOfType<LevelManager>();

        currentSlashDuration = reaperSlashDuration;
        currentDashDuration = dashDuration;
        doubleJumpUsed = false;
        reaperSlashActivated = false;
        dashing = false;
        usingGroundPound = false;
        knockedBack = false;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Ground Dectection
        isGrounded = Physics2D.OverlapCircle(groundcheck.position, groundcheckRadius, ground);

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
            if (Input.GetButtonDown("Jump") && isGrounded)
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
            if (Time.time >= nextAttackTime & !reaperSlashActivated)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }

                //Uppercut
                if (Input.GetKeyDown(KeyCode.E))
                {
                    rb.AddForce(Vector2.up * uppercutAirBoost);
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }

                //Groundpound and Slam
                if (Input.GetKeyDown(KeyCode.S))
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

            //Reaper Slash Setup
            if (Input.GetKeyDown(KeyCode.Mouse1) && !reaperSlashActivated)
            {
                reaperSlashActivated = true;

                //Slow Down Time
                Time.timeScale = slowDownFactor;

                //Create Area for Player to choose ReaperSlashTarget
                reaperSlashArea.SetActive(true);
                reaperSlashArea.transform.localScale = new Vector3(reaperSlashRadius, reaperSlashRadius, transform.localScale.z);

            }
            else if (Input.GetKeyDown(KeyCode.Mouse1) && reaperSlashActivated) //This is for Debugging only
            {
                reaperSlashActivated = false;

                //Reset Time to normal
                Time.timeScale = 1f;

                //Reset ReaperSlash
                reaperSlashArea.SetActive(false);
            }
            //Execute Reaper Slash
            if (reaperSlashActivated && dashTargetSelected)
            {
                currentSlashDuration -= Time.deltaTime;

                if (currentSlashDuration > 0)
                {
                    Attack();
                    rb.MovePosition(reaperSlashTarget);
                    if (reaperSlashTarget.x > transform.position.x)
                    {
                        transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
                    }
                    else if (reaperSlashTarget.x < transform.position.x)
                    {
                        transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
                    }
                }
                else
                {
                    if (currentSlashDuration <= 0)
                    {
                        dashTargetSelected = false;
                        reaperSlashActivated = false;
                        rb.velocity = Vector2.zero;
                        currentSlashDuration = reaperSlashDuration;
                    }
                }
            }
            //Dash
            if (Input.GetKeyDown(KeyCode.Q))
            {
                currentDashDuration = dashDuration;
                dashing = true;
            }
        }
        else if (dashing & !knockedBack)
        {
            if (currentDashDuration >= 0)
            {
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
                dashing = false;
            }
        }
        else if (usingGroundPound & !knockedBack)
        {
            //Detect Enemies in range of attack
            Collider2D[] poundedEnemies = Physics2D.OverlapCircleAll(groundPoundCheck.position, 2f, attackable);

            if (Input.GetKeyUp(KeyCode.S))
            {
                //Apply Damage to Detected Enemies
                foreach (Collider2D enemy in poundedEnemies)
                {

                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.up * poundDownKnockback);

                    if (enemy.transform.position.x > this.transform.position.x)
                    {
                        enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.right * poundDownKnockback / 2);
                    }
                    else if (enemy.transform.position.x < this.transform.position.x)
                    {
                        enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.left * poundDownKnockback / 2);
                    }

                    if (enemy.name.Contains("Melee"))
                    {
                        if (enemy.GetComponent<EnemyController>().isGrounded)
                        {
                            enemy.GetComponent<EnemyController>().TakeDamage(standardAttackDamage);
                        }
                    }
                    else if (enemy.name.Contains("Ranged"))
                    {
                        if (enemy.GetComponent<RangedEnemy>().isGrounded)
                        {
                            enemy.GetComponent<RangedEnemy>().TakeDamage(standardAttackDamage);
                        }
                    }
                }
            }

            if (!isGrounded)
            {
                rb.velocity = Vector2.down * poundDownForce/2;
            }
            else if (isGrounded)
            {
                usingGroundPound = false;
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
                reaperSlashActivated = false;
                Time.timeScale = 1f;
                reaperSlashArea.SetActive(false);

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
            if (Input.GetKey(KeyCode.Mouse0))
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.up * uppercutKnockback);
                enemy.GetComponent<EnemyController>().TakeDamage(uppercutDamage);
                if (enemy.name.Contains("Melee"))
                {
                    enemy.GetComponent<EnemyController>().TakeDamage(standardAttackDamage);
                }
                else if (enemy.name.Contains("Ranged"))
                {
                    enemy.GetComponent<RangedEnemy>().TakeDamage(standardAttackDamage);
                }
            }

            //Is using Slam
            if (Input.GetKeyDown(KeyCode.S) && isGrounded)
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
                    enemy.GetComponent<EnemyController>().TakeDamage(standardAttackDamage);
                }
                else if (enemy.name.Contains("Ranged"))
                {
                    enemy.GetComponent<RangedEnemy>().TakeDamage(standardAttackDamage);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!knockedBack)
        {
            currentHealth -= damage;

            theManager.UpdateHeartMeter();

            //hurt
        }
        knockedBack = true;
        canMove = false;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Checkpoint")
        {
            respawnPosition = other.transform.position; // Set player respawn position to entered Checkpoint's position
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackRange.position, attackRangeRadius);
        Gizmos.DrawWireSphere(groundcheck.position, groundcheckRadius);
        Gizmos.DrawWireSphere(groundPoundCheck.position, 2f);
    }
}
