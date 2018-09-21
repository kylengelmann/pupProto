using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerReset : MonoBehaviour {

    Vector2 resetPosition;
    Character character;

    private void Start()
    {
        character = GetComponent<Character>();
    }

    void onReset()
    {
        character.velocity = Vector2.zero;
        character.transform.position = new Vector3(resetPosition.x, resetPosition.y, character.transform.position.z);
    }
}
