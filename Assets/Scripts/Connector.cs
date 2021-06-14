using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Flowable))]
public class Connector : MonoBehaviour
{
    public int visualMergeID = 1;
    public Mesh mesh;
    void Start()
    {
        Random.InitState(visualMergeID);
        Color32 color = Random.ColorHSV();
        int colorPropertyId = Shader.PropertyToID("_IdColor");
        MaterialPropertyBlock properties = new MaterialPropertyBlock();
        properties.SetColor(colorPropertyId, color);
        GetComponent<MeshRenderer>().SetPropertyBlock(properties);
    }
    public void SetDirection(int i)
    {
        CombineInstance[] combine = new CombineInstance[1];
        combine[0].mesh = mesh;
        combine[0].transform = Matrix4x4.Rotate(Quaternion.Euler(-90, i == 4 ? 90 : i == 1 ? 270 : i == 2 ? 0 : 180, 0));
        GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
