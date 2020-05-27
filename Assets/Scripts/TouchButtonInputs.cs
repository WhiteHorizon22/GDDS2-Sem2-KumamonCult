using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchButtonInputs : MonoBehaviour
{
    PlayerController player;
    private float screenWidth;

    // Start is called before the first frame update
    void Start()
    {
        screenWidth = Screen.width;
        player = FindObjectOfType<PlayerController>();
    }
    void Update()
    {
        int i = 0;
        //loop over every touch found
        while(i < Input.touchCount)
        {
            if (Input.GetTouch(i).position.x < (screenWidth / 3) * 2)
            {
                //move Player Left
                PlayerMovement(-1.0f);
            }
            else if ((Input.GetTouch(i).position.x > (screenWidth / 3) * 2) && Input.GetTouch(i).position.x < (screenWidth / 2))
            {
                //move Player Right
                PlayerMovement(1.0f);
            }
        }
    }
    void PlayerMovement(float horizontalInput)
    {
        player.rb.velocity = new Vector2(horizontalInput * player.moveSpeed * Time.deltaTime, player.rb.velocity.y);
    }
}
