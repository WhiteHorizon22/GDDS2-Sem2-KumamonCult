using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPrompter : MonoBehaviour
{
    public GameObject tutorialPrompt;

    LevelManager levelManager;

    TouchIntegratedPlayerControl thePlayer;

    bool understood;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        thePlayer = FindObjectOfType<TouchIntegratedPlayerControl>();
        understood = false;
    }

    public void Continue()
    {
        understood = true;
        tutorialPrompt.SetActive(false);
        Time.timeScale = 1f;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (!understood)
            {
                tutorialPrompt.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }
}
