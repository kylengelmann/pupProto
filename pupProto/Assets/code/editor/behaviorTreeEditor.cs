using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class behaviorTreeEditor : EditorWindow {

    const float scrollSpeed = -.008f;

    Color backgroundColor = new Color(.2f, .2f, .2f);

    GUIStyle nodeStyle;
    GUIStyle rootStyle;
    GUIStyle connectorStyle;

    [MenuItem("Window/Behavior Tree")]
    public static void ShowWindow()
    {
        GetWindow(typeof(behaviorTreeEditor));
    }

    behaviorNode root;
    public List<behaviorNode> nodes;
    public List<Connection> connections;

    float scale;
    Vector2 translation;

    bool settingChild;

    public TreeData data;
    public BehaviorTree tree;

    private void OnEnable()
    {
        behaviorNode.nodeStyle = new GUIStyle();
        behaviorNode.nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
        behaviorNode.nodeStyle.border = new RectOffset(12, 12, 12, 12);

        behaviorNode.failureStyle = new GUIStyle();
        behaviorNode.failureStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node6.png") as Texture2D;
        behaviorNode.failureStyle.border = new RectOffset(12, 12, 12, 12);

        behaviorNode.successStyle = new GUIStyle();
        behaviorNode.successStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node3.png") as Texture2D;
        behaviorNode.successStyle.border = new RectOffset(12, 12, 12, 12);

        behaviorNode.runningStyle = new GUIStyle();
        behaviorNode.runningStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node4.png") as Texture2D;
        behaviorNode.runningStyle.border = new RectOffset(12, 12, 12, 12);

        behaviorNode.rootStyle = new GUIStyle();
        behaviorNode.rootStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        behaviorNode.rootStyle.border = new RectOffset(12, 12, 12, 12);

        behaviorNode.connectorStyle = new GUIStyle();
        behaviorNode.connectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        behaviorNode.connectorStyle.border = new RectOffset(12, 12, 12, 12);

        background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        background.SetPixel(0, 0, backgroundColor);
        background.Apply();

        changeData();

        OnSelectionChange();

        EditorApplication.playModeStateChanged += playModeStateChanged;
    }

    void playModeStateChanged(PlayModeStateChange change)
    {
        if(change == PlayModeStateChange.EnteredPlayMode)
        {
            OnSelectionChange();
        }
    }

    private void OnSelectionChange()
    {
        if(Selection.activeGameObject != null)
        tree = Selection.activeGameObject.GetComponent<BehaviorTree>();
        if(tree != null && tree.tree != null && tree.tree != data) 
        {
            data = tree.tree;
            changeData();
        }
    }

    private void changeData()
    {
        scale = 1f;
        translation = Vector2.zero;
        nodes = new List<behaviorNode>();
        connections = new List<Connection>();
        if(data == null) return;
        foreach(treeNode node in data.nodes)
        {
            nodes.Add(new behaviorNode(this, node, data));
        }
        foreach(treeConnection connection in data.connections)
        {
            connections.Add(new Connection(this, connection, data));
        }

        root = nodes[0];

        scale = 1f;
        translation = new Vector2(position.width/2f, position.height/2f);
    }

    Texture2D background;

    bool shouldRepaint;

    public behaviorNode getNodeByID(uint ID)
    {
        foreach(behaviorNode node in nodes)
        {
            if(node.ID == ID) return node;
        }
        return null;
    }

    private void Update()
    {
        Repaint();
    }

    private void OnGUI() {

        if(background == null) {
            background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            background.SetPixel(0, 0, backgroundColor);
            background.Apply();
        }

        GUI.DrawTexture(new Rect(Vector2.zero, position.size), background);
        float spacing = Mathf.Pow(5f, Mathf.Ceil(Mathf.Log(1f/scale) / Mathf.Log(5f)));

        drawGrid(.5f, 20f*spacing, position.size);
        drawGrid(1f, 100f*spacing, position.size);

        handleEvents(Event.current);

        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        foreach (behaviorNode node in nodes)
        {
            node.draw(translation, scale, Event.current.type);
        }

        foreach (Connection connection in connections)
        {
            connection.draw();
        }
        if(newConnection != null) newConnection.draw();

        GUI.Box(new Rect(Vector2.zero, new Vector2(320f, 30f)), "");
        TreeData newData = (TreeData)EditorGUI.ObjectField(new Rect(new Vector2(10f, 7.5f), new Vector2(300f, 15f)), "Editing Behavior Tree", data, typeof(TreeData), true);
        if(newData != data) {
            data = newData;
            changeData();
        }

        if(shouldRepaint){
            Repaint();
            shouldRepaint = false;
        }

        if(tree != null && tree.nodeStatus != null)
        {
            tree.nodeStatus[0] = (BehaviorTreeNode.Status)(-1);
        }
    }

    bool isCreatingConnection;
    Connection newConnection;

    void handleEvents(Event e)
    {
        wantsMouseMove = isCreatingConnection;
        switch (e.type)
        {
            case EventType.MouseDown:
                mouseDown(e.button, e.mousePosition);
                break;
            case EventType.MouseDrag:
                shouldRepaint = true;
                if(clickedNode != null)
                {
                    clickedNode.drag(e.delta, scale);
                }
                else
                {
                    translation += e.delta/scale;
                }
                break;
            case EventType.MouseMove:
                if(isCreatingConnection && newConnection != null)
                {
                    if(newConnection.parent == null)
                    {
                        newConnection.startPoint = e.mousePosition;
                    }
                    else
                    {
                        newConnection.endPoint = e.mousePosition;
                    }

                    shouldRepaint = true;
                }
                break;
            case EventType.ScrollWheel:
                Vector2 transformedPivot = e.mousePosition/scale - translation;

                scale *= Mathf.Exp(scrollSpeed*e.delta.y);
                scale = Mathf.Clamp(scale, .01f, 4.9f);

                translation = e.mousePosition/scale - transformedPivot;

                shouldRepaint = true;
                break;
            default:
                break;
        }
    }

    behaviorNode clickedNode;
    Connection clickedConnection;

    void mouseDown(int button, Vector2 position)
    {
        shouldRepaint = true;
        clickedNode = null;
        clickedConnection = null;
        foreach(behaviorNode node in nodes)
        {
            if(node.checkClick(position))
            {
                clickedNode = node;
            }
        }
        foreach (Connection connection in connections)
        {
            if (connection.checkClick(position))
            {
                clickedConnection = connection;
            }
        }

        switch(button)
        {
            case 0:
                leftClick(clickedNode, position);
                break;
            case 1:
                rightClick(clickedNode, clickedConnection, position);
                break;
        }

        isCreatingConnection = false;
        if (newConnection != null) newConnection.remove();
        newConnection = null;
    }

    void leftClick(behaviorNode clickedNode, Vector2 position)
    {
        if (clickedNode != null)
        {
            nodes.Remove(clickedNode);
            nodes.Add(clickedNode);

            if (isCreatingConnection && clickedNode != null)
            {
                if (newConnection.parent == null && clickedNode != newConnection.child && clickedNode.childrenMode != treeNode.ChildrenMode.None)
                {
                    clickedNode.addChild(newConnection);
                    connections.Add(newConnection);
                    newConnection.finishConnection();
                    newConnection = null;
                }
                else if (newConnection.child == null && clickedNode != newConnection.parent && clickedNode.canHaveParents)
                {
                    clickedNode.addParent(newConnection);
                    connections.Add(newConnection);
                    newConnection.finishConnection();
                    newConnection = null;
                }
            }
        }
    }

    void rightClick(behaviorNode clickedNode, Connection clickedConnection, Vector2 position)
    {
        if(Application.isPlaying) return;
        if(clickedConnection != null)
        {
            clickedNode = null;
        }
        GenericMenu genericMenu = new GenericMenu();
        if (clickedNode != null)
        {
            if (clickedNode.childrenMode != treeNode.ChildrenMode.None)
                genericMenu.AddItem(new GUIContent("Add Child"), false, () => {
                    isCreatingConnection = true;
                    newConnection = new Connection(this, data);
                    clickedNode.addChild(newConnection);
                });
            if (clickedNode.canHaveParents)
                genericMenu.AddItem(new GUIContent("Add Parent"), false, () => {
                    isCreatingConnection = true;
                    newConnection = new Connection(this, data);
                    clickedNode.addParent(newConnection);
                });
            if (clickedNode != root)
                genericMenu.AddItem(new GUIContent("Remove Node"), false, () => {
                    clickedNode.remove();
                    Repaint();
                });
        }
        else if(clickedConnection != null)
        {
            genericMenu.AddItem(new GUIContent("Remove Connection"), false, () => {
                clickedConnection.remove();
                Repaint();
            });
        }
        else if(data!=null)
        {
            makeNodeCreationMenu(genericMenu, "Create Node/", position);
        }

        genericMenu.ShowAsContext();
    }
    
    string behaviorDir = "Assets/code/characters/behavior/behaviorTree/";

    void makeNodeCreationMenu(GenericMenu menu, string menuRoot, Vector2 position)
    {
        string[] files = Directory.GetFiles(behaviorDir + "composites/", "*.*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (file.Contains(".meta")) continue;
            string path = file.Remove(0, (behaviorDir + "composites/").Length);
            path = path.Split('.')[0];
            path = path.Replace('\\', '/');
            string[] nameSplit = path.Split('/');
            string name = nameSplit[nameSplit.Length - 1];
            menu.AddItem(new GUIContent(menuRoot + "composites/" + path), false, () => makeOneChild(position, name));
        }
        files = Directory.GetFiles(behaviorDir + "decorators/", "*.*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (file.Contains(".meta")) continue;
            string path = file.Remove(0, (behaviorDir + "decorators/").Length);
            path = path.Split('.')[0];
            path = path.Replace('\\', '/');
            string[] nameSplit = path.Split('/');
            string name = nameSplit[nameSplit.Length - 1];
            menu.AddItem(new GUIContent(menuRoot + "decorators/" + path), false, () => makeOneChild(position, name));
        }
        //dir = new DirectoryInfo(behaviorDir + "filters/");
        //info = dir.GetFiles("*.*", SearchOption.AllDirectories);

        files = Directory.GetFiles(behaviorDir + "filters/", "*.*", SearchOption.AllDirectories);
        foreach(string file in files)
        {
            if (file.Contains(".meta")) continue;
            string path = file.Remove(0, (behaviorDir + "filters/").Length);
            path = path.Split('.')[0];
            path = path.Replace('\\', '/');
            string[] nameSplit = path.Split('/');
            string name = nameSplit[nameSplit.Length - 1];
            menu.AddItem(new GUIContent(menuRoot + "filters/" + path), false, () => makeOneChild(position, name));
        }
        //foreach (FileInfo f in info)
        //{
        //    if (f.Name.Contains(".meta")) continue;
        //    string path = f.FullName.Split('.')[0];
        //    string [] nameSplit = path.Split('\\');
        //    string name = nameSplit[nameSplit.Length-1];
        //    menu.AddItem(new GUIContent(menuRoot + "filters/" + path), false, () => makeOneChild(position, name));
        //}
        files = Directory.GetFiles(behaviorDir + "leaves/", "*.*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (file.Contains(".meta")) continue;
            string path = file.Remove(0, (behaviorDir + "leaves/").Length);
            path = path.Split('.')[0];
            path = path.Replace('\\', '/');
            string[] nameSplit = path.Split('/');
            string name = nameSplit[nameSplit.Length - 1];
            menu.AddItem(new GUIContent(menuRoot + "leaves/" + path), false, () => makeOneChild(position, name));
        }
    }

    void makeComposite(Vector2 screenPosition, string label)
    {
        Repaint();
        Vector2 pos = screenPosition / scale - translation;
        System.Type type = System.Type.GetType(label + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        FieldInfo info = type.GetField("serializableProperties");
        if(info == null)
        {
            info = typeof(BehaviorTreeNode).GetField("serializableProperties");
        }
        nodes.Add(new behaviorNode(this, label, pos, data, treeNode.ChildrenMode.Many, true, (List<serializableProperty>)info.GetValue(null)));
    }

    void makeOneChild(Vector2 screenPosition, string label)
    {
        Repaint();
        Vector2 pos = screenPosition / scale - translation;
        System.Type type = System.Type.GetType(label + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        FieldInfo info = type.GetField("serializableProperties");
        if (info == null)
        {
            info = typeof(BehaviorTreeNode).GetField("serializableProperties");
        }
        nodes.Add(new behaviorNode(this, label, pos, data, treeNode.ChildrenMode.One, true, (List<serializableProperty>)info.GetValue(null)));
    }

    void makeLeaf(Vector2 screenPosition, string label)
    {
        Repaint();
        Vector2 pos = screenPosition / scale - translation;
        System.Type type = System.Type.GetType(label + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        FieldInfo info = type.GetField("serializableProperties");
        if (info == null)
        {
            info = typeof(BehaviorTreeNode).GetField("serializableProperties");
        }
        nodes.Add(new behaviorNode(this, label, pos, data, treeNode.ChildrenMode.None, true, (List<serializableProperty>)info.GetValue(null)));
    }

    void drawGrid(float opacity, float spacing, Vector2 windowSize)
    {
        spacing = spacing * scale;
        int numX = Mathf.CeilToInt(windowSize.x / (spacing));
        int numY = Mathf.CeilToInt(windowSize.y / (spacing));

        Handles.BeginGUI();
        Handles.color = new Color(.5f, .5f, .5f, opacity);

        float offX = (translation.x * scale) % (spacing);
        float offY = (translation.y * scale) % (spacing);

        for (int i = 0; i < numX; ++i)
        {
            float x = i * spacing + offX;
            Handles.DrawLine(new Vector3(x, 0f, 0f), new Vector3(x, position.height, 0));
        }
        for (int i = 0; i < numY; ++i)
        {
            float y = i * spacing + offY;
            Handles.DrawLine(new Vector3(0f, y, 0f), new Vector3(position.width, y, 0));
        }

        Handles.EndGUI();
    }

}
