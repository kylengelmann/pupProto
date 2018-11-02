using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {


    public float gravity = 30;

    [HideInInspector] public Vector2 velocity;

    public characterEventHandler events = new characterEventHandler();
    [HideInInspector] public Vector2 groundNormal;

    abilitiesManager abilitiesManager = new abilitiesManager();

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public float airTime;

    protected virtual void Start()
    {
        abilitiesManager.init(events);
        groundNormal = Vector2.up;
    }

    public virtual void move(Vector2 dS) {}

    public virtual void dropThrough() {}

}



public class characterEvents
{
    public safeAction onGrounded = new safeAction();
    public safeAction onHardLand = new safeAction();
    public safeAction onLeaveGround = new safeAction();
    public safeAction<RaycastHit2D, RaycastHit2D> onPositionUpdate = new safeAction<RaycastHit2D, RaycastHit2D>();

}
