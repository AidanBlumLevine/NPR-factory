using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile9Slice : Tile
{
    public Mesh center, edgeFlat, edgeRounded, cornerFlat, cornerRounded, cornerInverse, cornerStraightLeft, cornerStraightRight;

    public override void Set(Tile[] neighbors)
    {
        base.Set(neighbors);
        SetAppearance();
        TileManager.Instance.RegisterCornerUpdate(this, transform.position);
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

    void SetAppearance()
    {
        CombineInstance[] combine = new CombineInstance[9];

        bool forward = ShouldMerge(neighbors[Forward]);
        bool back = ShouldMerge(neighbors[Back]);
        bool left = ShouldMerge(neighbors[Left]);
        bool right = ShouldMerge(neighbors[Right]);

        //middle
        combine[0].mesh = center;
        combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));
        //sides
        combine[1].mesh = forward ? edgeFlat : edgeRounded;
        combine[1].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));
        combine[2].mesh = back ? edgeFlat : edgeRounded;
        combine[2].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 180, 0));
        combine[3].mesh = left ? edgeFlat : edgeRounded;
        combine[3].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 270, 0));
        combine[4].mesh = right ? edgeFlat : edgeRounded;
        combine[4].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 90, 0));

        //corners
        if (forward && left)
        {
            combine[5].mesh = ShouldMerge(neighbors[Forward].neighbors[Left]) ? cornerFlat : cornerInverse;
            combine[5].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 270, 0));
        }
        else
        {
            combine[5].mesh = forward ? cornerStraightRight : left ? cornerStraightLeft : cornerRounded;
            combine[5].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 270, 0));
        }
        if (forward && right)
        {
            combine[6].mesh = ShouldMerge(neighbors[Forward].neighbors[Right]) ? cornerFlat : cornerInverse;
            combine[6].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));
        }
        else
        {
            combine[6].mesh = forward ? cornerStraightLeft : right ? cornerStraightRight : cornerRounded;
            combine[6].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));
        }
        if (back && left)
        {
            combine[7].mesh = ShouldMerge(neighbors[Back].neighbors[Left]) ? cornerFlat : cornerInverse;
            combine[7].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 180, 0));
        }
        else
        {
            combine[7].mesh = back ? cornerStraightLeft : left ? cornerStraightRight : cornerRounded;
            combine[7].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 180, 0));
        }
        if (back && right)
        {
            combine[8].mesh = ShouldMerge(neighbors[Back].neighbors[Right]) ? cornerFlat : cornerInverse;
            combine[8].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 90, 0));
        }
        else
        {
            combine[8].mesh = back ? cornerStraightRight : right ? cornerStraightLeft : cornerRounded;
            combine[8].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, 90, 0));
        }

        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine);
        GetComponent<MeshCollider>().sharedMesh = mf.mesh;
    }

    bool ShouldMerge(Tile t)
    {
        return t != null && t.mergeID == mergeID;
    }
}

