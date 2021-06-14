using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Flowable))]
public class Tile : MonoBehaviour
{
    public int mergeID;
    [HideInInspector]
    public Tile[] neighbors;
    [HideInInspector]
    public Flowable flow;

    protected MeshRenderer renderer;

    void Awake()
    {
        flow = GetComponent<Flowable>();
        gameObject.name = gameObject.name + Random.Range(0, 1000).ToString();
    }

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

    protected virtual bool ShouldMerge(Tile t, int dirTowardsOther)
    {
        return t != null && t.mergeID == mergeID;
    }

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

    public static int Up = 0;
    public static int Down = 5;
    public static int Left = 2;
    public static int Right = 3;
    public static int Forward = 4;
    public static int Back = 1;
}