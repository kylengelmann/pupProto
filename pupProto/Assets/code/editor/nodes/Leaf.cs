using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : behaviorNode
{

    public Leaf(behaviorTreeEditor editor, string text, Vector2 nodePos, TreeData treeData) : base(editor, text, nodePos, treeData)
    {
        nodeSize.y = 64f;

        canHaveParents = true;
        canHaveChildren = false;
    }
    
    public Leaf(behaviorTreeEditor editor, treeNode treeNode, TreeData treeData) : base(editor, treeNode, treeData)
    {
        nodeSize.y = 64f;

        canHaveParents = true;
        canHaveChildren = false;
    }

    protected override void doDraw(Vector2 transformedPosition)
    {
        GUI.BeginGroup(new Rect(transformedPosition, nodeSize), currentStyle);

        GUI.Box(new Rect(nodeSize.x / 2f - 20f, 6f, 40f, 30f), "", connectorStyle);
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(Vector2.up * 36f, new Vector2(nodeSize.x, 20f)), nodeType);

        GUI.EndGroup();
    }
}
