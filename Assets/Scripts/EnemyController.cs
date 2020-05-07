using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    PlayerController player;
    Rigidbody2D rb;
    Animator anim;

    public int maxHealth;
    public int currentHealth;
    SpriteRenderer sr;

    [Header("Ground Check")]
    public Transform groundcheck;
    public float groundcheckRadius;
    public LayerMask ground;
    public bool isGrounded;

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
    public bool chasing;
    public bool patrolling;
    public bool stunned;
    public float stunTime;
    public float speed;

    public GameObject attackEffect;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Ground Dectection
        isGrounded = Physics2D.OverlapCircle(groundcheck.position, groundcheckRadius, ground);
        //Player Detection
        playerInRange = Physics2D.OverlapCircle(attackRange.position, attackRangeRadius, playerLayer);
        //Sight
        playerInSight = Physics2D.OverlapCircle(sight.position, sightDistance, playerLayer);

        if (stunned)
        {
            if (stunTime >= 0)
            {
                stunTime -= Time.time;
            }
            else
            {
                stunTime = 0.5f;
                stunned = false;
            }
        }
        else if (playerInRange)
        {
            if (Time.time >= nextAttackTime && isGrounded)
            {
                anim.SetTrigger("attack");
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        if (playerInSight && !stunned)
        {
            if (player.transform.position.x > transform.position.x)
            {
                anim.SetBool("chase", true);
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else if (player.transform.position.x < transform.position.x)
            {
                anim.SetBool("chase", true);
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }
            else
            {
                anim.SetBool("chase", false);
                rb.velocity = Vector2.zero;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        stunned = true;

        currentHealth -= damage;
        Destroy(Instantiate(attackEffect, rb.transform), 2);

        //Play hurt animation
        anim.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        //Die animation

        //Disable the Enemy

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackRange.position, attackRangeRadius);
        Gizmos.DrawWireSphere(groundcheck.position, groundcheckRadius);
        Gizmos.DrawWireSphere(sight.position, sightDistance);
    }
}
