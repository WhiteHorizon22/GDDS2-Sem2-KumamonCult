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
    public float standardAttackDamage;
    public float uppercutDamage;
    public float dashDamage;
    public float reaperSlashDamage;

    [Header("Uppercut")]
    public float uppercutAirBoost;

    [Header("Reaper Slash Variables")]
    //Set Up
    public bool reaperSlashActivated;
    public float slowDownFactor;
    //Area of Effect
    public GameObject reaperSlashArea;
    public float reaperSlashRadius;
    public Vector3 reaperSlashTarget;
    //Dash Execution
    public bool dashTargetSelected;
    public float reaperSlashDuration;
    private float currentSlashDuration;
    public float reaperSlashSpeed;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        levelManager = FindObjectOfType<LevelManager>();
        currentSlashDuration = reaperSlashDuration;
        doubleJumpUsed = false;
        reaperSlashActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Ground Dectection
        isGrounded = Physics2D.OverlapCircle(groundcheck.position, groundcheckRadius, ground);

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
        }

        //Uppercut
        if (Input.GetKeyDown(KeyCode.E))
        {

        }

        //Reaper Slash Setup
        if (Input.GetKeyDown(KeyCode.Mouse1) && !reaperSlashActivated)
        {
            reaperSlashActivated = true;

            //Slow Down Time
            Time.timeScale = slowDownFactor;

            //Set Up Timer


            //Create Area for Player to choose ReaperSlashTarget
            reaperSlashArea.SetActive(true);
            reaperSlashArea.transform.localScale = new Vector3(reaperSlashRadius, transform.localScale.y, transform.localScale.z);

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
    }

    void Attack()
    {
        //Detect Enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackRange.position, attackRangeRadius, attackable);
        
        //Apply Damage to Detected Enemies
        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("we hit" + enemy.name);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackRange.position, attackRangeRadius);
    }
}
