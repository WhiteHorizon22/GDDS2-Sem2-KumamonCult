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

    }

    //New Game
    public void NewGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    //Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Back()
    {
         creditsScreen.SetActive(false);
    }

    public void Credits()
    {
        print("pressed");
        creditsScreen.SetActive(true);
    }
}
