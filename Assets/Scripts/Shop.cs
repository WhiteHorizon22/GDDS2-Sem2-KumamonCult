﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    LevelManager levelManager;
    PlayerController player;
    CheckpointController checkpoint;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    void SpendPoints()
    {
        levelManager.LosePoints(1000);
    }

    void DestroyCheckpoint()
    {
        if (checkpoint.checkpointInUse == false)
        {
            levelManager.AddPoints(3000);
        }
    }

    void UseCheckPoint()
    {
        checkpoint.checkpointInUse = true;
    }
}