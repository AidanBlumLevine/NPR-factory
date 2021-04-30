using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile4Way : Tile
{
    public Mesh straight, tpipe, fourway, bend, core, cap;

    public override void Set(Tile[] neighbors)
    {
        base.Set(neighbors);
        SetAppearance();
    }

    protected override void AddNeighbor(Tile tile, int index)
    {
        base.AddNeighbor(tile, index);
        SetAppearance();
    }

    public override void TileUpdate()
    {
        SetAppearance();
    }

    void SetAppearance()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        bool forward = ShouldMerge(neighbors[(int)Dir.Forward]);
        bool back = ShouldMerge(neighbors[(int)Dir.Back]);
        bool left = ShouldMerge(neighbors[(int)Dir.Left]);
        bool right = ShouldMerge(neighbors[(int)Dir.Right]);
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
    }

    bool ShouldMerge(Tile t)
    {
        return t != null && t.mergeID == mergeID;
    }
}

