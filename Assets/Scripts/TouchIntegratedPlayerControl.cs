using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchIntegratedPlayerControl : MonoBehaviour
{
    [Header("Essentials")]
    public Rigidbody2D rb;
    Animator anim;
    public Vector3 respawnPosition; // Store a respawn position the player will go to whenever she dies
    LevelManager theManager; // Make reference to LevelManager
    public Vector3 originalSpawn;
    BreakableObject breakableObject;
    Chest chestClosed;
    StaminaBar rageMeter;

    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    [Header("Ground Check")]
    public Transform groundcheck;
    public float groundcheckRadius;
    public LayerMask ground;
    public bool isGrounded;

    [Header("Movement")]
    public bool isMoving;
    public float moveSpeed;
    public float jumpHeight;
    public bool doubleJumpUsed;
    public bool canMove;

    [Header("Dash")]
    public float dashSpeed;
    public float currentDashDuration;
    public float dashDuration;
    public bool dashing;

    [Header("Attack Range")]
    public Transform attackRange;
    public float attackRangeRadius;
    public LayerMask attackable;
    public float attackRate;
    public float nextAttackTime = 0f;

    [Header("Damage")]
    public float standardAttackDamage;
    public float uppercutDamage;
    public float dashDamage;
    public float slamDamage;
    public float standardAttackBoost;
    public float standardAttackKnockback;

    [Header("Uppercut")]
    public float uppercutKnockback;

    [Header("GroundPound & Slam")]
    public float poundDownForce;
    public float poundDownKnockback;
    public float slamKnockback;
    public bool usingGroundPound;
    public Transform groundPoundCheck;
    public bool onEnemy;

    [Header("Knockback")]
    public bool knockedBack;
    public float knockbackDuration;
    public float knockbackStrength;
    public float originKnockbackDuration;

    [Header("Colliders")]
    public GameObject normalBodyCollision;

    [Header("SFX")]
    public AudioSource dash;
    public AudioSource slam;
    public AudioSource uppercut;

    //Screen Functions
    private float screenWidth;
    private float screenHeight;
    private float movementZone;
    private float jumpZone;

    [Header("Swipe Controls")]
    public float maxSwipetime;
    public float minSwipeDistance;

    private float swipeStartTime;
    private float swipeEndTime;
    private float swipeTime;

    private Vector2 startSwipePosition;
    private Vector2 endSwipePosition;
    private float swipeLength;

    // Start is called before the first frame update
    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        movementZone = screenWidth / 4;
        jumpZone = screenHeight / 3;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        theManager = FindObjectOfType<LevelManager>();
        rageMeter = FindObjectOfType<StaminaBar>();
        doubleJumpUsed = false;

        respawnPosition = transform.position; // Wheb game starts, respawn position is equal to player's current position
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
        if (isGrounded)
        {
            anim.SetBool("IsGrounded", true);
        }
        else
        {
            anim.SetBool("IsGrounded", false);
        }

        if (dashing)
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
                rb.velocity = Vector2.zero;
                dashing = false;
            }
        }
        else if (usingGroundPound)
        {
            if (!isGrounded)
            {
                rb.velocity = Vector2.down * poundDownForce / 2;
                groundPoundCheck.gameObject.SetActive(true);
                normalBodyCollision.gameObject.SetActive(false);
            }
            else
            {
                groundPoundCheck.gameObject.SetActive(false);
                normalBodyCollision.gameObject.SetActive(true);
                usingGroundPound = false;
            }
        }
        else if (knockedBack)
        {
            rb.velocity = Vector2.zero;
            if (knockbackDuration >= 0)
            {
                canMove = false;
                dashing = false;
                usingGroundPound = false;
                normalBodyCollision.gameObject.SetActive(true);
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
        else if (canMove & !dashing & !usingGroundPound & !knockedBack)
        {
            var touchCount = Input.touchCount;
            for (var i = 0; i < touchCount; i++)
            {
                var touch = Input.GetTouch(i);

                //Movement
                if (touch.position.x < movementZone && touch.phase == TouchPhase.Stationary)
                {
                    if (touch.position.x < (movementZone / 2))
                    {
                        //move Player Left
                        transform.localScale = new Vector2(-1f, transform.localScale.y);
                        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                        anim.SetBool("IsMoving", true);
                    }
                    else if (touch.position.x > (movementZone / 2))
                    {
                        //move Player Right
                        transform.localScale = new Vector2(1f, transform.localScale.y);
                        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                        anim.SetBool("IsMoving", true);
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    anim.SetBool("IsMoving", false);
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }

                //Attacking
                if (touch.position.x > movementZone && touch.position.y > jumpZone)
                {
                    if (Time.time >= nextAttackTime)
                    {
                        Attack();
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                }

                if (touch.position.x > movementZone && touch.position.y < jumpZone)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        if (isGrounded)
                        {
                            doubleJumpUsed = false;
                            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                        }
                        else if (!isGrounded && !doubleJumpUsed)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                            doubleJumpUsed = true;
                        }
                    }
                }

                //Swipping
                if (touch.position.x > movementZone)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        swipeStartTime = Time.time;
                        startSwipePosition = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        swipeEndTime = Time.time;
                        endSwipePosition = touch.position;
                        swipeTime = swipeEndTime - swipeStartTime;
                        swipeLength = (endSwipePosition - startSwipePosition).magnitude;

                        if (swipeTime < maxSwipetime && swipeLength > minSwipeDistance)
                        {
                            SwipeControl();
                        }
                    }
                }
            }
        }
    }

    void SwipeControl()
    {
        Vector2 Distance = endSwipePosition - startSwipePosition;
        float xDistance = Mathf.Abs(Distance.x);
        float yDistance = Mathf.Abs(Distance.y);

        if(xDistance > yDistance)
        {
            if(Distance.x > 0)
            {
                print("Dash");
                dash.Play();
                transform.localScale = new Vector2(1f, transform.localScale.y);
                currentDashDuration = dashDuration;
                dashing = true;
            }
            else if (Distance.x < 0)
            {
                print("Dash");
                dash.Play();
                transform.localScale = new Vector2(-1f, transform.localScale.y);
                currentDashDuration = dashDuration;
                dashing = true;
            }
        }
        else if (yDistance > xDistance)
        {
            if(Distance.y > 0)
            {
                print("Uppercut");
                uppercut.Play();
                rb.velocity = new Vector2(0, uppercutKnockback);
                Uppercut();
            }
            else if(Distance.y < 0)
            {
                if (isGrounded)
                {
                    print("Slam");
                    slam.Play();
                    Slam();
                }
                else
                {
                    print("GroundPound");
                    slam.Play();
                    usingGroundPound = true;
                }
            }
        }
    }
    public void Jump()
    {
        //Jump
        if (isGrounded)
        {
            doubleJumpUsed = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        }
        else if (!isGrounded && !doubleJumpUsed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            doubleJumpUsed = true;
        }
    }
    public void Attack()
    {
        attackRange.GetComponent<AudioSource>().Play();

        //Detect Enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackRange.position, attackRangeRadius, attackable);

        //Apply Damage to Detected Enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            this.rb.AddForce(Vector2.up * standardAttackBoost);

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
            else if (enemy.tag == "Destructibles")
            {
                enemy.GetComponent<BreakableObject>().TakeDamage(standardAttackDamage);
            }
            else if (enemy.name == "Chest")
            {
                enemy.GetComponent<Chest>().TakeDamage(standardAttackDamage);
            }
        }
    }
    public void Uppercut()
    {
        //Detect Enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackRange.position, attackRangeRadius, attackable);

        //Apply Damage to Detected Enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.name.Contains("Melee"))
            {
                enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(0, uppercutKnockback);
                enemy.GetComponent<EnemyController>().TakeDamage(uppercutDamage);
            }
            else if (enemy.name.Contains("Ranged"))
            {
                enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(0, uppercutKnockback);
                enemy.GetComponent<RangedEnemy>().TakeDamage(uppercutDamage);
            }
            else if (enemy.name.Contains("EnemyFireball"))
            {
                enemy.GetComponent<EnemyBullet>().Deflected();
            }
            else if (enemy.tag == "Destructibles")
            {
                enemy.GetComponent<BreakableObject>().TakeDamage(uppercutDamage);
            }
            else if (enemy.name == "Chest")
            {
                enemy.GetComponent<Chest>().TakeDamage(uppercutDamage);
            }
        }
    }
    public void Slam()
    {
        //Detect Enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackRange.position, attackRangeRadius, attackable);

        //Apply Damage to Detected Enemies
        foreach (Collider2D enemy in hitEnemies)
        {
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
        print("check");
        if (!knockedBack)
        {
            currentHealth -= damage;
            rageMeter.mana.DecreaseMana(5);
            theManager.UpdateHeartMeter();
        }
        if (currentHealth <= 0)
        {
            print("Die");
            theManager.Respawn();
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

        if (other.tag == "Killzone")
        {
            theManager.Respawn();
        }

        if (other.tag == "MovingPlatform")
        {
            transform.parent = other.gameObject.transform; //Locks Player as a child under Moving Platform
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Checkpoint")
        {
            other.GetComponent<CheckpointController>().checkpointActive = true;
            respawnPosition = other.transform.position; // Set player respawn position to entered Checkpoint's position
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "MovingPlatform")
        {
            transform.parent = null; //When Player is away from platform, it is removed from being a child under Moving Platform
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
