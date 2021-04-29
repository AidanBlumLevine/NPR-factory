using UnityEngine;

[ExecuteAlways]
public class Grid : MonoBehaviour
{
    public static Grid data;
    public Vector3Int gridSize = new Vector3Int(5, 5, 5);
    public Material gridMat;

    void Awake(){
        data = this;
    }

    void OnRenderObject()
    {
        GL.PushMatrix();
        gridMat.SetPass(0);
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(Color.black);
        for (int x = 0; x <= gridSize.x; x++)
        {
            for (int y = 0; y <= gridSize.y; y++)
            {

                GL.Vertex(new Vector3(x - gridSize.x / 2f, y, -gridSize.z / 2f));
                GL.Vertex(new Vector3(x - gridSize.x / 2f, y, gridSize.z / 2f));
            }
            for (int z = 0; z <= gridSize.z; z++)
            {

                GL.Vertex(new Vector3(x - gridSize.x / 2f, 0, z - gridSize.z / 2f));
                GL.Vertex(new Vector3(x - gridSize.x / 2f, gridSize.y, z - gridSize.z / 2f));
                for (int y = 0; y <= gridSize.y; y++)
                {

                    GL.Vertex(new Vector3(-gridSize.x / 2f, y, z - gridSize.z / 2f));
                    GL.Vertex(new Vector3(gridSize.x / 2f, y, z - gridSize.z / 2f));
                }
            }
        }
        GL.End();
        GL.PopMatrix();
    }
}
