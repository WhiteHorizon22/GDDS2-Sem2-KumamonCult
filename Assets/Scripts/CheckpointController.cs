using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointController : MonoBehaviour
{
    // Store sprites images flag open and close
    public Sprite flagOpen;
    public Sprite flagClosed;
    public GameObject shopMenu;
    public GameObject popUpButton;

    private SpriteRenderer theSpriteRenderer;

    public bool checkpointActive;
    public bool checkpointInUse;

    // Start is called before the first frame update
    void Start()
    {
        //Get andd Store a reference to the SpriteRenderer component
        theSpriteRenderer = GetComponent<SpriteRenderer>();
        shopMenu.SetActive(false);
        popUpButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (checkpointInUse)
        {
            // Set sprite in the SpriteRenderer to flagOpen sprite
            theSpriteRenderer.sprite = flagOpen;
            checkpointActive = true;
    }

    // Check for when the Player enters the checkpoint
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            popUpButton.SetActive(true);
        }
        else
        {
            popUpButton.SetActive(false);
        }

        if (other.tag == "Player" && Input.GetKeyDown(KeyCode.J))
        {
            shopMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
