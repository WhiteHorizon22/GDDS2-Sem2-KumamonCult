using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    Vector2 moveDirection;
    Rigidbody2D rb; // Reference to Rigid 2D component in Bullet prefab

    public int damageToGive;
    TouchIntegratedPlayerControl player;
    public float bulletForce;

    public bool deflected;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        player = FindObjectOfType<TouchIntegratedPlayerControl>();

        deflected = false;

        if (player.gameObject.activeInHierarchy == true)
        {
            moveDirection = (player.transform.position - transform.position).normalized * bulletForce;

            rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        Destroy(gameObject, 2);
    }

    public void Deflected()
    {
        deflected = true;
        rb.velocity = new Vector2(-moveDirection.x, -moveDirection.y) * bulletForce * 2;
        gameObject.layer = 11;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            player.TakeDamage(damageToGive);
            Destroy(gameObject);
        }

        if (deflected)
        {
            if (other.name.Equals("Melee"))
            {
                other.GetComponent<EnemyController>().TakeDamage(damageToGive);
                Destroy(gameObject);
            }

            if (other.name.Equals("Ranged"))
            {
                other.GetComponent<RangedEnemy>().TakeDamage(damageToGive);
                Destroy(gameObject);
            }
        }
    }

}
