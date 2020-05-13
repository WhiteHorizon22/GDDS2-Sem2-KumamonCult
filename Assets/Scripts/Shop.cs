using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    LevelManager levelManager;
    PlayerController player;
    StaminaBar rageMeter;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        player = FindObjectOfType<PlayerController>();
        rageMeter = FindObjectOfType<StaminaBar>();
    }

    public void Heal()
    {
        if (levelManager.pointsCount >= 1000)
        {
            player.Heal(6);
            levelManager.LosePoints(1000);
        }
    }

    public void IncreaseRage()
    {
        if (levelManager.pointsCount >= 1000)
        {
            rageMeter.mana.IncreaseMana(20);
            levelManager.LosePoints(1000);
        }
    }

    public void IncreaseAttackRange()
    {
        if (levelManager.pointsCount >= 2000)
        {
            player.attackRangeRadius += 0.5f;
            levelManager.LosePoints(2000);
        }
    }

    public void EndDemo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LeaveShop()
    {
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }
}
