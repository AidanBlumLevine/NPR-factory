using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
// using UnityEditor.SceneManagement;
using UnityEngine;

[System.Serializable]
public class Flowable : MonoBehaviour
{
    public static int Unset = 0;
    public static int In = -1;
    public static int Out = 1;

    Material lineMat;

    [HideInInspector]
    public bool flowable;
    [HideInInspector]
    public FlowConnection[] connections = new FlowConnection[6];
    // public List<FlowConnection> connected = new List<FlowConnection>();\
    public bool machineMode = true;
    [HideInInspector]
    public List<TrackNode> trackGraph = new List<TrackNode>();
    void Awake()
    {
        foreach (FlowConnection c in connections)
            c.parent = this;

        lineMat = new Material(Shader.Find("Diffuse"));

        if (machineMode)
            trackGraph.Add(new TrackNode(transform.position, true));
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

    void OnDrawGizmos()
    {
        foreach (TrackNode tn in trackGraph)
        {
            Gizmos.color = new Color(0, 0, 0, .75f);
            Gizmos.DrawSphere(tn.pos, .02f);
            foreach (TrackNode o in tn.connected)
            {
                Debug.DrawLine(tn.pos, o.pos, Gizmos.color);
            }
        }
    }

    void OnDestroy()
    {
        foreach (TrackNode tn in trackGraph)
            tn.deleted = true;
    }

    public bool CanConnect(Flowable other, int dirTowardsOther)
    {
        bool simpleCases = other.connections[5 - dirTowardsOther].enabled && connections[dirTowardsOther].enabled;
        bool alreadyConnected = other.connections[5 - dirTowardsOther].attachedTo == connections[dirTowardsOther] && other.connections[5 - dirTowardsOther] == connections[dirTowardsOther].attachedTo;
        bool compatibleDirections = other.connections[5 - dirTowardsOther].direction * connections[dirTowardsOther].direction <= 0;
        return alreadyConnected || simpleCases && compatibleDirections;
    }

    //this happens for both objects
    public void Connect(Flowable other, int dirTowardsOther)
    {
        FlowConnection thisside = connections[dirTowardsOther];
        if (thisside.attachedTo == null)
        {
            thisside.entry = new TrackNode((Vector3)Tile.DirVectors[dirTowardsOther] / -2 + transform.position, true, thisside);
            trackGraph.Add(thisside.entry);
            ConstructLinks(thisside);

            FlowConnection attachedTo = other.connections[5 - dirTowardsOther];
            thisside.attachedTo = attachedTo;

            if (attachedTo.enforced)
            {
                thisside.direction = attachedTo.direction * -1;
            }
        }
    }

    //this needs to disconnect both sides at once
    public void Disconnect(int dirTowardsOther, bool forcedDisconnect = false)
    {
        FlowConnection thisside = connections[dirTowardsOther];
        FlowConnection attachedTo = thisside.attachedTo;

        if (attachedTo != null && attachedTo.attachedTo != null)
        {
            thisside.entry.Dissolve(trackGraph, true);
            thisside.entry = null;
            attachedTo.entry.Dissolve(attachedTo.parent.trackGraph, true);
            attachedTo.entry = null;
            if(!thisside.enforced) thisside.direction = Unset;
            if(!attachedTo.enforced) attachedTo.direction = Unset;

            thisside.attachedTo = null;
            attachedTo.attachedTo = null;

            // if (trackGraph.Count == 0)
            //     MakeSingleConnectionCap();
            // if (attachedTo.parent.trackGraph.Count == 0)
            //     attachedTo.parent.MakeSingleConnectionCap();
            if (trackGraph.Count == 1)
                HandleSingleConnection();
            if (attachedTo.parent.trackGraph.Count == 1)
                attachedTo.parent.HandleSingleConnection();
        }
    }

    public void ConstructLinks(FlowConnection fromConnection)
    {
        TrackNode from = fromConnection.entry;

        //destroy the floating nodes created by the ConnectedCount == 0 case below
        if (!machineMode && ConnectedCount() == 1)
            foreach (FlowConnection alreadyConnected in connections)
                if (alreadyConnected.attachedTo != null)
                    alreadyConnected.entry.connected[0].Dissolve(trackGraph, true);

        if (machineMode)
            from.Connect(trackGraph[0]);
        else if (ConnectedCount() == 0)
            HandleSingleConnection();
        else
        {
            foreach (FlowConnection toConnection in connections)
            {
                if (toConnection.attachedTo != null && toConnection != fromConnection)
                {
                    TrackNode to = toConnection.entry;

                    if (Vector3.Dot(to.pos - transform.position, from.pos - transform.position) == 0)
                    {
                        //90 degree turn
                        float contour = 50;

                        TrackNode current = from;
                        Vector3 start = from.pos;
                        Vector3 startControl = (from.pos + transform.position * contour) / (contour + 1);
                        Vector3 end = to.pos;
                        Vector3 endControl = (to.pos + transform.position * contour) / (contour + 1);

                        for (float t = .1f; t < 1; t += .1f)
                        {
                            Vector3 bez = Mathf.Pow(1 - t, 3) * start + 3 * Mathf.Pow(1 - t, 2) * t * startControl + 3 * (1 - t) * Mathf.Pow(t, 2) * endControl + Mathf.Pow(t, 3) * end;
                            current = current.Extrude(bez, trackGraph);
                        }
                        current.Connect(to);
                    }
                    else
                    {
                        //assuming straight
                        // Vector3 offset = (to.pos - from.pos) / 10;
                        // TrackNode current = from;
                        // for(int i = 1; i < 9; i++){
                        //     current.Extrude(current.pos + offset, trackGraph);
                        // }
                        // current.Connect(to);'
                        from.Connect(to);
                    }
                }
            }
        }
    }

    void HandleSingleConnection()
    {
        if(machineMode)
            return;

        TrackNode from = trackGraph[0];
        if (from.flowConnection == null)
        {
            from.Dissolve(trackGraph, true);
        }
        else
            from.Extrude((from.pos * 2 + transform.position) / 3, trackGraph, true);
    }

    int ConnectedCount()
    {
        int c = 0;
        foreach (FlowConnection co in connections)
            c += co.attachedTo == null ? 0 : 1;
        return c;
    }
}

[System.Serializable]
public class FlowConnection
{
    public bool enabled;
    public int direction;
    public FlowConnection attachedTo;
    public bool enforced;
    public Flowable parent;
    public TrackNode entry;
    public FlowConnection(int direction)
    {
        this.direction = direction;
    }
}


// #if UNITY_EDITOR
// [CustomEditor(typeof(Flowable))]
// [CanEditMultipleObjects]
// public class FlowableEditor : Editor
// {
//     bool enabled = false;
//     bool[] enableds = new bool[6];
//     FlowOptions[] fOpts = new FlowOptions[6];
//     Flowable flowable;
//     public Material lineMat;
//     void OnEnable()
//     {
//         flowable = (Flowable)target;
//         enabled = flowable.flowable;
//         for (int i = 0; i < 6; i++)
//         {
//             enableds[i] = flowable.connections[i].enabled;
//             fOpts[i] = (FlowOptions)flowable.connections[i].direction;
//         }
//     }

//     public override void OnInspectorGUI()
//     {
//         GUILayout.Label("Enable Flow");
//         enabled = EditorGUILayout.Toggle(enabled);
//         flowable.flowable = enabled;
//         if (enabled)
//         {
//             GUILayout.Space(10f);
//             GUILayout.Label("Flow Settings");
//             for (int i = 0; i < 6; i++)
//             {
//                 EditorGUILayout.BeginHorizontal();
//                 GUILayout.Label(DirLabels[i]);
//                 enableds[i] = EditorGUILayout.Toggle(enableds[i]);

//                 if (enableds[i])
//                 {
//                     fOpts[i] = (FlowOptions)EditorGUILayout.EnumPopup(fOpts[i]);
//                     if (flowable.connections[i] == null)
//                         flowable.connections[i] = new FlowConnection((int)fOpts[i]);
//                     else
//                         flowable.connections[i].direction = (int)fOpts[i];
//                 }
//                 flowable.connections[i].enabled = enableds[i];
//                 EditorGUILayout.EndHorizontal();
//             }
//         }

//         if (GUI.changed)
//         {
//             EditorUtility.SetDirty(flowable);
//             EditorSceneManager.MarkSceneDirty(flowable.gameObject.scene);
//         }
//     }

//     string[] DirLabels = {
//         "Up",
//         "Back",
//         "Left",
//         "Right",
//         "Forward",
//         "Down"
//     };

//     enum FlowOptions
//     {
//         Unset = 0,
//         In = -1,
//         Out = 1

//     }
// }
// #endif