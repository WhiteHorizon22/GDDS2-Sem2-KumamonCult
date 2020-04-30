using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    Rigidbody2D rb;
    [Header("Dash")]
    public float dashSpeed;
    public float currentDashDuration;
    public float dashDuration;
    public bool dashing;
    // Start is called before the first frame update
    void Start()
    {
        currentDashDuration = dashDuration;
        rb = GetComponent<Rigidbody2D>();
        dashing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentDashDuration <= 0)
            {
                rb.velocity = Vector2.zero;
                currentDashDuration = dashDuration;

            }
            else
            {
                dashing = true;
            }
        }

        if (dashing)
        {
            if (transform.localScale.x > 1)
            {
                transform.position = new Vector2(1 * dashSpeed, rb.velocity.y);
            }
            else if (transform.localScale.x < 1)
            {
                transform.position = Vector2.left * dashSpeed;
            }
        }
    }
}
