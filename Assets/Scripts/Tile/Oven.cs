using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : Machine
{
    public override void Set(Tile[] neighbors)
    {
        flow.connections[0].enabled = false;
        flow.connections[1].enabled = false;

        flow.connections[2].enabled = true;
        flow.connections[2].direction = Flowable.In;
        flow.connections[2].enforced = true;
        
        flow.connections[3].enabled = true;
        flow.connections[3].direction = Flowable.In;
        flow.connections[3].enforced = true;

        flow.connections[4].enabled = false;
        flow.connections[5].enabled = false;

        base.Set(neighbors);
        flow.trackGraph[0].onRecieve = (item) =>
        {
            Destroy(item.gameObject);
            return true;
        };
    }
}
