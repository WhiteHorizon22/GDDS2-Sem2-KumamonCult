using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointController : MonoBehaviour
{
    // Store sprites images flag open and close
    public Sprite flagOpen;
    public Sprite flagClosed;
    public Shop shopMenu;
    public GameObject popUpButton;
    LevelManager theManager;

    public bool checkpointActive;

    private SpriteRenderer theSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //Get andd Store a reference to the SpriteRenderer component
        theSpriteRenderer = GetComponent<SpriteRenderer>();
        checkpointActive = false;
        theManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkpointActive)
        {
            // Set sprite in the SpriteRenderer to flagOpen sprite
            theSpriteRenderer.sprite = flagOpen;
        }
    }

    // Check for when the Player enters the checkpoint
    void OnTriggerStay2D(Collider2D other)
    {
        //if (other.tag == "Player")
        //{
        //    popUpButton.SetActive(true);
        //}
        //else
        //{
        //    popUpButton.SetActive(false);
        //}

        if (other.tag == "Player")
        {
            checkpointActive = true;
        }
        //else
        //{
        //    return;
        //}
    }

}
