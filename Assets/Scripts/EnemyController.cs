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
    public float wakeUp;
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
    public LayerMask playerSightLayer;
    public bool playerInSight;

    [Header("Behaviour")]
    public bool chasing;
    public bool patrolling;

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

        if (playerInRange)
        {
            if (Time.time >= nextAttackTime && isGrounded)
            {
                StartCoroutine("Attack");
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

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

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(wakeUp);
        if (playerInRange)
        {
            anim.SetTrigger("attack");
            yield return new WaitForSeconds(1.5f);
            player.TakeDamage(damage);
            Debug.Log("Took Damage" + damage);
        }
    }
}
