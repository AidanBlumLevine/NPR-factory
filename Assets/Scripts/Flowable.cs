using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[System.Serializable]
public class Flowable : MonoBehaviour
{
    public static int Unset = 0;
    public static int In = 1;
    public static int Out = -1;

    Material lineMat;

    [HideInInspector]
    public bool flowable;
    [HideInInspector]
    public FlowConnection[] connections = new FlowConnection[6];
    List<FlowConnection> connected = new List<FlowConnection>();
    void Awake()
    {
        foreach (FlowConnection c in connections)
            c.parent = this;

        lineMat = new Material(Shader.Find("Diffuse"));
    }

    void OnRenderObject()
    {

        for (int i = 0; i < 6; i++)
        {
            if (connections[i].attachedTo != null && connections[i].direction != 0)
            {
                GL.PushMatrix();
                lineMat.SetPass(0);
                GL.MultMatrix(Matrix4x4.TRS(transform.localPosition, Quaternion.LookRotation(Tile.DirVectors[i], Vector3.up), transform.localScale));
                GL.Begin(GL.LINES);
                GL.Color(Color.black);
                Vector3 center = new Vector3(0, -.2f, 0);
                //Vector3 center = new Vector3(0, 0, 0);
                if (connections[i].direction == 1)
                {
                    GL.Vertex(new Vector3(.1f, 0, -.2f) + center);
                    GL.Vertex(new Vector3(0, 0, -.35f) + center);
                    GL.Vertex(new Vector3(0, 0, -.35f) + center);
                    GL.Vertex(new Vector3(-.1f, 0, -.2f) + center);
                    GL.Vertex(new Vector3(-.1f, 0, -.2f) + center);
                    GL.Vertex(new Vector3(.1f, 0, -.2f) + center);
                }
                else
                {
                    GL.Vertex(new Vector3(.1f, 0, -.35f) + center);
                    GL.Vertex(new Vector3(0, 0, -.2f) + center);
                    GL.Vertex(new Vector3(0, 0, -.2f) + center);
                    GL.Vertex(new Vector3(-.1f, 0, -.35f) + center);
                    GL.Vertex(new Vector3(-.1f, 0, -.35f) + center);
                    GL.Vertex(new Vector3(.1f, 0, -.35f) + center);
                }
                GL.End();
                GL.PopMatrix();
            }
        }

    }

    public bool CanConnect(Flowable other, int dirTowardsOther)
    {
        bool simpleCases = other.connections[5 - dirTowardsOther].enabled && connections[dirTowardsOther].enabled;
        bool alreadyConnected = other.connections[5 - dirTowardsOther].attachedTo == connections[dirTowardsOther] && other.connections[5 - dirTowardsOther] == connections[dirTowardsOther].attachedTo;
        bool compatibleDirections = other.connections[5 - dirTowardsOther].direction * connections[dirTowardsOther].direction <= 0;
        return alreadyConnected || simpleCases && compatibleDirections;
    }

    public void Connect(Flowable other, int dirTowardsOther)
    {
        FlowConnection thisside = connections[dirTowardsOther];
        FlowConnection attachedTo = other.connections[5 - dirTowardsOther];
        thisside.attachedTo = attachedTo;
        attachedTo.attachedTo = thisside;
        if(attachedTo.enforced){
            thisside.direction = attachedTo.direction * -1;
        }
        // if (thisside.attachedTo == null) // && attachedTo.attachedTo == null
        // {
        //     connected.Add(thisside);
        //     other.connected.Add(attachedTo);

        //     if (connected.Count == 3)
        //         for (int i = 0; i < 6; i++)
        //             if (connections[i].attachedTo != null)
        //                 connections[i].SetCrossAndPropagate(Unset);
        //     if (other.connected.Count == 3)
        //         for (int i = 0; i < 6; i++)
        //             if (other.connections[i].attachedTo != null)
        //                 other.connections[i].SetCrossAndPropagate(Unset);


        //     thisside.attachedTo = attachedTo;
        //     attachedTo.attachedTo = thisside;

        //     if (!attachedTo.enforced)
        //         attachedTo.SetAndPropagate(-thisside.direction);
        //     if (!thisside.enforced)
        //         thisside.SetAndPropagate(-attachedTo.direction);

        //     ResolveErrors();
        //     other.ResolveErrors();
        // }
    }

