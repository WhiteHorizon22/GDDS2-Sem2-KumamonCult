using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    // Store sprites images flag open and close
    public Sprite flagOpen;
    public Sprite flagClosed;

    private SpriteRenderer theSpriteRenderer;

    public bool checkpointActive;

    // Start is called before the first frame update
    void Start()
    {
        //Get andd Store a reference to the SpriteRenderer component
        theSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check for when the Player enters the checkpoint
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // Set sprite in the SpriteRenderer to flagOpen sprite
            theSpriteRenderer.sprite = flagOpen;
            checkpointActive = true;
        }
    }
}
