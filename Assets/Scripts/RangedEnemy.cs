using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
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

    [Header("Sight")]
    public Transform sight;
    public float sightDistance;
    public float attackRate;
    public bool playerInSight;
    public LayerMask playerLayer;
    float nextAttackTime = 0f;

    [Header("Behaviour")]
    public bool chasing;
    public bool patrolling;
    public bool stunned;
    public float stunTime;
    public float speed;

    public GameObject attackEffect;
    public GameObject ammo;

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
        //Sight
        playerInSight = Physics2D.OverlapCircle(sight.position, sightDistance, playerLayer);

        if (!stunned)
        {
            if (playerInSight)
            {
                if (Time.time >= nextAttackTime && isGrounded)
                {
                    anim.SetTrigger("Attack");
                    nextAttackTime = Time.time + 1f / attackRate;
                }
                else if (playerInSight && !stunned)
                {
                    rb.velocity = Vector2.zero;
                }

                if (player.transform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(2, 2, transform.localScale.z);
                }
                else if (player.transform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(-2, 2, transform.localScale.z);
                }
            }
        }
        else
        {
            if (stunTime > 0)
            {
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
        Gizmos.DrawWireSphere(groundcheck.position, groundcheckRadius);
        Gizmos.DrawWireSphere(sight.position, sightDistance);
    }

}
