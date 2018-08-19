using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class behaviorNode {

    public bool canHaveParents { get; protected set; }
    public bool canHaveChildren { get; protected set; }

    public static List<behaviorNode> nodes;

    protected List<Connection> parents;
    protected List<Connection> children;


    protected Vector2 nodeSize = new Vector2(150, 200);

    public static GUIStyle nodeStyle;
    public static GUIStyle successStyle;
    public static GUIStyle failureStyle;
    public static GUIStyle runningStyle;
    public static GUIStyle rootStyle;
    public static GUIStyle connectorStyle;

    protected GUIStyle currentStyle;

    protected Vector2 position;

    protected Vector2 transformedPosition;

    protected string nodeType;

    TreeData treeData;
    public uint ID;

    behaviorTreeEditor editor;

    public behaviorNode(behaviorTreeEditor editor, string nodeType, Vector2 nodePos, TreeData treeData)
    {
        this.editor = editor;
        this.nodeType = nodeType;
        position = nodePos;
        transformedPosition = position;

        this.treeData = treeData;
        ID = treeData.CreateNode(nodePos, nodeType, GetType().ToString());

        children = new List<Connection>();
        parents = new List<Connection>();

        currentStyle = nodeStyle;
    }

    public behaviorNode(behaviorTreeEditor editor, treeNode treeNode, TreeData treeData)
    {
        this.editor = editor;
        nodeType = treeNode.Type;
        position = treeNode.position;
        transformedPosition = position;

        this.treeData = treeData;
        ID = treeNode.ID;

        children = new List<Connection>();
        parents = new List<Connection>();

        currentStyle = nodeStyle;
    }

    public void remove()
    {
        if(editor != null) editor.nodes.Remove(this);
        while(parents.Count > 0)
        {
            parents[0].remove();
        }
        while (children.Count > 0)
        {
           children[0].remove();
        }

        treeData.removeNode(ID);
    }

    public bool checkClick(Vector2 mousePos)
    {
        return (new Rect(transformedPosition, nodeSize)).Contains(mousePos);
    }

    public void drag(Vector2 delta, float scale)
    {
        position += delta/scale;

        editor.data.getNode(ID).position = position;
        
    }

    void setStyle()
    {
        if(ID == 0) return;
        if(editor.tree == null || editor.tree.nodeStatus == null) {
            currentStyle = nodeStyle;
            return;
        } else if (editor.tree.nodeStatus[0] == (BehaviorTreeNode.Status)(-1)) return;

        switch(editor.tree.nodeStatus[ID])
        {
            case BehaviorTreeNode.Status.failure:
                currentStyle = failureStyle;
                break;
            case BehaviorTreeNode.Status.success:
                currentStyle = successStyle;
                break;
            case BehaviorTreeNode.Status.running:
                currentStyle = runningStyle;
                break;
            default:
                currentStyle = nodeStyle;
                break;
        }

        editor.tree.nodeStatus[ID] = (BehaviorTreeNode.Status)(-1);
    }

    public void draw(Vector2 translation, float scale)
    {
        transformedPosition = (position + translation) * scale;
        setStyle();
        doDraw(transformedPosition);
        updateConnections(transformedPosition);
    }

    protected abstract void doDraw(Vector2 transformedPosition);

    public virtual void addParent(Connection connection)
    {
        if(!canHaveParents) return;
        parents.Add(connection);
        connection.child = this;
    }

    public virtual void addChild(Connection connection)
    {
        if (!canHaveChildren) return;
        //if(childrenType == ChildrenType.One)
        //{
        //    if(children.Count > 0) children[0].remove();
        //}
        connection.parent = this;
        children.Add(connection);
    }

    public void removeParent(Connection connection)
    {
        parents.Remove(connection);
        connection.child = null;
    }

    public void removeChild(Connection connection)
    {
        children.Remove(connection);
        connection.parent = null;
    }


    void updateConnections(Vector2 transformedPosition)
    {
        Vector2 basePos = transformedPosition + new Vector2(nodeSize.x/2f - 60f, nodeSize.y - 22f);
        int count = children.Count;
        float spacing = 120f/(count + 1f);
        for(int i = 0; i < count; ++i)
        {
            children[i].startPoint = basePos + Vector2.right*(i+1)*spacing;
        }
        foreach(Connection parent in parents)
        {
            parent.endPoint = transformedPosition + new Vector2(nodeSize.x / 2f, 18f);
        }
    }
}
