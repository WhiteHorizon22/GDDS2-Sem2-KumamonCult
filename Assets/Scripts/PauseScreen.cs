using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseScreen : MonoBehaviour
{
    public GameObject pauseScreen;
    private PlayerController player;
    private LevelManager theManager;
    public AudioMixer audioMixer;

    public static bool GamePaused = false;

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        Debug.Log(Time.timeScale);
        GamePaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        pauseScreen.SetActive(false);
        GamePaused = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        theManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
}
