using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setCheckpoint : MonoBehaviour {

    playerReset reset;

    void Start()
    {
        Character character = gameObject.GetComponentInHierarchy<Character>();
        character.events.interaction.onInteraction += interact;
        
        reset = GetComponent<playerReset>();
    }

    public void interact(interactable interactable)
    {
        if(interactable is checkpoint)
        {
            checkpoint checkpoint = (checkpoint)interactable;
            reset.resetPosition = checkpoint.transform.position;
        }
    }

}