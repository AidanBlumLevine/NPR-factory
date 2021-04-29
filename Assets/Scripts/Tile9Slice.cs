using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile9Slice : Tile
{
    public Mesh center, fFwd, fLft, fRgt, fBwd, cFwd, cLft, cRgt, cBwd;
    public Mesh fullFL, inFL, strFL, strLF, normFL, fullFR, inFR, strFR, strRF, normFR, fullBL, inBL, strBL, strLB, normBL, fullBR, inBR, strBR, strRB, normBR;

    public override void Set(Tile[] neighbors)
    {
        base.Set(neighbors);
        SetAppearance();
        TileManager.Instance.CornerUpdate(transform.position);
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
        List<Mesh> appearance = new List<Mesh>();

        bool forward = ShouldMerge(neighbors[(int)Dir.Forward]);
        bool back = ShouldMerge(neighbors[(int)Dir.Back]);
        bool left = ShouldMerge(neighbors[(int)Dir.Left]);
        bool right = ShouldMerge(neighbors[(int)Dir.Right]);

        //middle
        appearance.Add(center);

        //sides
        appearance.Add(forward ? fFwd : cFwd);
        appearance.Add(back ? fBwd : cBwd);
        appearance.Add(left ? fLft : cLft);
        appearance.Add(right ? fRgt : cRgt);

        //corners
        if (forward && left)
            appearance.Add(ShouldMerge(neighbors[(int)Dir.Forward].neighbors[(int)Dir.Left]) ? fullFL : inFL);
        else
            appearance.Add(forward ? strFL : left ? strLF : normFL);
        if (forward && right)
            appearance.Add(ShouldMerge(neighbors[(int)Dir.Forward].neighbors[(int)Dir.Right]) ? fullFR : inFR);
        else
            appearance.Add(forward ? strFR : right ? strRF : normFR);
        if (back && left)
            appearance.Add(ShouldMerge(neighbors[(int)Dir.Back].neighbors[(int)Dir.Left]) ? fullBL : inBL);
        else
            appearance.Add(back ? strBL : left ? strLB : normBL);
        if (back && right)
            appearance.Add(ShouldMerge(neighbors[(int)Dir.Back].neighbors[(int)Dir.Right]) ? fullBR : inBR);
        else
            appearance.Add(back ? strBR : right ? strRB : normBR);

        CombineInstance[] combine = new CombineInstance[9];
        MeshFilter mf = GetComponent<MeshFilter>();

        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        Matrix4x4 m = Matrix4x4.Rotate(rotation);
        for (int i = 0; i < 9; i++)
        {
            combine[i].mesh = appearance[i];
            combine[i].transform = m;
        }

        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine);
    }

    bool ShouldMerge(Tile t)
    {
        return t != null && t.mergeID == mergeID;
    }
}

