using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchButtonInputs : MonoBehaviour
{
    TouchIntegratedPlayerControl player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<TouchIntegratedPlayerControl>();
    }

    public void Attack()
    {
        if (Time.time >= player.nextAttackTime)
        {
            player.Attack();
            player.nextAttackTime = Time.time + 1f / player.attackRate;
        }
    }
    public void Jump()
    {
        player.Jump();
    }
}
