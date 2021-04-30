using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int mergeID;
    public Tile[] neighbors;
    protected MeshRenderer renderer;

    public virtual void Set(Tile[] neighbors)
    {
        renderer = GetComponent<MeshRenderer>();


        this.neighbors = neighbors;
        for (int i = 0; i < 6; i++)
        {
            if (neighbors[i] != null)
            {
                neighbors[i].AddNeighbor(this, i);
            }
        }
        SetOutlineID();
    }

    protected virtual void AddNeighbor(Tile tile, int index)
    {
        neighbors[5 - index] = tile;
    }

    protected virtual void RemoveNeighbor(int index)
    {
        neighbors[5 - index] = null;
    }

    public virtual void TileUpdate() { }

    public virtual void Delete()
    {
        for (int i = 0; i < 6; i++)
        {
            if (neighbors[i] != null)
            {
                neighbors[i].RemoveNeighbor(i);
            }
        }
        Destroy(gameObject);
    }

    void SetOutlineID()
    {
        Random.InitState(mergeID);
        Color32 color = Random.ColorHSV();
        int colorPropertyId = Shader.PropertyToID("_IdColor");
        MaterialPropertyBlock properties = new MaterialPropertyBlock();
        properties.SetColor(colorPropertyId, color);
        renderer.SetPropertyBlock(properties);
    }

    public static Vector3Int[] DirVectors = {
        new Vector3Int(0,1,0),
        new Vector3Int(0,0,-1),
        new Vector3Int(-1,0,0),
        new Vector3Int(1,0,0),
        new Vector3Int(0,0,1),
        new Vector3Int(0,-1,0),
    };
}

public enum Dir
{
    Up = 0,
    Down = 5,
    Left = 2,
    Right = 3,
    Forward = 4,
    Back = 1
}
