
using UnityEngine;


public class editorRoot : behaviorNode {

    public editorRoot(behaviorTreeEditor editor, string text, Vector2 nodePos, TreeData treeData) : base(editor, text, nodePos, treeData)
    {
        nodeSize.y = 64f;
        canHaveParents = false;
        canHaveChildren = true;
    }

    public editorRoot(behaviorTreeEditor editor, treeNode treeNode, TreeData treeData) : base(editor, treeNode, treeData)
    {
        nodeSize.y = 64f;
        canHaveParents = false;
        canHaveChildren = true;
    }

    protected override void doDraw(Vector2 transformedPosition)
    {
        GUI.BeginGroup(new Rect(transformedPosition, nodeSize), rootStyle);

        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(Vector2.up*6f, new Vector2(nodeSize.x, 20f)), nodeType);
        GUI.Box(new Rect(nodeSize.x / 2f - 20f, nodeSize.y - 38f, 40f, 30f), "", connectorStyle);

        GUI.EndGroup();
    }

    public override void addChild(Connection connection)
    {
        if (children.Count > 0) children[0].remove();
        base.addChild(connection);
    }
}
