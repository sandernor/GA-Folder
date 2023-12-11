using System.Collections;
using System.Collections.Generic;
//using TriangleNet;
using Unity.VisualScripting;
using UnityEngine;

public struct vertice
{
    public Vector3 position;
    //public Color color;
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

    int width = 100;
    int height = 100;

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
    float[] U;

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
                vertice vertex = new vertice();
                vertex.position = new Vector3(i, 250, j);

                points[k] = vertex;

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
        w = 2f / 8f;
        u = 5f * w;
        n = 4;

        A = new float[n];
        W = new float[n];
        U = new float[n];

        for (int i = 0; i < n; i++)
        {
            A[i] = Random.Range(a / 2, a) * (w / 100f);
            W[i] = w;
            //W[i] = A[i];
            U[i] = Random.Range(u / 2, u);

            //A[i] = a;
            //W[i] = w;
            //U[i] = u;
        }
    }

    public void RanColGPU()
    {
        Vector4 a4v;
        Vector4 w4v;
        Vector4 u4v;
        a4v = new Vector4(A[0], A[1], A[2], A[3]);
        w4v = new Vector4(W[0], W[1], W[2], W[3]);
        u4v = new Vector4(U[0], U[1], U[2], U[3]);

        int vector3Size = sizeof(float) * 3;

        int totalSize = vector3Size;

        ComputeBuffer pointsBuffer = new ComputeBuffer(points.Length, totalSize);
        pointsBuffer.SetData(points);

        waterShader.SetBuffer(0, "vertices", pointsBuffer);
        waterShader.SetVector("a", a4v);
        waterShader.SetVector("w", w4v);
        waterShader.SetVector("u", u4v);
        waterShader.SetFloat("t", t);
        waterShader.Dispatch(0, points.Length / 10, 1, 1);

        pointsBuffer.GetData(points);

        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = points[i].position;
        }

        pointsBuffer.Dispose();
    }

    void Start()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.uv = uvs;
        //mesh.normals = CalcVNormals();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        t += Time.deltaTime;

        RanColGPU();

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
