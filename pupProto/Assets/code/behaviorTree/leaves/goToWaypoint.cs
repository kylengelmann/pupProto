using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BTENode("None", "Leaves/Go To Waypoint")]
public class goToWaypoint : BehaviorTreeNode {
    Character character;
    
    [BTEField("Destination Index")]
    public float destIdx;

    Transform dest;

    [BTEField("Success Threshold")]
    public float successThresh;

    bool isThere;
    
	public goToWaypoint(BehaviorTree tree, treeNode node) : base(tree, node)
    {
        character = tree.GetComponent<Character>();
        destIdx = node.getProperty("Destination Index").Float;
        dest = tree.GetComponent<waypointList>().waypoints[Mathf.RoundToInt(destIdx)];
        successThresh = node.getProperty("Success Threshold").Float;
    }

    protected override void onStart()
    {
        Vector2 pos = character.transform.position;
        Vector2 goal = dest.position;
        Vector2 move = goal - pos;
        isThere = Mathf.Abs(move.x) < successThresh * successThresh;
    }

    protected override Status update()
    {
        if(destIdx == null || isThere)
        {
            return Status.failure;
        }
        Vector2 pos = character.transform.position;
        Vector2 goal = dest.position;
        Vector2 move = goal - pos;
        if(Mathf.Abs(move.x) < successThresh*successThresh)
        {
            return Status.success;
        }
        character.events.move.setMove.Invoke(Mathf.Sign(move.x));

        return Status.running;
    }

    protected override void onStop()
    {
        character.events.move.setMove.Invoke(0f);
    }
}
