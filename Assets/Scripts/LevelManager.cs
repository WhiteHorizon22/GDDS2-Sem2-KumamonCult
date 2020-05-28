using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public float waitToRespawn;
    public TouchIntegratedPlayerControl thePlayer; // Makes reference to an object of PlayerController
    public GameObject deathSplosion;

    public int pointsCount; // Keep track of number of coins that the player collected
    public Text pointsText;

    //public Text livesText;
    //public int lifeCount;

    // Make a reference to the 3 heart images
    public Text healthText;

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
        thePlayer = FindObjectOfType<TouchIntegratedPlayerControl>();

        getScene = SceneManager.GetActiveScene();

        thisScene = getScene.buildIndex;

        thePlayer.currentHealth = thePlayer.maxHealth;

        //StartCoroutine("SetUpPlayer");

        pointsText.text = "Pts: " + pointsCount;

        UpdateHeartMeter();

        //livesText.text = "Lives: " + lifeCount;
    }

    // Update is called once per frame
    void Update()
    {
        //if (thePlayer.currentHealth <= 0 && !respawning) //starts Respawn() if health is 0 or below
        //{
        //    Respawn();
        //    respawning = true;
        //}
    }

    public void Respawn()
    {
         StartCoroutine("RespawnCo");  

        //else
        //{
        //    // If you do not have anymore lives, save the scene for retry, then open up the gameover screen
        //    //PlayerPrefs.SetInt("lastLevel", thisScene);
        //    //thePlayer.gameObject.SetActive(false);  // Deactivate the player in the world
        //    //Instantiate(deathSplosion, thePlayer.transform.position, thePlayer.transform.rotation);
        //    //gameOverScreen.SetActive(true);
        //    //levelMusic.Stop();
        //    //gameOverMusic.Play();
        //}
    }

    public IEnumerator RespawnCo()
    {
        thePlayer.gameObject.SetActive(false); // Deactivate Player in the world 

        // Create Object
        Instantiate(deathSplosion, thePlayer.transform.position, thePlayer.transform.rotation);

        if (thePlayer.currentHealth <= 0)
        {
            //TakeLives(-1); //Take away 1 Life
            thePlayer.currentHealth = thePlayer.maxHealth; //Set health to max
        }

        yield return new WaitForSeconds(waitToRespawn); // How many seconds we want the game to wait for

        UpdateHeartMeter();

        //healthCount = maxHealth;
        UpdateHeartMeter(); //Update the heart meter when player respawns
        thePlayer.transform.position = thePlayer.respawnPosition; // Move player to respawn position
        thePlayer.rb.velocity = new Vector2(0, 0);
        thePlayer.doubleJumpUsed = false;
        thePlayer.gameObject.SetActive(true); // Reactivate Player in the world
    }

    public void AddPoints(int pointsToAdd)
    {
        pointsCount += pointsToAdd; //Short form

        pointsText.text = "Pts: " + pointsCount;

    }

    public void LosePoints(int pointsToLose)
    {
        pointsCount -= pointsToLose; //Short form

        pointsText.text = "Pts: " + pointsCount;
    }

    //public void AddLives(int livesToGive)
    //{
    //    lifeCount += livesToGive;

    //    livesText.text = "Lives: " + lifeCount;

    //    //coinSound.Play();
    //}

    //public void TakeLives(int livesToTake)
    //{
    //    lifeCount += livesToTake;

    //    livesText.text = "Lives: " + lifeCount;

    //    if (lifeCount <= 0)
    //    {
    //        SceneManager.LoadScene("GameOver");
    //    }

    //    //coinSound.Play();
    //}

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
        healthText.text = "" + thePlayer.currentHealth;
    }
}
