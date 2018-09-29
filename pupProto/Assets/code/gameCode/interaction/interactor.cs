using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactor : MonoBehaviour {

    [SerializeField] float interactionDistance = .3f;

    [SerializeField] float interactorHeight = 1.2f;

    [SerializeField] float interactorHeightOffset = 0;

    [SerializeField] LayerMask interactionMask;

    public bool DEBUG;

    Character character;

    Vector2 castBox;

    private void Start()
    {
        character = gameObject.GetComponentInHierarchy<Character>();
        character.events.interaction.doInteract += interact;

        castBox = new Vector2(.01f, interactorHeight);
        
    }

    void interact()
    {
        

        Vector2 castOrigin = new Vector2(transform.position.x, transform.position.y + interactorHeightOffset);

        Vector2 castDir = Vector2.right*transform.lossyScale.x;

        #if UNITY_EDITOR
        
        if(DEBUG) {
            Debug.DrawLine(castOrigin + new Vector2(0, -interactorHeight*.5f), (castOrigin + new Vector2(0, -interactorHeight * .5f)) + castDir*interactionDistance, Color.green, 2f);
            Debug.DrawLine(castOrigin, castOrigin + castDir* interactionDistance, Color.green, 2f);
            Debug.DrawLine(castOrigin + new Vector2(0, interactorHeight * .5f), (castOrigin + new Vector2(0, interactorHeight * .5f)) + castDir * interactionDistance, Color.green, 2f);
        }
        #endif


        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, castBox, 0, castDir, interactionDistance, interactionMask.value);
        if(hit.collider != null)
        {
            interactable hitInteractable = hit.collider.gameObject.GetComponent<interactable>();
            if(hitInteractable != null) {
                hitInteractable.interact(character.gameObject);
                character.events.interaction.onInteraction.Invoke(hitInteractable);
            }
        }
    }
}


public class interactionEvents
{
    public safeAction doInteract = new safeAction();
    public safeAction<interactable> onInteraction = new safeAction<interactable>();
}


