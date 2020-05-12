using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
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
    public GameObject deathEffect;

    public GameObject attackEffect;
    public GameObject ammo;

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
        UpdateHealthBar();
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
            if (player.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(2, 2, transform.localScale.z);
            }
            else if (player.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-2, 2, transform.localScale.z);
            }

            if (playerInSight)
            {
                if (Time.time >= nextAttackTime && isGrounded && !stunned)
                {
                    anim.SetTrigger("Attack");
                    nextAttackTime = Time.time + 1f / attackRate;
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

        rageMeter.mana.IncreaseMana(1);

        theManager.AddPoints(10);

        currentHealth -= damage;
        UpdateHealthBar();
        Destroy(Instantiate(attackEffect, rb.transform), 2);

        //Play hurt animation
        anim.SetTrigger("Hurt");

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

    void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector3(currentHealth / 10f, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(groundcheck.position, groundcheckRadius);
        Gizmos.DrawWireSphere(sight.position, sightDistance);
    }

}
