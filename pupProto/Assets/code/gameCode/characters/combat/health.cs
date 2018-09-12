using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class health : MonoBehaviour {

    public heartDisplay heartDisplay;
    public int maxHP;
    public int currentHP {get; private set;}
    Character character;

	void Start () {
		heartDisplay.setupHearts(maxHP);
        currentHP = maxHP;
        character = GetComponent<Character>();
        character.events.combat.onGotHit += loseHP;
	}

    public void loseHP(attackData data)
    {
        --currentHP;
        heartDisplay.loseHeart();
    }

    
	
    
}
