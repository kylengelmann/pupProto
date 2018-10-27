using UnityEngine;


public class physicsAnimator : MonoBehaviour {

    Character character;

    public Vector2 velocity;
    public Vector2 acceleration;

    private Vector2 characterVelocity;

    public enum animationMode
    {
        velocity,
        acceleration
    }

    public animationMode mode;

    Animator anim;

    int currentState = -1;

	void Awake () {
		character = gameObject.GetComponentInHierarchy<Character>();
        anim = gameObject.GetComponentInHierarchy<Animator>();
	}

    private void OnEnable()
    {
        characterVelocity = character.velocity;
        currentState = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
        character.events.physicsAnimation.onEnable.Invoke();
        character.gravity = 0f;
    }

    private void OnDisable()
    {
        character.events.physicsAnimation.onDisable.Invoke();
    }

    void Update () {
        if(currentState != anim.GetCurrentAnimatorStateInfo(0).fullPathHash)
        {
            enabled = false;
            return;
        }
        switch (mode) {
            case animationMode.velocity:
                characterVelocity = velocity;
                characterVelocity.x *= Mathf.Sign(transform.lossyScale.x);
                break;
            case animationMode.acceleration:
                Vector2 dV = acceleration * Time.deltaTime;
                dV.x *= Mathf.Sign(transform.lossyScale.x);
                characterVelocity += dV;
                break;
            default:
                return;
        }
        character.velocity = characterVelocity;
    }

    public void setVelocity()
    {
        character.velocity = velocity;
        character.velocity.x *= Mathf.Sign(transform.lossyScale.x);
        characterVelocity = character.velocity;
    }
}


public class physicsAnimationEvents
{
    public safeAction onEnable = new safeAction();
    public safeAction onDisable = new safeAction();
}
