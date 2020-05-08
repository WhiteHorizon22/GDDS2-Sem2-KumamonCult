using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    Vector2 moveDirection;
    Rigidbody2D rb; // Reference to Rigid 2D component in Bullet prefab
    private LevelManager theLevelManager; // Make reference to LevelManager
    public int damageToGive;
    PlayerController player;
    public float bulletForce;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        theLevelManager = FindObjectOfType<LevelManager>();

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
            theLevelManager.HurtPlayer(damageToGive);
            print(theLevelManager.healthCount);
        }
        else if (other.tag == "PlayerAttackRange" && player.basicAttackTrigger == true)
        {
            rb.velocity = new Vector2(-moveDirection.x, -moveDirection.y);
        }
        else if (other.tag == "Wall" || other.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "PlayerAttackRange" && player.basicAttackTrigger == true)
        {
            rb.velocity = new Vector2(-moveDirection.x, -moveDirection.y);
        }
    }
}
