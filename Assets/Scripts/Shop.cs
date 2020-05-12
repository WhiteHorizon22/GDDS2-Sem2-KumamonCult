using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    LevelManager levelManager;
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        player = FindObjectOfType<PlayerController>();
    }

    public void Heal()
    {
        if (levelManager.pointsCount >= 1000)
        {
            player.Heal(6);
            levelManager.LosePoints(1000);
        }
    }

    public void LeaveShop()
    {
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }
}
