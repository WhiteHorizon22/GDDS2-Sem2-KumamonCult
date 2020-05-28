using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    TouchIntegratedPlayerControl player;
    LevelManager theManager;
    public int damageToGive;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<TouchIntegratedPlayerControl>();
        theManager = FindObjectOfType<LevelManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            player.TakeDamage(damageToGive);
            theManager.Respawn();
        }
        
        if (other.name.Equals("Melee"))
        {
            other.GetComponent<EnemyController>().Die();
        }

        if (other.name.Equals("Ranged"))
        {
            other.GetComponent<RangedEnemy>().Die();
        }
    }
}
