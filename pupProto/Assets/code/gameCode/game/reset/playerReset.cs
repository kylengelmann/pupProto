using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerReset : MonoBehaviour {

    public Vector2 resetPosition;
    Character character;

    private void Start()
    {
        character = gameObject.GetComponentInHierarchy<Character>();
        resetPosition = transform.position;
    }

    void onReset()
    {
        character.velocity = Vector2.zero;
        character.transform.position = new Vector3(resetPosition.x, resetPosition.y, character.transform.position.z);
    }
}
