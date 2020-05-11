using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    PlayerController player;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    LevelManager theManager;

    [Header("Health")]
    public int maxHealth;
    public int currentHealth;
    public GameObject healthBar;

    [Header("Ground Check")]
    public Transform groundcheck;
    public float groundcheckRadius;
    public LayerMask ground;
    public bool isGrounded;
    public Transform front;

    [Header("Attack Range")]
    public int damage;
    public Transform attackRange;
    public float attackRangeRadius;
    public LayerMask playerLayer;
    public bool playerInRange;
    public float attackRate;
    float nextAttackTime = 0f;

    [Header("Sight")]
    public Transform sight;
    public float sightDistance;
    public bool playerInSight;

    [Header("Behaviour")]
    public bool canMoveAhead;
    public bool stunned;
    public float stunTime;
    public float speed;
    public GameObject deathEffect;

    public GameObject attackEffect;
    public StaminaBar rageMeter;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        rageMeter = FindObjectOfType<StaminaBar>();
        theManager = FindObjectOfType<LevelManager>();

        stunned = false;
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        //Ground Dectection
        isGrounded = Physics2D.OverlapCircle(groundcheck.position, groundcheckRadius, ground);
        //Anti-Lemming Detectiom
        canMoveAhead = Physics2D.OverlapCircle(front.position, groundcheckRadius, ground);

        //Player Detection
        playerInRange = Physics2D.OverlapCircle(attackRange.position, attackRangeRadius, playerLayer);
        //Sight
        playerInSight = Physics2D.OverlapCircle(sight.position, sightDistance, playerLayer);

        if (!stunned)
        {
            if (playerInRange)
            {
                if (Time.time >= nextAttackTime && isGrounded)
                {
                    anim.SetBool("chase", false);
                    anim.SetTrigger("attack");
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
            else if (playerInSight && !playerInRange)
            {
                if (isGrounded)
                {
                    if (!stunned && !canMoveAhead)
                    {
                        rb.velocity = Vector2.zero;
                        anim.SetBool("chase", false);
                        if (!canMoveAhead & (player.transform.position.x > front.transform.position.x))
                        {
                            transform.localScale = new Vector3(2, 2, transform.localScale.z);
                        }
                        else if (player.transform.position.x < front.transform.position.x)
                        {
                            transform.localScale = new Vector3(-2, 2, transform.localScale.z);
                        }
                    }
                    else
                    {
                        if (player.transform.position.x > front.transform.position.x)
                        {
                            transform.localScale = new Vector3(2, 2, transform.localScale.z);
                            anim.SetBool("chase", true);
                            rb.velocity = new Vector2(speed, rb.velocity.y);
                        }
                        else if (player.transform.position.x < front.transform.position.x)
                        {
                            transform.localScale = new Vector3(-2, 2, transform.localScale.z);
                            anim.SetBool("chase", true);
                            rb.velocity = new Vector2(-speed, rb.velocity.y);
                        }
                    }
                }
            }
            
        }
        else
        {
            print("stunned");
            if (stunTime > 0)
            {
                anim.SetBool("chase", false);
                stunTime -= Time.deltaTime;
            }
            else
            {
                stunTime = 2f;
                stunned = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        stunned = true;

        rageMeter.mana.IncreaseMana(1);

        theManager.AddPoints(10);

        currentHealth -= damage;
        UpdateHealthBar();
        Destroy(Instantiate(attackEffect, rb.transform), 2);

        //Play hurt animation
        anim.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            Instantiate(deathEffect, transform);
            Die();
        }
    }

    void Die()
    {
        rageMeter.mana.IncreaseMana(2);
        Destroy(gameObject);
        //Die animation
        theManager.AddPoints(100);
        //Disable the Enemy

    }

    void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector3(currentHealth / 10f, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "Player"))
        {
            if (player.usingGroundPound)
            {
                TakeDamage(player.slamDamage);
            }
            else if (player.dashing)
            {
                TakeDamage(player.dashDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackRange.position, attackRangeRadius);
        Gizmos.DrawWireSphere(groundcheck.position, groundcheckRadius);
        Gizmos.DrawWireSphere(sight.position, sightDistance);
        Gizmos.DrawWireSphere(front.position, groundcheckRadius);
    }
}
