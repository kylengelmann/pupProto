using UnityEngine;

public class hittableCharacter : MonoBehaviour, IHittable
{
    Character character;
    void Start()
    {
        character = transform.parent.GetComponent<Character>();
    }

    public void Hit(attackData attackData)
    {
        character.events.combat.onGotHit.Invoke(attackData);
    }
}


public interface IHittable {
    void Hit(attackData attackData);
}