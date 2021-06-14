using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : Tile
{
    public Connector connectorPrefab;
    Connector[] connections = new Connector[6];

    public override void Set(Tile[] neighbors)
    {
        foreach (FlowConnection c in flow.connections)
            c.enforced = true;
        base.Set(neighbors);
        SetConnections();
    }

    protected override void AddNeighbor(Tile tile, int index)
    {
        base.AddNeighbor(tile, index);
        SetConnections();
    }

    protected override void RemoveNeighbor(int index)
    {
        base.RemoveNeighbor(index);
        SetConnections();
    }

    public override void TileUpdate()
    {
        SetConnections();
    }

    public override void Delete()
    {
        for (int i = 0; i < 6; i++)
        {
            if (connections[i] != null)
            {
                connections[i].Delete();
                //neighbors[i].flow.UnenforceFlow(5 - i);
            }
        }
        base.Delete();
    }

    void SetConnections()
    {
        for (int i = 0; i < 6; i++)
        {
            if (neighbors[i] != null && flow.CanConnect(neighbors[i].flow, i))
            {
                if (connections[i] == null)
                {
                    Connector conn = Instantiate(connectorPrefab.gameObject, transform.position, connectorPrefab.transform.rotation).GetComponent<Connector>();
                    conn.SetDirection(i);
                    connections[i] = conn;
                    flow.Connect(neighbors[i].flow, i);
                }
            }
            else if (connections[i] != null)
            {
                connections[i].Delete();
                    flow.Disconnect(i);
            }
        }
    }


}
