using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Connection {
    
    const float tangentLength = 65f;

	public Vector2 startPoint;
    public Vector2 endPoint;

    public behaviorNode parent;
    public behaviorNode child;

    behaviorTreeEditor editor;

    TreeData treeData;
    uint ID = uint.MaxValue;

    public Connection(behaviorTreeEditor editor, TreeData treeData)
    {
        this.editor = editor;
        this.treeData = treeData;
        startPoint = new Vector2();
        endPoint = new Vector2();
    }
    
    public Connection(behaviorTreeEditor editor, treeConnection treeConnection, TreeData treeData)
    {
        this.editor = editor;
        this.treeData = treeData;

        startPoint = new Vector2();
        endPoint = new Vector2();

        ID = treeConnection.ID;

        parent = editor.getNodeByID(treeConnection.parentID);
        child = editor.getNodeByID(treeConnection.childID);

        parent.addChild(this);
        child.addParent(this);
    }

    public void finishConnection()
    {
        if(parent == null || child == null) return;
        ID = treeData.createConnection(parent.ID, child.ID);
    }

    public void remove()
    {
        if(editor != null) editor.connections.Remove(this);
        if(parent != null) parent.removeChild(this);
        if(child != null) child.removeParent(this);
        treeData.removeConnection(ID);
    }

    public bool checkClick(Vector2 position)
    {
        float dist = HandleUtility.DistancePointBezier(position, startPoint, endPoint,
            new Vector3(startPoint.x, tangentLength + startPoint.y), new Vector3(endPoint.x, -tangentLength + endPoint.y));
        return dist < 6f;
    }

    public void draw()
    {
        //Handles.BeginGUI();
        Handles.DrawBezier(startPoint, endPoint,
            new Vector3(startPoint.x, tangentLength + startPoint.y), 
            new Vector3(endPoint.x, -tangentLength + endPoint.y), 
            Color.white, null, 2f);
        //Handles.EndGUI();
    }
}
