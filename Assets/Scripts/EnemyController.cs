using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    PlayerController player;
    Rigidbody2D rb;

    public int maxHealth;
    public int currentHealth;
    SpriteRenderer sr;

    [Header("Ground Check")]
    public Transform groundcheck;
    public float groundcheckRadius;
    public LayerMask ground;
    public bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Ground Dectection
        isGrounded = Physics2D.OverlapCircle(groundcheck.position, groundcheckRadius, ground);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        //Play hurt animation

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
}
