using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public float waitToRespawn;
    public PlayerController thePlayer; // Makes reference to an object of PlayerController
    public GameObject deathSplosion;

    public int coinCount; // Keep track of number of coins that the player collected
    public Text coinText;

    public Text livesText;
    public int lifeCount;

    // Make a reference to the 3 heart images
    public Image heart1;
    public Image heart2;
    public Image heart3;

    private bool respawning;

    // Store sprtie images heartFull, heartHalf and heartEmpty
    public Sprite heartFull;
    public Sprite heartHalf;
    public Sprite heartEmpty;

    public AudioSource coinSound;
    public AudioSource levelMusic;
    public AudioSource gameOverMusic;

    public GameObject gameOverScreen; //Referring to the Game Over Screen game object

    //PlayerPrefs
    int coinsTransferred;
    string livesLeft;
    int thisScene;
    Scene getScene;

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = FindObjectOfType<PlayerController>();

        getScene = SceneManager.GetActiveScene();

        thisScene = getScene.buildIndex;

        thePlayer.currentHealth = thePlayer.maxHealth;

        //StartCoroutine("SetUpPlayer");

        //coinText.text = "Coins: " + coinCount;

        livesText.text = "Lives: " + lifeCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (thePlayer.currentHealth <= 0 && !respawning) //starts Respawn() if health is 0 or below
        {
            Respawn();
            respawning = true;
        }
    }

    public void Respawn()
    {
        if (thePlayer.currentHealth > 0 || lifeCount > 0)
        {
            // If you still have lives left, respawn
            StartCoroutine("RespawnCo");  // In the () is the string name of the Coroutine
        }
        else
        {
            // If you do not have anymore lives, save the scene for retry, then open up the gameover screen
            PlayerPrefs.SetInt("lastLevel", thisScene);
            thePlayer.gameObject.SetActive(false);  // Deactivate the player in the world
            Instantiate(deathSplosion, thePlayer.transform.position, thePlayer.transform.rotation);
            gameOverScreen.SetActive(true);
            levelMusic.Stop();
            gameOverMusic.Play();
        }
    }

    public IEnumerator RespawnCo()
    {
        thePlayer.gameObject.SetActive(false); // Deactivate Player in the world 

        // Create Object
        Instantiate(deathSplosion, thePlayer.transform.position, thePlayer.transform.rotation);

        thePlayer.currentHealth = thePlayer.maxHealth; //Set health to max

        TakeLives(-1); //Take away 1 Life

        yield return new WaitForSeconds(waitToRespawn); // How many seconds we want the game to wait for

        UpdateHeartMeter();

        //healthCount = maxHealth;
        respawning = false;
        //UpdateHeartMeter(); //Update the heart meter when player respawns
        thePlayer.transform.position = thePlayer.respawnPosition; // Move player to respawn position
        thePlayer.rb.velocity = new Vector2(0, 0);
        thePlayer.doubleJumpUsed = false;
        thePlayer.gameObject.SetActive(true); // Reactivate Player in the world
    }

    public void AddCoins(int coinsToAdd)
    {
        //coinCount = coinCount + coinsToAdd;
        coinCount += coinsToAdd; //Short form

        coinText.text = "Coins: " + coinCount;

    }

    public void AddLives(int livesToGive)
    {
        lifeCount += livesToGive;

        livesText.text = "Lives: " + lifeCount;

        coinSound.Play();
    }

    public void TakeLives(int livesToTake)
    {
        lifeCount += livesToTake;

        livesText.text = "Lives: " + lifeCount;

        coinSound.Play();
    }

    //public IEnumerator SetUpPlayer()
    //{
    //    print("setting up");

    //    yield return new WaitForSeconds(0.2f);

    //    coinCount = PlayerPrefs.GetInt("coinsTransfered");
    //    //coinText.text = "Coins: " + coinCount;

    //    if (PlayerPrefs.GetInt("livesLeft") >= 0)
    //    {
    //        lifeCount = PlayerPrefs.GetInt("livesLeft");
    //        livesText.text = "Lives: " + lifeCount;
    //    }
    //}

    public void UpdateHeartMeter()
    {
        switch (thePlayer.currentHealth)
        {
            //When healthCount = 600, full health
            case 6:
                heart1.sprite = heartFull;
                heart2.sprite = heartFull;
                heart3.sprite = heartFull;
                break;

            //Take away half of the heart when player gets hit once
            case 5:
                heart1.sprite = heartFull;
                heart2.sprite = heartFull;
                heart3.sprite = heartHalf;
                break;

            case 4:
                heart1.sprite = heartFull;
                heart2.sprite = heartFull;
                heart3.sprite = heartEmpty;
                break;

            case 3:
                heart1.sprite = heartFull;
                heart2.sprite = heartHalf;
                heart3.sprite = heartEmpty;
                break;

            case 2:
                heart1.sprite = heartFull;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;
                break;

            case 1:
                heart1.sprite = heartHalf;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;
                break;

            case 0:
                heart1.sprite = heartEmpty;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;
                break;

            default:
                heart1.sprite = heartEmpty;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;
                break;
        }
    }
}
