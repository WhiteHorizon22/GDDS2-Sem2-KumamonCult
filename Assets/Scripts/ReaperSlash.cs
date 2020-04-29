using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperSlash : MonoBehaviour
{
    PlayerController player;
    Collider2D col;
    public Camera cam;

    Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
        player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Collider2D touchedCollider = Physics2D.OverlapPoint(mousePos);
            if (col == touchedCollider)
            {
                player.reaperSlashTarget = mousePos;
                player.dashTargetSelected = true;
                Time.timeScale = 1f;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            this.gameObject.SetActive(false);
        }

        //Touch Tracking
        //if (Input.touchCount > 0)
        //{
        //    Touch touch = Input.GetTouch(0);
        //    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        //    touchPosition.z = 0f;

        //    if (touch.phase == TouchPhase.Began)
        //    {
        //        Collider2D touchedCollider = Physics2D.OverlapPoint(touchPosition);
        //        if (col == touchedCollider)
        //        {
        //            player.reaperSlashTarget = touchPosition;
        //        }
        //    }

        //    if (touch.phase == TouchPhase.Moved)
        //    {
        //        Collider2D touchedCollider = Physics2D.OverlapPoint(touchPosition);
        //        if (col == touchedCollider)
        //        {
        //            player.reaperSlashTarget = touchPosition;
        //        }
        //    }

        //    if (touch.phase == TouchPhase.Ended)
        //    {

        //    }
        //}


    }
}
