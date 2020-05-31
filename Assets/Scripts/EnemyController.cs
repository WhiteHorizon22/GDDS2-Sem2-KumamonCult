using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    TouchIntegratedPlayerControl player;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    LevelManager theManager;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;
    public GameObject healthBar;

    [Header("Looks")]
    public float enemySize;

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
    public bool canMove;
    public GameObject deathEffect;

    public GameObject attackEffect;
    public StaminaBar rageMeter;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        player = FindObjectOfType<TouchIntegratedPlayerControl>();
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
                    if (tag.Contains("Melee"))
                    {
                        anim.SetBool("chase", false);
                    }
                    anim.SetTrigger("attack");
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
            else if (playerInSight && !playerInRange)
            {
                if (isGrounded)
                {
                    if (tag.Contains("Ranged"))
                    {
                        if (Time.time >= nextAttackTime && isGrounded && !stunned)
                        {
                            anim.SetTrigger("Attack");
                            nextAttackTime = Time.time + 1f / attackRate;
                        }
                    }
                       
                    else if (tag.Contains("Melee"))
                    {
                        if (!stunned && !canMoveAhead)
                        {
                            rb.velocity = Vector2.zero;
                            anim.SetBool("chase", false);
                            if (!canMoveAhead & (player.transform.position.x > front.transform.position.x))
                            {
                                transform.localScale = new Vector3(enemySize, enemySize, transform.localScale.z);
                            }
                            else if (player.transform.position.x < front.transform.position.x)
                            {
                                transform.localScale = new Vector3(-enemySize, enemySize, transform.localScale.z);
                            }
                        }
                        else
                        {
                            if (player.transform.position.x > front.transform.position.x)
                            {
                                transform.localScale = new Vector3(enemySize, enemySize, transform.localScale.z);
                                anim.SetBool("chase", true);
                                rb.velocity = new Vector2(speed, rb.velocity.y);
                            }
                            else if (player.transform.position.x < front.transform.position.x)
                            {
                                transform.localScale = new Vector3(-enemySize, enemySize, transform.localScale.z);
                                anim.SetBool("chase", true);
                                rb.velocity = new Vector2(-speed, rb.velocity.y);
                            }
                        }
                    }
                }
            }
            
        }
        else
        {
            if (stunTime > 0)
            {
                stunTime -= Time.deltaTime;
                if (tag.Contains("Melee"))
                {
                    anim.SetBool("chase", false);
                }
            }
            else
            {
                stunTime = 2f;
                stunned = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        stunned = true;

        rageMeter.mana.IncreaseMana(2);
        theManager.AddPoints(10);

        currentHealth -= damage;
        UpdateHealthBar();
        Destroy(Instantiate(attackEffect, rb.transform), 2);

        //Play hurt animation
        anim.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Vector3 pos = transform.position;
        Quaternion rotation = transform.rotation;
        Destroy(Instantiate(deathEffect, pos, rotation), 1);
        rageMeter.mana.IncreaseMana(2);
        theManager.AddPoints(100);
        Destroy(gameObject);
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
