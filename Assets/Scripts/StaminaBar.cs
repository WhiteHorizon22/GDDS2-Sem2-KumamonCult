using System.Collections;
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
            manaRegenAmount = 5f;
        }
        public void Update()
        {
            manaAmount -= manaRegenAmount * Time.deltaTime;
            manaAmount = Mathf.Clamp(manaAmount, 0f, MANA_MAX);
        }

        public void IncreaseMana(int amount)
        {
            if (manaAmount < MANA_MAX)
            {
                manaAmount += amount;
            }
        }
        public void DecreaseMana(int amount)
        {
            if (manaAmount > 0)
            {
                manaAmount -= amount;
            }
            else
            {
                manaAmount = 0;
            }
        }

        public float GetManaNormalized()
        {
            return manaAmount / MANA_MAX;
        }
    }
}