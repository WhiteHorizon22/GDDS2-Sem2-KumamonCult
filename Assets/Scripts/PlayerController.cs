using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //References
    Rigidbody2D rb;
    LevelManager levelManager;

    [Header("Movement")]
    public bool isMoving;
    public float moveSpeed;
    public float jumpHeight;
    public bool doubleJumpUsed;

    [Header("Ground Check")]
    public Transform groundcheck;
    public float groundcheckRadius;
    public LayerMask ground;
    public bool isGrounded;

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
        levelManager = FindObjectOfType<LevelManager>();
        currentSlashDuration = reaperSlashDuration;
        currentDashDuration = dashDuration;
        doubleJumpUsed = false;
        reaperSlashActivated = false;
        dashing = false;
        usingGroundPound = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Ground Dectection
        isGrounded = Physics2D.OverlapCircle(groundcheck.position, groundcheckRadius, ground);

        if (!dashing && !usingGroundPound)
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
            if (Time.time >= nextAttackTime)
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
        else if (dashing)
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
        else if (usingGroundPound)
        {
            //Detect Enemies in range of attack
            Collider2D[] poundedEnemies = Physics2D.OverlapCircleAll(groundPoundCheck.position, 2f, attackable);

            //Apply Damage to Detected Enemies
            foreach (Collider2D enemy in poundedEnemies)
            {
                Debug.Log("we hit" + enemy.name);

                if (enemy.GetComponent<EnemyController>().isGrounded == false)
                {
                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.down * poundDownForce * 2);
                }
                else
                {
                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.up * poundDownKnockback);

                    if (enemy.transform.position.x > this.transform.position.x)
                    {
                        enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.right * poundDownKnockback);
                    }
                    else if (enemy.transform.position.x < this.transform.position.x)
                    {
                        enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.left * poundDownKnockback);
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
    }

    void Attack()
    {
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
                enemy.GetComponent<EnemyController>().TakeDamage(standardAttackDamage);
            }

            //If using Uppercut
            if (Input.GetKeyDown(KeyCode.E))
            {
                enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.up * uppercutKnockback);
                enemy.GetComponent<EnemyController>().TakeDamage(uppercutDamage);
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
                enemy.GetComponent<EnemyController>().TakeDamage(slamDamage);
            }

            if (Input.GetKeyDown(KeyCode.S) && !isGrounded && enemy.GetComponent<EnemyController>().isGrounded == false)
            {
                if (enemy.GetComponent<EnemyController>().isGrounded == false)
                {
                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.down * poundDownForce * 2);
                }
                else if (enemy.GetComponent<EnemyController>().isGrounded == true)
                {
                    enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.up * poundDownKnockback);

                    if (enemy.transform.position.x > this.transform.position.x)
                    {
                        enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.right * poundDownKnockback);
                    }
                    else if (enemy.transform.position.x < this.transform.position.x)
                    {
                        enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.left * poundDownKnockback);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackRange.position, attackRangeRadius);
        Gizmos.DrawWireSphere(groundcheck.position, groundcheckRadius);
        Gizmos.DrawWireSphere(groundPoundCheck.position, 2f);
    }
}
