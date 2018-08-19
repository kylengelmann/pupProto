﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TreeData : ScriptableObject {
    public List<treeNode> nodes = new List<treeNode>{
        new treeNode
        {
            position = Vector2.zero,
            Type = "Root",
            GUINodeType = "editorRoot",
            ID = 0
        }
    };
    public List<treeConnection> connections = new List<treeConnection>();
    public List<uint> removedIDs = new List<uint>();
    public uint nextID = 1;

    public treeNode getNode(uint ID)
    {
        foreach(treeNode node in nodes)
        {
            if(node.ID == ID)
            {
                return node;
            }
        }
        return null;
    }

    public treeConnection getConnection(uint ID)
    {
        foreach (treeConnection connection in connections)
        {
            if (connection.ID == ID)
            {
                return connection;
            }
        }
        return null;
    }

    public uint CreateNode(Vector2 position, string Type, string GUINodeType)
    {
        treeNode newNode = new treeNode
        {
            position = position,
            Type = Type,
            GUINodeType = GUINodeType
        };
        if(removedIDs.Count > 0)
        {
            newNode.ID = removedIDs[0];
            removedIDs.RemoveAt(0);
        }
        else
        {
            newNode.ID = nextID++;
        }
        nodes.Add(newNode);
        return newNode.ID;
    }

    public uint createConnection(uint pID, uint cID)
    {
        treeConnection newConnection = new treeConnection
        {
            parentID = pID,
            childID = cID
        };

        if (removedIDs.Count > 0)
        {
            newConnection.ID = removedIDs[0];
            removedIDs.RemoveAt(0);
        }
        else
        {
            newConnection.ID = nextID++;
        }

        connections.Add(newConnection);
        return newConnection.ID;
    }

    public void removeNode(uint ID)
    {
        treeNode node = getNode(ID);
        if(node == null) return;
        removedIDs.Add(node.ID);
        nodes.Remove(node);
    }
    
    public void removeConnection(uint ID)
    {
        treeConnection connection = getConnection(ID);
        if(connection == null) return;
        removedIDs.Add(connection.ID);
        connections.Remove(connection);
    }
}

[System.Serializable]
public class treeNode
{
    public uint ID;
    public Vector2 position;
    public string Type;
    public string GUINodeType;
}

[System.Serializable]
public class treeConnection
{
    public uint ID;
    public uint parentID;
    public uint childID;
}
