using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public TrackNode tracking;
    public Vector3 dir;
    public void Set(Vector3 exportDir, TrackNode tracking)
    {
        dir = exportDir;
        this.tracking = tracking;
        gameObject.tag = "Item";
    }

    void Update()
    {
        if (tracking.deleted)
        {
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, 1);
            foreach (Collider nearby in nearbyObjects)
            {
                Flowable flowable;
                float closest = .1f;
                TrackNode newNode = null;
                // if ((flowable = nearby.GetComponent<Flowable>()) != null)
                // {
                //     foreach (TrackNode tn in flowable.trackGraph)
                //     {
                //         float sqrDist = (transform.position - tn.pos).sqrMagnitude;
                //         if (sqrDist < closest && NItems(tn.pos, transform.lossyScale.x / 2) == 0)
                //         {
                //             closest = sqrDist;
                //             newNode = tn;
                //         }
                //     }
                // }
                if (newNode != null)
                {
                    transform.position = newNode.pos;
                    tracking = newNode;
                }
                else
                    Destroy(gameObject);
            }
        }

        Vector3 oldPos = transform.position + Vector3.zero;
        tracking = tracking.Track(this, Time.deltaTime);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.lossyScale.x / 2);
        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject != gameObject && hit.gameObject.tag == "Item")
            {
                float towardsOther = Vector3.Dot(dir, hit.transform.position - transform.position);
                if (towardsOther > 0.001)
                {
                    transform.position = oldPos;
                    float otherTowardsThis = Vector3.Dot(hit.GetComponent<Item>().dir, transform.position - hit.transform.position);
                    if (otherTowardsThis > 0.001)
                    {
                        tracking = tracking.Flip(this);
                    }
                }
            }
        }
        Debug.DrawRay(transform.position + Vector3.up, dir);
    }

    public float NItems(Vector3 pos, float radius = .4f)
    {
        float count = 0;
        Collider[] hitColliders = Physics.OverlapSphere(pos, radius);
        foreach (Collider hit in hitColliders)
            if (hit.gameObject != gameObject && hit.gameObject.tag == "Item")
                count += radius + .2f - (hit.transform.position - pos).magnitude;
        return count;
    }

    public static float NItemsGlobal(Vector3 pos, float radius = .4f)
    {
        float count = 0;
        Collider[] hitColliders = Physics.OverlapSphere(pos, radius);
        foreach (Collider hit in hitColliders)
            if (hit.gameObject.tag == "Item")
                count += radius + .2f - (hit.transform.position - pos).magnitude;
        return count;
    }
}