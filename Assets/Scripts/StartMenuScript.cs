using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    public GameObject creditsScreen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            creditsScreen.SetActive(false);
        }
    }

    //New Game
    public void NewGame()
    {
        SceneManager.LoadScene("TutorialLevel");
    }

    //Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturntoMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }

    public void Credits()
    {
        print("pressed");
        creditsScreen.SetActive(true);
    }
}
