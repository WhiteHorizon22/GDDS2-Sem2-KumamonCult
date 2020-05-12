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
    }

    void SpendPoints()
    {
        levelManager.LosePoints(1000);
    }

    void LeaveShop()
    {
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }
}
