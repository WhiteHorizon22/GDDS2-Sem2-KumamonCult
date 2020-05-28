using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    TouchIntegratedPlayerControl player;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    LevelManager theManager;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        player = FindObjectOfType<TouchIntegratedPlayerControl>();
        rb = GetComponent<Rigidbody2D>();
        theManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Vector3 pos = transform.position;
        Quaternion rotation = transform.rotation;
        Destroy(gameObject);
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

}