    public void Disconnect(int dirTowardsOther, bool forcedDisconnect = false)
    {
        FlowConnection thisside = connections[dirTowardsOther];
        FlowConnection attachedTo = thisside.attachedTo;
        
        if (attachedTo != null && attachedTo.attachedTo != null)
        {
        //     Flowable other = attachedTo.parent;

            thisside.attachedTo = null;
            attachedTo.attachedTo = null;

        //     if (!thisside.enforced)
        //         thisside.SetAndPropagate(Unset);
        //     if (!attachedTo.enforced)
        //         attachedTo.SetAndPropagate(Unset);

        //     connected.Remove(thisside);
        //     other.connected.Remove(attachedTo);

        //     if (connected.Count == 1 && !connected[0].enforced)
        //         connected[0].SetCrossAndPropagate(Unset);
        //     if (other.connected.Count == 1 && !other.connected[0].enforced)
        //         other.connected[0].SetCrossAndPropagate(Unset);

        //     if (forcedDisconnect)
        //     {
        //         requestingVisualUpdate = true;
        //         other.requestingVisualUpdate = true;
        //     }

        //     ResolveErrors();
        //     other.ResolveErrors();
        }
    }

    // public void ResolveErrors()
    // {
    //     if (connected.Count == 2 && connected[0].direction + connected[1].direction != 0)
    //     {
    //         if (connected[0].direction != Unset)
    //             connected[1].SetCrossAndPropagate(-connected[0].direction);
    //         else
    //             connected[0].SetCrossAndPropagate(-connected[1].direction);
    //     }

    //     for (int i = 0; i < 6; i++)
    //     {
    //         FlowConnection c = connections[i];
    //         if (c.attachedTo != null && c.direction + c.attachedTo.direction != 0)
    //         {
    //             Disconnect(i, true);
    //         }
    //     }
    //     requestingVisualUpdate = true;
    // }

    // public void SetPropagation(FlowConnection set)
    // {
    //     if (connected.Count == 2)
    //     {
    //         FlowConnection propagateTo = connected.Find(c => c != set);
    //         propagateTo.SetCrossAndPropagate(-set.direction);
    //         ResolveErrors();
    //     }
    //     requestingVisualUpdate = true;
    // }
}

[System.Serializable]
public class FlowConnection
{
    public bool enabled;
    public int direction;
    public FlowConnection attachedTo;
    public bool enforced;
    public Flowable parent;
    public FlowConnection(int direction)
    {
        this.direction = direction;
    }

    // public void SetAndPropagate(int dir)
    // {
    //     if (enforced)
    //     {
    //         //just wait for resolveErrors to break this connection
    //         if (dir * direction > 0)
    //             return;

    //         //dont flip the propagation if its already right
    //         else if (dir * direction == 0 && attachedTo != null)
    //             attachedTo.SetAndPropagate(-direction);
    //         return;
    //     }

    //     if (direction != dir)
    //     {
    //         direction = dir;
    //         parent.SetPropagation(this);
    //     }
    // }

    // public void SetCrossAndPropagate(int dir)
    // {
    //     if (enforced)
    //         return;

    //     if (direction != dir)
    //     {
    //         direction = dir;
    //         if (attachedTo != null)
    //             attachedTo.SetAndPropagate(-dir);
    //     }
    // }
}









#if UNITY_EDITOR
[CustomEditor(typeof(Flowable))]
[CanEditMultipleObjects]
public class FlowableEditor : Editor
{
    bool enabled = false;
    bool[] enableds = new bool[6];
    FlowOptions[] fOpts = new FlowOptions[6];
    Flowable flowable;
    public Material lineMat;
    void OnEnable()
    {
        flowable = (Flowable)target;
        enabled = flowable.flowable;
        for (int i = 0; i < 6; i++)
        {
            enableds[i] = flowable.connections[i].enabled;
            fOpts[i] = (FlowOptions)flowable.connections[i].direction;
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Enable Flow");
        enabled = EditorGUILayout.Toggle(enabled);
        flowable.flowable = enabled;
        if (enabled)
        {
            GUILayout.Space(10f);
            GUILayout.Label("Flow Settings");
            for (int i = 0; i < 6; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(DirLabels[i]);
                enableds[i] = EditorGUILayout.Toggle(enableds[i]);

                if (enableds[i])
                {
                    fOpts[i] = (FlowOptions)EditorGUILayout.EnumPopup(fOpts[i]);
                    if (flowable.connections[i] == null)
                        flowable.connections[i] = new FlowConnection((int)fOpts[i]);
                    else
                        flowable.connections[i].direction = (int)fOpts[i];
                }
                flowable.connections[i].enabled = enableds[i];
                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(flowable);
            EditorSceneManager.MarkSceneDirty(flowable.gameObject.scene);
        }
    }

    string[] DirLabels = {
        "Up",
        "Back",
        "Left",
        "Right",
        "Forward",
        "Down"
    };

    enum FlowOptions
    {
        Unset = 0,
        In = -1,
        Out = 1

    }
}
#endif