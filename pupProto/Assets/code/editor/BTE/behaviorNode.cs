using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class behaviorNode {

    const float nodeWidth = 250;

    float nodeHeight = 1f;

    public bool canHaveParents;

    public treeNode.ChildrenMode childrenMode;

    public static List<behaviorNode> nodes;

    protected List<Connection> parents;
    protected List<Connection> children;

    public static GUIStyle nodeStyle;
    public static GUIStyle successStyle;
    public static GUIStyle failureStyle;
    public static GUIStyle runningStyle;
    public static GUIStyle rootStyle;
    public static GUIStyle connectorStyle;

    protected GUIStyle currentStyle;

    protected Vector2 position;

    protected Vector2 transformedPosition;

    protected string nodeName;

    TreeData treeData;
    public uint ID;

    behaviorTreeEditor editor;

    List<serializableProperty> serializableProperties;

    public behaviorNode(behaviorTreeEditor editor, string nodeName, string nodeType, Vector2 nodePos, TreeData treeData, treeNode.ChildrenMode childrenMode, bool canHaveParents, List<serializableProperty> properties)
    {
        this.editor = editor;
        this.nodeName = nodeName;
        position = nodePos;
        transformedPosition = position;

        this.treeData = treeData;
        ID = treeData.CreateNode(nodePos, nodeName, nodeType, childrenMode, canHaveParents, properties);

        children = new List<Connection>();
        parents = new List<Connection>();

        serializableProperties = properties;
        this.childrenMode = childrenMode;
        this.canHaveParents = canHaveParents;
        if (ID == 0) currentStyle = rootStyle;
        else currentStyle = nodeStyle;

    }

    public behaviorNode(behaviorTreeEditor editor, treeNode treeNode, TreeData treeData)
    {
        this.editor = editor;
        nodeName = treeNode.nodeName;
        position = treeNode.position;
        transformedPosition = position;

        this.treeData = treeData;
        ID = treeNode.ID;

        children = new List<Connection>();
        parents = new List<Connection>();

        serializableProperties = treeNode.serializableProperties;
        childrenMode = treeNode.childrenMode;
        canHaveParents = treeNode.canHaveParents;

        if (ID == 0) currentStyle = rootStyle;
        else currentStyle = nodeStyle;
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
        return (new Rect(transformedPosition.x, transformedPosition.y, nodeWidth, nodeHeight)).Contains(mousePos);
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

    public void draw(Vector2 translation, float scale, EventType eType)
    {
        //if(eType != EventType.Repaint && eType != EventType.Layout) return;
        transformedPosition = (position + translation) * scale;
        setStyle();

        GUI.BeginGroup(new Rect(transformedPosition, new Vector2(nodeWidth, nodeHeight)), currentStyle);

        GUILayout.BeginArea(new Rect(Vector2.zero, new Vector2(nodeWidth, nodeHeight)));


        EditorGUILayout.BeginVertical(GUILayout.Width(nodeWidth));

        GUILayout.Space(6f);
        if (canHaveParents)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box("", connectorStyle, GUILayout.Width(40f), GUILayout.Height(30f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Space(8f);
        }

        GUILayout.Label(nodeName);

        if(serializableProperties != null && serializableProperties.Count > 0) {
            GUILayout.Space(6f);
            foreach (serializableProperty prop in serializableProperties)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(prop.Name);
                switch(prop.shownProperty)
                {
                    case serializableProperty.Types.Float:
                        prop.Float = EditorGUILayout.FloatField(prop.Float, GUILayout.Width(50f));
                        break;
                    case serializableProperty.Types.Bool:
                        prop.Bool = EditorGUILayout.Toggle(prop.Bool);
                        break;
                    case serializableProperty.Types.String:
                        prop.String = EditorGUILayout.TextField(prop.String);
                        break;
                    case serializableProperty.Types.Transform:
                        prop.Transform = (Transform)EditorGUILayout.ObjectField(prop.Transform, typeof(Transform), true, GUILayout.Width(100f));
                        //prop.Object = EditorGUILayout.ObjectField(prop.Name, prop.Object, typeof(Object), true);
                        break;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(6f);
            }
        }

        

        switch (childrenMode)
        {
            case treeNode.ChildrenMode.Many:
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box("", connectorStyle, GUILayout.Width(120f), GUILayout.Height(30f));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                break;
            case treeNode.ChildrenMode.One:
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box("", connectorStyle, GUILayout.Width(40f), GUILayout.Height(30f));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                break;
            default:
                GUILayout.Space(8f);
                break;
        }

        GUILayout.Space(6f);

        EditorGUILayout.EndVertical();
        if (eType == EventType.Repaint)
        {
            nodeHeight = GUILayoutUtility.GetLastRect().height;
        }

        GUILayout.EndArea();

        GUI.EndGroup();
        updateConnections(transformedPosition);
    }

    public virtual void addParent(Connection connection)
    {
        if(!canHaveParents) return;
        parents.Add(connection);
        connection.child = this;
    }

    public virtual void addChild(Connection connection)
    {
        if (childrenMode == treeNode.ChildrenMode.None) return;
        if (childrenMode == treeNode.ChildrenMode.One)
        {
            if (children.Count > 0) children[0].remove();
        }
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
        Vector2 basePos = transformedPosition + new Vector2(nodeWidth/2f - 60f, nodeHeight - 22f);
        int count = children.Count;
        float spacing = 120f/(count + 1f);
        for(int i = 0; i < count; ++i)
        {
            children[i].startPoint = basePos + Vector2.right*(i+1)*spacing;
        }
        foreach(Connection parent in parents)
        {
            parent.endPoint = transformedPosition + new Vector2(nodeWidth / 2f, 18f);
        }
    }
}
