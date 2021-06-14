using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }
    [HideInInspector]
    public GameObject selectedTile;
    public AnimationCurve bulgeCurve;
    Vector3Int gridSize;
    Material gridMat;

    Tile[,,] tiles;
    List<Tile>[,,] registeredUpdates;
    Vector4 bulge;
    float bulgeTime;
    public Color normalGridColor, edgeGridColor;
    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        selectedTile = null;
        gridSize = Grid.data.gridSize;
        gridMat = Grid.data.gridMat;
        tiles = new Tile[gridSize.x, gridSize.y, gridSize.z];
        registeredUpdates = new List<Tile>[gridSize.x, gridSize.y, gridSize.z];
        bulge = new Vector4(0, -1000, 0, 0);
    }

    void Update()
    {
        gridMat.SetVector("FocusedPos", new Vector3(0, -100, 0));
        gridMat.SetVector("SelectedPos", new Vector3(0, -100, 0));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            Vector3Int index = PointToIndex(hit.point + hit.normal * .5f);

            Tile hitTile;
            if ((hitTile = hit.collider.gameObject.GetComponent<Tile>()) != null)
            {
                gridMat.SetVector("SelectedPos", hitTile.gameObject.transform.position);
                if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.Space))
                {
                    bulgeTime = 0;
                    bulge = new Vector4(hitTile.transform.position.x, hitTile.transform.position.y, hitTile.transform.position.z, 0);
                    hitTile.Delete();
                    TriggerRegisteredUpdates(PointToIndex(hitTile.transform.position));
                }

                //calculate real index
                Vector3 dir = hit.point - hitTile.transform.position + hit.normal * .5f;
                float ax = Mathf.Abs(dir.x);
                float ay = Mathf.Abs(dir.y);
                float az = Mathf.Abs(dir.z);
                if (ax > ay && ax > az)
                    dir = new Vector3(Mathf.Sign(dir.x), 0, 0);
                if (ay > ax && ay > az)
                    dir = new Vector3(0, Mathf.Sign(dir.y), 0);
                if (az > ay && az > ax)
                    dir = new Vector3(0, 0, Mathf.Sign(dir.z));
                index = PointToIndex(hitTile.transform.position + dir);
            }

            Vector3 tile = IndexToWorld(index);
            if (Inbounds(index) && tiles[index.x, index.y, index.z] != null)
            {
                if (hitTile == null)
                    gridMat.SetVector("SelectedPos", tile);
            }
            else
            {
                gridMat.SetVector("FocusedPos", tile);
                gridMat.SetColor("NearbyColor", Inbounds(index) ? normalGridColor : edgeGridColor);
            }

            if (selectedTile != null && Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.Space) && Inbounds(index) && tiles[index.x, index.y, index.z] == null)
            {
                bulgeTime = 0;
                bulge = new Vector4(tile.x, tile.y, tile.z, 0);
                Tile t = Instantiate(selectedTile, tile, selectedTile.transform.rotation).GetComponent<Tile>();
                t.Set(GetNeighbors(index));
                tiles[index.x, index.y, index.z] = t;
                TriggerRegisteredUpdates(index);
            }
        }

        bulge.w = bulgeCurve.Evaluate(bulgeTime);
        Shader.SetGlobalVector("_Bulge", bulge);
        bulgeTime += Time.deltaTime;
    }

    Vector3Int PointToIndex(Vector3 raw)
    {
        return new Vector3Int(Mathf.FloorToInt(raw.x + gridSize.x / 2f), Mathf.FloorToInt(raw.y), Mathf.FloorToInt(raw.z + gridSize.z / 2f));
    }

    Vector3 IndexToWorld(Vector3Int index)
    {
        return new Vector3(index.x - gridSize.x / 2f + .5f, index.y + .5f, index.z - gridSize.z / 2f + .5f);
    }

    bool Inbounds(Vector3Int i)
    {
        return i.x >= 0 && i.x < gridSize.x && i.y >= 0 && i.y < gridSize.y && i.z >= 0 && i.z < gridSize.z;
    }

    public Tile[] GetNeighbors(Vector3Int p)
    {
        Tile[] neighbors = new Tile[6];
        for (int i = 0; i < 6; i++)
        {
            Vector3Int index = p - Tile.DirVectors[i];
            if (Inbounds(index))
                neighbors[i] = tiles[index.x, index.y, index.z];
        }
        return neighbors;
    }

    public void RegisterUpdate(Tile t, Vector3 pos)
    {
        Vector3Int i = PointToIndex(pos);
        if (!Inbounds(i))
            return;

        if (registeredUpdates[i.x, i.y, i.z] == null)
            registeredUpdates[i.x, i.y, i.z] = new List<Tile>();

        registeredUpdates[i.x, i.y, i.z].Add(t);
        registeredUpdates[i.x, i.y, i.z].RemoveAll(o => o == null);
    }

    public void RegisterCornerUpdate(Tile t, Vector3 pos)
    {
        Vector3Int[] corners = {
            new Vector3Int(1,0,1),
            new Vector3Int(-1,0,1),
            new Vector3Int(1,0,-1),
            new Vector3Int(-1,0,-1),
        };

        foreach (Vector3Int v in corners)
        {
            RegisterUpdate(t, v + pos);
        }
    }

    public void TriggerRegisteredUpdates(Vector3Int index)
    {
        if (registeredUpdates[index.x, index.y, index.z] != null)
        {
            foreach (Tile toUpdate in registeredUpdates[index.x, index.y, index.z])
            {
                if (toUpdate != null)
                    toUpdate.TileUpdate();
            }
        }
    }

    // int[] GetTileIndex(Tile t)
    // {
    //     for (int x = 0; x < tiles.GetLength(0); x++)
    //     {
    //         for (int y = 0; y < tiles.GetLength(1); y++)
    //         {
    //             for (int z = 0; z < tiles.GetLength(2); z++)
    //             {
    //                 if (tiles[x, y, z] == t)
    //                 {
    //                     return new int[] { x, y, z };
    //                 }
    //             }
    //         }
    //     }
    //     return null;
    // }
}
