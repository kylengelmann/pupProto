using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TreeData : ScriptableObject {
    public List<treeNode> nodes = new List<treeNode>();
    public List<treeConnection> connections = new List<treeConnection>();
    public List<uint> removedIDs = new List<uint>();
    public uint nextID = 1;

    private void Awake()
    {
        if(nodes.Count == 0)
        {
            nodes.Add(
                new treeNode
                {
                    position = Vector2.zero,
                    nodeName = "Root",
                    Type = "Root",
                    ID = 0,
                    childrenMode = treeNode.ChildrenMode.One,
                    canHaveParents = false

                }
            );
            removedIDs = new List<uint>();
            nextID = 1;
        }
    }

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

    public uint CreateNode(Vector2 position, string nodeName, string Type, treeNode.ChildrenMode childrenMode, bool canHaveParents, List<serializableProperty> properties)
    {
        treeNode newNode = new treeNode
        {
            position = position,
            nodeName = nodeName,
            Type = Type,
            childrenMode = childrenMode,
            canHaveParents = canHaveParents,
            serializableProperties = properties
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
        updateRemoved();
    }

    void updateRemoved()
    {
        if(removedIDs[removedIDs.Count-1] == nextID - 1)
        {
            --nextID;
            removedIDs.RemoveAt(removedIDs.Count - 1);
            updateRemoved();
        }
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
    public string nodeName;
    public string Type;
    public enum ChildrenMode
    {
        None = 0,
        One = 1,
        Many = 2
    }
    public ChildrenMode childrenMode;
    public bool canHaveParents;

    public List<serializableProperty> serializableProperties;

    public serializableProperty getProperty(string name)
    {
        return serializableProperties.Find(sp => { return sp.Name == name;});
    }
}

[System.Serializable]
public class treeConnection
{
    public uint ID;
    public uint parentID;
    public uint childID;
}

[System.Serializable]
public class serializableProperty
{
    public string Name;
    public float Float;
    public bool Bool;
    public string String;
    public Transform Transform;

    public enum Types
    {
        Float,
        Bool,
        String,
        Transform,
    }

    public Types shownProperty;
}
