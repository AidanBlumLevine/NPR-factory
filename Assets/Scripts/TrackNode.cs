using System;
using System.Collections.Generic;
using UnityEngine;
public class TrackNode
{
    public Vector3 pos;
    public List<TrackNode> connected = new List<TrackNode>();
    public bool isEntry;
    public FlowConnection flowConnection;
    public Func<Item, bool> onRecieve;
    public bool deleted = false;
    public TrackNode(Vector3 pos, bool isEntry = false, FlowConnection flowConnection = null)
    {
        this.pos = pos;
        this.isEntry = isEntry;
        this.flowConnection = flowConnection;
    }

    public TrackNode Track(Item item, float moveDist)
    {
        float dist = (item.transform.position - pos).magnitude;
        if (dist < moveDist)
        {
            if(onRecieve != null){
                onRecieve(item);
            }

            List<TrackNode> options = new List<TrackNode>(connected);
            List<TrackNode> optionsEntries = new List<TrackNode>();
            if (flowConnection != null && !(flowConnection.attachedTo.enforced && flowConnection.attachedTo.direction == 1))
                options.AddRange(flowConnection.attachedTo.entry.connected);

            foreach (TrackNode opt in options)
                optionsEntries.Add(opt.NextEntry(this));

            float maxWeight = float.MinValue;
            TrackNode newTrack = null;
            for (int i = 0; i < options.Count; i++)
            {
                float conDot = Vector3.Dot(item.dir, (optionsEntries[i].pos - pos).normalized) / 2;
                float items = -item.NItems(optionsEntries[i].pos);

                FlowConnection fc = optionsEntries[i].flowConnection;
                float inout = 2 * (fc == null ? 0 : fc.direction);             
                
                if (conDot < -0.001) //this prevents spontaneous turn arounds
                    conDot = -100;

                float weight = conDot + items + inout;
                if (weight > maxWeight)
                {
                    maxWeight = weight;
                    newTrack = options[i];
                }
            }

            item.transform.position = pos;
            item.dir = (newTrack.pos - pos).normalized;
            return newTrack.Track(item, moveDist - dist);
        }

        item.transform.position = Vector3.MoveTowards(item.transform.position, pos, moveDist);
        return this;
    }

    public TrackNode Flip(Item item)
    {
        item.dir *= -1;
        float maxDot = float.MinValue;
        TrackNode newTrack = null;
        foreach (TrackNode opt in connected)
        {
            float conDot = Vector3.Dot(item.dir, (opt.pos - pos).normalized);
            if (conDot > maxDot)
            {
                maxDot = conDot;
                newTrack = opt;
            }
        }
        return newTrack;
    }

    public TrackNode NextEntry(TrackNode from)
    {
        if (isEntry)
            return this;
        foreach (TrackNode n in connected)
            if ((n.pos - from.pos).sqrMagnitude > .0001f)
                return n.NextEntry(this);

        return null;
    }

    public void Dissolve(List<TrackNode> graph, bool start = false)
    {
        if (!isEntry || start)
        {
            while (connected.Count > 0)
            {
                TrackNode removing = connected[0];
                connected.RemoveAt(0);
                removing.connected.Remove(this);
                removing.Dissolve(graph);
            }
            graph.Remove(this);
            deleted = true;
        }
    }

    public TrackNode Extrude(Vector3 newPos, List<TrackNode> graph, bool isEntry = false)
    {
        TrackNode newNode = new TrackNode(newPos, isEntry);
        graph.Add(newNode);
        Connect(newNode);
        return newNode;
    }

    public void Connect(TrackNode other)
    {
        connected.Add(other);
        other.connected.Add(this);
    }
}