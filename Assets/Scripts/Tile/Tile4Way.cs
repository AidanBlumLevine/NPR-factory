using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile4Way : Tile
{
    public Mesh straight, tpipe, fourway, bend, core, cap;

    bool oldf, oldb, oldl, oldr;
    public override void Set(Tile[] neighbors)
    {
        flow.connections[0].enabled = true;
        flow.connections[1].enabled = true;
        flow.connections[2].enabled = true;
        flow.connections[3].enabled = true;
        flow.connections[4].enabled = true;
        flow.connections[5].enabled = true;
        base.Set(neighbors);
        SetAppearance();
    }

    protected override void AddNeighbor(Tile tile, int index)
    {
        base.AddNeighbor(tile, index);
        SetAppearance();
    }

    protected override void RemoveNeighbor(int index)
    {
        base.RemoveNeighbor(index);
        SetAppearance();
    }

    public override void TileUpdate()
    {
        SetAppearance();
    }

    protected override bool ShouldMerge(Tile t, int dirTowardsOther)
    {
        return t != null && flow.CanConnect(t.flow, dirTowardsOther);
    }

    void SetAppearance()
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        bool forward = ShouldMerge(neighbors[Forward], Forward);
        if (forward)
            flow.Connect(neighbors[Forward].flow, Forward);
        else
            flow.Disconnect(Forward);
        forward = flow.connections[Forward].attachedTo != null;

        bool back = ShouldMerge(neighbors[Back], Back);
        if (back)
            flow.Connect(neighbors[Back].flow, Back);
        else
            flow.Disconnect(Back);
        back = flow.connections[Back].attachedTo != null;

        bool left = ShouldMerge(neighbors[Left], Left);
        if (left)
            flow.Connect(neighbors[Left].flow, Left);
        else
            flow.Disconnect(Left);
        left = flow.connections[Left].attachedTo != null;

        bool right = ShouldMerge(neighbors[Right], Right);
        if (right)
            flow.Connect(neighbors[Right].flow, Right);
        else
            flow.Disconnect(Right);
        right = flow.connections[Right].attachedTo != null;

        int count = (forward ? 1 : 0) + (back ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

        CombineInstance[] combine = new CombineInstance[1];
        combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));

        if (forward && back && left && right)
            combine[0].mesh = fourway;
        if (!forward && !back && !left && !right)
            combine[0].mesh = core;

        if (count == 1)
        {
            if (forward)
            {
                combine[0].mesh = cap;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 90, 0));
            }
            if (back)
            {
                combine[0].mesh = cap;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 270, 0));
            }
            if (left)
            {
                combine[0].mesh = cap;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));
            }
            if (right)
            {
                combine[0].mesh = cap;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 180, 0));
            }
        }
        if (count == 2)
        {
            if (forward && back)
            {
                combine[0].mesh = straight;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 90, 0));
            }
            if (left && right)
            {
                combine[0].mesh = straight;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));
            }
            if (forward && left)
            {
                combine[0].mesh = bend;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));
            }
            if (forward && right)
            {
                combine[0].mesh = bend;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 90, 0));
            }
            if (left && back)
            {
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 270, 0));
                combine[0].mesh = bend;
            }
            if (right && back)
            {
                combine[0].mesh = bend;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 180, 0));
            }
        }
        if (count == 3)
        {
            if (!forward)
            {
                combine[0].mesh = tpipe;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 180, 0));
            }
            if (!back)
            {
                combine[0].mesh = tpipe;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));
            }
            if (!left)
            {
                combine[0].mesh = tpipe;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 90, 0));
            }
            if (!right)
            {
                combine[0].mesh = tpipe;
                combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 270, 0));
            }
        }
        mf.mesh.CombineMeshes(combine);
        GetComponent<MeshCollider>().sharedMesh = mf.mesh;
    }
}

