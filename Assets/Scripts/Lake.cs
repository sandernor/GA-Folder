using System.Collections;
using System.Collections.Generic;
//using TriangleNet;
using Unity.VisualScripting;
using UnityEngine;

public struct vertice
{
    public Vector3 position;
    public Color color;
}

public class Lake : MonoBehaviour
{
    public ComputeShader waterShader;
    public GameObject daCube;
    private Mesh mesh;
    //public RenderTexture waterTexture;

    //int threadsX = 8;
    //int threadsY = 8;
    //int threadsZ = 1;

    int width = 10;
    int height = 10;

    Vector3[] vertices;
    Color[] colors;
    int[] triangles;
    private vertice[] points;

    // sinewave stuff
    private float a;    // waveheight
    private float w;    // 2 / wavelength   -   essentially wavelength
    private float t;    // time
    private float u;    // 2 / wavelength * speed   -   essentially speed
    private int n;      // number of waves to sum   -   making up 1 more realistic wave

    float[] A;
    float[] W;

    public void RanColGPU()
    {
        //int colorSize = sizeof(float) * 4;
        //int vector3Size = sizeof(float) * 3;
        //int totalSize = colorSize + vector3Size;

        //ComputeBuffer cubesBuffer = new ComputeBuffer(cubes.Length, totalSize);
        //cubesBuffer.SetData(cubes);

        //waterShader.SetBuffer(0, "cubes", cubesBuffer);
        //waterShader.SetFloat("resolution", cubes.Length);
        //waterShader.Dispatch(0, cubes.Length / 10, 1, 1);

        //cubesBuffer.GetData(cubes);

        //for (int i = 0; i < cubes.Length; i++)
        //{
        //    GameObject instCube = Instantiate(daCube, cubes[i].position, Quaternion.identity);
        //    instCube.GetComponent<MeshRenderer>().material.SetColor("_Color", cubes[i].color);
        //}

        //cubesBuffer.Dispose();

        int colorSize = sizeof(float) * 4;
        int vector3Size = sizeof(float) * 3;
        int totalSize = colorSize + vector3Size;

        ComputeBuffer cubesBuffer = new ComputeBuffer(points.Length, totalSize);
        cubesBuffer.SetData(points);

        waterShader.SetBuffer(0, "vertices", cubesBuffer);
        waterShader.SetFloat("resolution", points.Length);
        waterShader.Dispatch(0, points.Length / 10, 1, 1);

        cubesBuffer.GetData(points);

        for (int i = 0; i < points.Length; i++)
        {
            //GameObject instCube = Instantiate(daCube, points[i].position, Quaternion.identity);
            //instCube.GetComponent<MeshRenderer>().material.SetColor("_Color", points[i].color);

            vertices[i] = points[i].position;
            colors[i] = points[i].color;
        }

        cubesBuffer.Dispose();
    }

    public float SumSinesTest(Vector3 pos)
    {
        float x = 0;

        for (int i = 0; i < n; i++)
        {
            x += A[i] * Mathf.Sin(1 * W[i] * (t + pos.x/* + pos.z + 1*/) * u);
        }

        return x * 1f;
    }
    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        points = new vertice[width * height];
        vertices = new Vector3[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];
        colors = new Color[width * height];

        int k = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //vertice vertex = new vertice();
                //vertex.position = new Vector3(i, 250, j);
                //vertex.color = Color.yellow;

                //points[k] = vertex;

                vertices[k] = new Vector3(i, 250, j);

                k++;


            }
        }

        // making indeces for triangles in mesh
        // making to triangles at a time, so a quad
        // therefore, 6 ints are calculated
        k = 0;
        for (int i = 0; i < width * (width - 2) + 1; i++)
        {
            // this is her to jump to the next line of vertices in the grid when getting to the edge
            if (i + k == width * (k + 1) - 1)
            {
                k += 1;
            }

            //tri 1
            triangles[i * 6] = i + k;
            triangles[i * 6 + 1] = i + k + 1;
            triangles[i * 6 + 2] = width + i + k;

            //tri 2
            triangles[i * 6 + 3] = i + k + 1;
            triangles[i * 6 + 4] = width + i + 1 + k;
            triangles[i * 6 + 5] = width + i + k;
        }

        a = 2;
        w = 4 / 2;
        u = 0.2f * w;
        n = 4;

        A = new float[n];
        W = new float[n];

        for (int i = 0; i < n; i++)
        {
            A[i] = Random.Range(1, a);
            W[i] = A[i];
        }

    }
    void Start()
    {
        //waterTexture = new RenderTexture(256, 256, 24);
        //waterTexture.enableRandomWrite = true;
        //waterTexture.Create();

        //waterShader.SetTexture(0, "Result", waterTexture);
        //waterShader.SetFloat("Resolution", waterTexture.width);
        //waterShader.Dispatch(0, waterTexture.width / threadsX, waterTexture.height / threadsY, threadsZ);

        //Graphics.Blit(waterTexture, waterTexture);

        //RanColGPU();

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.colors = colors;
        //mesh.uv = uvs;
        //mesh.colors
        //mesh.normals = CalcVNormals();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        t += Time.deltaTime;

        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = /*vertices[i].y*/ + SumSinesTest(vertices[i]);
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
