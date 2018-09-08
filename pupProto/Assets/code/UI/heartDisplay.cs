using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heartDisplay : MonoBehaviour {

	public GameObject heartImage;
    public Sprite hasHeart;
    public Sprite lostHeart;

    Image[] hearts;
    int currentHP;

    public void setupHearts(int numHearts)
    {
        hearts = new Image[numHearts];
        currentHP = numHearts;
        for(int i = 0; i < numHearts; ++i)
        {
            hearts[i] = Instantiate(heartImage, transform).GetComponent<Image>();
            hearts[i].sprite = hasHeart;
        }
    }

    public void loseHeart()
    {
        hearts[--currentHP].sprite = lostHeart;
    }

    public void resetHearts()
    {
        int numHearts = hearts.Length;
        for(; currentHP < numHearts; ++currentHP)
        {
            hearts[currentHP].sprite = hasHeart;
        }
    }
}
