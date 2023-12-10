using System.Collections;
using System.Collections.Generic;
using TriangleNet;
using Unity.VisualScripting;
using UnityEngine;

public class Lake : MonoBehaviour
{
    public ComputeShader waterShader;
    public RenderTexture waterTexture;

    int threadsX = 8;
    int threadsY = 8;
    int threadsZ = 1;

    int width = 10;
    int height = 10;

    Vector3[] vertices;

    // Start is called before the first frame update
    private void Awake()
    {
        vertices = new Vector3[width * height];
        int k = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                vertices[k] = new Vector3(i, 100, j);
                k++;
            }
        }
    }
    void Start()
    {
        waterTexture = new RenderTexture(256, 256, 24);
        waterTexture.enableRandomWrite = true;
        waterTexture.Create();

        waterShader.SetTexture(0, "Result", waterTexture);
        waterShader.SetFloat("Resolution", waterTexture.width);
        waterShader.Dispatch(0, waterTexture.width / threadsX, waterTexture.height / threadsY, threadsZ);

        Graphics.Blit(waterTexture, waterTexture);

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triVerts;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
