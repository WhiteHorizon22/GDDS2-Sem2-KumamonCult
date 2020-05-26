using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    PlayerController player;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    LevelManager theManager;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;
    
    public GameObject chestOpen, chestClosed;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        player = FindObjectOfType<PlayerController>();
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
            OpenChest();
        }
    }

    void OpenChest()
    {
        Vector3 pos = transform.position;
        Quaternion rotation = transform.rotation;
        Destroy(chestClosed.gameObject);
        chestOpen.gameObject.SetActive(true);
        Debug.Log("It's opened.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "Player"))
        {
            if (player.usingGroundPound)
            {
                Debug.Log("Groundpound check");
                TakeDamage(player.slamDamage);
            }
            else if (player.dashing)
            {
                TakeDamage(player.dashDamage);
            }
        }
    }
}
