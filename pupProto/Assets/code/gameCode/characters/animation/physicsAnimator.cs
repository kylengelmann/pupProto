using UnityEngine;


public class physicsAnimator : MonoBehaviour {

    Character character;

    public Vector2 velocity;
    public Vector2 acceleration;

    public enum animationMode
    {
        velocity,
        acceleration
    }

    public animationMode mode;

    Animator anim;

    int currentState = -1;

	void Start () {
		character = GetComponent<Character>();
        anim = GetComponent<Animator>();
	}

    private void OnEnable()
    {
        currentState = anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
        character.events.physicsAnimation.onEnable.Invoke();
    }

    private void OnDisable()
    {
        character.events.physicsAnimation.onEnable.Invoke();
    }

    void Update () {
        if(currentState != anim.GetCurrentAnimatorStateInfo(0).shortNameHash)
        {
            enabled = false;
            return;
        }
        switch (mode) {
            case animationMode.velocity:
                character.velocity = velocity;
                break;
            case animationMode.acceleration:
                character.velocity += acceleration*Time.deltaTime;
                break;
        }
	}

    public void setVelocity()
    {
        character.velocity = velocity;
    }
}


public class physicsAnimationEvents
{
    public safeAction onEnable = new safeAction();
    public safeAction onDisable = new safeAction();
}
