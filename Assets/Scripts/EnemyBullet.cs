using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    Vector2 moveDirection;
    Rigidbody2D rb; // Reference to Rigid 2D component in Bullet prefab

    public int damageToGive;
    PlayerController player;
    public float bulletForce;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();



        player = FindObjectOfType<PlayerController>();

        if (player.gameObject.activeInHierarchy == true)
        {
            moveDirection = (player.transform.position - transform.position).normalized * bulletForce;

            rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            player.TakeDamage(damageToGive);
        }
    }

}
