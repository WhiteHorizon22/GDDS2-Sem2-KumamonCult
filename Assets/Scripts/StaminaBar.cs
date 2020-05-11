﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Mana mana;
    private Image barImage;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        barImage = transform.Find("Bar").GetComponent<Image>();

        mana = new Mana();
    }

    private void Update()
    {
        mana.Update();

        barImage.fillAmount = mana.GetManaNormalized();
    }

    public class Mana
    {
        public const int MANA_MAX = 100;

        public float manaAmount;
        float manaRegenAmount;

        float manaToDrain;
        float amountToDrain;
        float initialMana;

        PlayerController player;

        void Start()
        {
            player = FindObjectOfType<PlayerController>();
        }

        public Mana()
        {
            manaAmount = 0;
            manaRegenAmount = 2f;
        }
        public void Update()
        {
            if (Input.GetButton("Fire2") && manaAmount > 0)
            {
                manaAmount -= manaRegenAmount * Time.unscaledDeltaTime;
            }
            else
            {
                manaAmount -= manaRegenAmount * Time.unscaledDeltaTime;
                manaAmount = Mathf.Clamp(manaAmount, 0f, MANA_MAX);
            }
        }

        public void IncreaseMana(int amount)
        {
            if (manaAmount < MANA_MAX)
            {
                manaAmount += amount;
            }
        }

        public float GetManaNormalized()
        {
            return manaAmount / MANA_MAX;
        }
    }
}