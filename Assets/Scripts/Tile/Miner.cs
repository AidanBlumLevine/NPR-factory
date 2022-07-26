using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Machine
{
    public Item minedItem;
    float productionTimer;
    int nextSide = 1;
    public override void Set(Tile[] neighbors)
    {
        flow.connections[0].enabled = false;
        flow.connections[1].enabled = true;
        flow.connections[1].direction = Flowable.Out;
        flow.connections[1].enforced = true;
        flow.connections[2].enabled = true;
        flow.connections[2].direction = Flowable.Out;
        flow.connections[2].enforced = true;
        flow.connections[3].enabled = true;
        flow.connections[3].direction = Flowable.Out;
        flow.connections[3].enforced = true;
        flow.connections[4].enabled = true;
        flow.connections[4].direction = Flowable.Out;
        flow.connections[4].enforced = true;
        flow.connections[5].enabled = false;
        base.Set(neighbors);
        productionTimer = 0;
    }
    void Update()
    {
        productionTimer += Time.deltaTime;
        if (productionTimer > 3)
        {
            productionTimer = 0;
            
            if(Item.NItemsGlobal(transform.position,.1f) > 0)
                return;

            for (int i = 0; i < 6; i++)
            {
                int tryExportDir = (nextSide + i) % 6;
                FlowConnection fc = flow.connections[tryExportDir];
                if (fc.attachedTo != null)
                {
                    nextSide += i + 1;
                    Item newItem = Instantiate(minedItem.gameObject, transform.position, Quaternion.identity).GetComponent<Item>();
                    newItem.Set(Tile.DirVectors[5-tryExportDir], fc.entry);
                    break;
                }
            }
        }
    }
}
