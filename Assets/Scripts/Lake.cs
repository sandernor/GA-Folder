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
    public GameObject player;
    private CamMovement script;
    private Mesh mesh;
    public GameObject house;
    public HouseLight light;

    //public RenderTexture waterTexture;

    //int threadsX = 8;
    //int threadsY = 8;
    //int threadsZ = 1;

    int width = 100;
    int height = 100;

    public float waterHeight = 120.6f;
    float zMod = 157f;

    Vector3[] vertices;
    Color[] colors;
    int[] triangles;
    private vertice[] points;
    Vector3[] allVerts;
    int[] allTris;

    // sinewave stuff
    private float a;    // waveheight
    private float w;    // 2 / wavelength   -   essentially wavelength
    private float t;    // time
    private float u;    // 2 / wavelength * speed   -   essentially speed
    private int n;      // number of waves to sum   -   making up 1 more realistic wave
    private float e = 0;

    float[] A;
    float[] W;
    float[] U;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        light = house.GetComponent("HouseLight") as HouseLight;

        points = new vertice[width * height];
        vertices = new Vector3[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];
        allVerts = new Vector3[vertices.Length * 4];
        allTris = new int[triangles.Length * 4];

        player = GameObject.Find("Player");
        script = player.GetComponent("CamMovement") as CamMovement;

        int k = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                vertice vertex = new vertice();
                vertex.position = new Vector3(i, waterHeight, j + zMod);

                points[k] = vertex;

                k++;


            }
        }

        // making indeces for triangles in mesh
        // making to triangles at a time, so a quad
        // therefore, 6 ints are calculated
        k = 0;
        for (int i = 0; i < triangles.Length / 6; i++)
        {
            // this is her to jump to the next line of vertices in the grid when getting to the edge
            if (i + k == width * (k + 1) - 1)
            {
                k += 1;
            }

            ////tri 1
            //triangles[i * 6] = i + k;
            //triangles[i * 6 + 1] = i + k + 1;
            //triangles[i * 6 + 2] = width + i + k;

            ////tri 2
            //triangles[i * 6 + 3] = i + k + 1;
            //triangles[i * 6 + 4] = width + i + 1 + k;
            //triangles[i * 6 + 5] = width + i + k;

            // this code is hard to read but very fast. i create 4 quadrants making the lake. here i make a quad for each quadrant each loop

            //quad 1
            //tri 1
            allTris[i * 6] = i + k;
            allTris[i * 6 + 1] = i + k + 1;
            allTris[i * 6 + 2] = width + i + k;

            //tri 2
            allTris[i * 6 + 3] = i + k + 1;
            allTris[i * 6 + 4] = width + i + 1 + k;
            allTris[i * 6 + 5] = width + i + k;

            //quad 2
            //tri 1
            allTris[i * 6 + triangles.Length] = i + k + vertices.Length;
            allTris[i * 6 + 1 + triangles.Length] = i + k + 1 + vertices.Length;
            allTris[i * 6 + 2 + triangles.Length] = width + i + k + vertices.Length;

            //tri 2
            allTris[i * 6 + 3 + triangles.Length] = i + k + 1 + vertices.Length;
            allTris[i * 6 + 4 + triangles.Length] = width + i + 1 + k + vertices.Length;
            allTris[i * 6 + 5 + triangles.Length] = width + i + k + vertices.Length;

            //quad 3
            //tri 1
            allTris[i * 6 + triangles.Length * 2] = i + k + (vertices.Length * 2);
            allTris[i * 6 + 1 + triangles.Length * 2] = i + k + 1 + (vertices.Length * 2);
            allTris[i * 6 + 2 + triangles.Length * 2] = width + i + k + (vertices.Length * 2);

            //tri 2
            allTris[i * 6 + 3 + triangles.Length * 2] = i + k + 1 + (vertices.Length * 2);
            allTris[i * 6 + 4 + triangles.Length * 2] = width + i + 1 + k + (vertices.Length * 2);
            allTris[i * 6 + 5 + triangles.Length * 2] = width + i + k + (vertices.Length * 2);

            //quad 4
            //tri 1
            allTris[i * 6 + triangles.Length * 3] = i + k + (vertices.Length * 3);
            allTris[i * 6 + 1 + triangles.Length * 3] = i + k + 1 + (vertices.Length * 3);
            allTris[i * 6 + 2 + triangles.Length * 3] = width + i + k + (vertices.Length * 3);

            //tri 2
            allTris[i * 6 + 3 + triangles.Length * 3] = i + k + 1 + (vertices.Length * 3);
            allTris[i * 6 + 4 + triangles.Length * 3] = width + i + 1 + k + (vertices.Length * 3);
            allTris[i * 6 + 5 + triangles.Length * 3] = width + i + k + (vertices.Length * 3);

            ////tri 1
            //allTris[i * 6 + triangles.Length * 4] = i + k + (vertices.Length * 4);
            //allTris[i * 6 + 1 + triangles.Length * 4] = i + k + 1 + (vertices.Length * 4);
            //allTris[i * 6 + 2 + triangles.Length * 4] = width + i + k + (vertices.Length * 4);

            ////tri 2
            //allTris[i * 6 + 3 + triangles.Length * 4] = i + k + 1 + (vertices.Length * 4);
            //allTris[i * 6 + 4 + triangles.Length * 4] = width + i + 1 + k + (vertices.Length * 4);
            //allTris[i * 6 + 5 + triangles.Length * 4] = width + i + k + (vertices.Length * 4);

            ////tri 1
            //allTris[i * 6 + triangles.Length * 5] = i + k + (vertices.Length * 5);
            //allTris[i * 6 + 1 + triangles.Length * 5] = i + k + 1 + (vertices.Length * 5);
            //allTris[i * 6 + 2 + triangles.Length * 5] = width + i + k + (vertices.Length * 5);

            ////tri 2
            //allTris[i * 6 + 3 + triangles.Length * 5] = i + k + 1 + (vertices.Length * 5);
            //allTris[i * 6 + 4 + triangles.Length * 5] = width + i + 1 + k + (vertices.Length * 5);
            //allTris[i * 6 + 5 + triangles.Length * 5] = width + i + k + (vertices.Length * 5);

            ////tri 1
            //allTris[i * 6 + triangles.Length * 6] = i + k + (vertices.Length * 6);
            //allTris[i * 6 + 1 + triangles.Length * 6] = i + k + 1 + (vertices.Length * 6);
            //allTris[i * 6 + 2 + triangles.Length * 6] = width + i + k + (vertices.Length * 6);

            ////tri 2
            //allTris[i * 6 + 3 + triangles.Length * 6] = i + k + 1 + (vertices.Length * 6);
            //allTris[i * 6 + 4 + triangles.Length * 6] = width + i + 1 + k + (vertices.Length * 6);
            //allTris[i * 6 + 5 + triangles.Length * 6] = width + i + k + (vertices.Length * 6);

            ////tri 1
            //allTris[i * 6 + triangles.Length * 7] = i + k + (vertices.Length * 7);
            //allTris[i * 6 + 1 + triangles.Length * 7] = i + k + 1 + (vertices.Length * 7);
            //allTris[i * 6 + 2 + triangles.Length * 7] = width + i + k + (vertices.Length * 7);

            ////tri 2
            //allTris[i * 6 + 3 + triangles.Length * 7] = i + k + 1 + (vertices.Length * 7);
            //allTris[i * 6 + 4 + triangles.Length * 7] = width + i + 1 + k + (vertices.Length * 7);
            //allTris[i * 6 + 5 + triangles.Length * 7] = width + i + k + (vertices.Length * 7);

            ////tri 1
            //allTris[i * 6 + triangles.Length * 8] = i + k + (vertices.Length * 8);
            //allTris[i * 6 + 1 + triangles.Length * 8] = i + k + 1 + (vertices.Length * 8);
            //allTris[i * 6 + 2 + triangles.Length * 8] = width + i + k + (vertices.Length * 8);

            ////tri 2
            //allTris[i * 6 + 3 + triangles.Length * 8] = i + k + 1 + (vertices.Length * 8);
            //allTris[i * 6 + 4 + triangles.Length * 8] = width + i + 1 + k + (vertices.Length * 8);
            //allTris[i * 6 + 5 + triangles.Length * 8] = width + i + k + (vertices.Length * 8);

            ////tri 1
            //allTris[i * 6 + triangles.Length * 9] = i + k + (vertices.Length * 9);
            //allTris[i * 6 + 1 + triangles.Length * 9] = i + k + 1 + (vertices.Length * 9);
            //allTris[i * 6 + 2 + triangles.Length * 9] = width + i + k + (vertices.Length * 9);

            ////tri 2
            //allTris[i * 6 + 3 + triangles.Length * 9] = i + k + 1 + (vertices.Length * 9);
            //allTris[i * 6 + 4 + triangles.Length * 9] = width + i + 1 + k + (vertices.Length * 9);
            //allTris[i * 6 + 5 + triangles.Length * 9] = width + i + k + (vertices.Length * 9);

            ////tri 1
            //allTris[i * 6 + triangles.Length * 10] = i + k + (vertices.Length * 10);
            //allTris[i * 6 + 1 + triangles.Length * 10] = i + k + 1 + (vertices.Length * 10);
            //allTris[i * 6 + 2 + triangles.Length * 10] = width + i + k + (vertices.Length * 10);

            ////tri 2
            //allTris[i * 6 + 3 + triangles.Length * 10] = i + k + 1 + (vertices.Length * 10);
            //allTris[i * 6 + 4 + triangles.Length * 10] = width + i + 1 + k + (vertices.Length * 10);
            //allTris[i * 6 + 5 + triangles.Length * 10] = width + i + k + (vertices.Length * 10);
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
        waterShader.SetFloat("e", e);
        waterShader.Dispatch(0, points.Length / 10, 1, 1);

        pointsBuffer.GetData(points);

        for (int i = 0; i < points.Length; i++)
        {
            allVerts[i] = points[i].position;
            allVerts[i + vertices.Length] = points[i].position + new Vector3(width - 1, 0, 0);
            allVerts[i + (vertices.Length * 2)] = points[i].position + new Vector3((width - 1) * 2, 0, 0);
            allVerts[i + (vertices.Length * 3)] = points[i].position + new Vector3((width - 1) * 3, 0, 0);
            //allVerts[i + (vertices.Length * 4)] = points[i].position + new Vector3((width - 1) * 1, 0, width - 1);
            //allVerts[i + (vertices.Length * 5)] = points[i].position + new Vector3((width - 1) * 2, 0, width - 1);
            //allVerts[i + (vertices.Length * 6)] = points[i].position + new Vector3((width - 1) * 3, 0, 0);
            //allVerts[i + (vertices.Length * 7)] = points[i].position + new Vector3((width - 1) * 3, 0, width - 1);
            //allVerts[i + (vertices.Length * 8)] = points[i].position + new Vector3((width - 1), 0, (width - 1) * 2);
            //allVerts[i + (vertices.Length * 9)] = points[i].position + new Vector3((width - 1) * 2, 0, (width - 1) * 2);
            //allVerts[i + (vertices.Length * 10)] = points[i].position + new Vector3((width - 1) * 3, 0, (width - 1) * 2);
        }

        for (int i = width * height - width; i < points.Length; i++)
        {
            allVerts[i].y = points[i - (width * height - width)].position.y;
            allVerts[i + vertices.Length].y = points[i - (width * height - width)].position.y;
            allVerts[i + (vertices.Length * 2)].y = points[i - (width * height - width)].position.y;
            allVerts[i + (vertices.Length * 3)].y = points[i - (width * height - width)].position.y;
        }

        int o = width - 1;
        for (int i = 0; i < points.Length; i += width)
        {
            //allVerts[i + width - 1].y = waterHeight;
            //allVerts[i].y = waterHeight;
            //allVerts[i + width - 1 + vertices.Length].y = waterHeight;
            //allVerts[i + vertices.Length].y = waterHeight;
            //allVerts[i + width - 1 + (vertices.Length * 2)].y = waterHeight;
            //allVerts[i + (vertices.Length * 2)].y = waterHeight;
            //allVerts[i + width - 1 + (vertices.Length * 3)].y = waterHeight;
            //allVerts[i + (vertices.Length * 3)].y = waterHeight;

            allVerts[i].y = points[o].position.y;
            allVerts[i + vertices.Length].y = points[o].position.y;
            allVerts[i + (vertices.Length * 2)].y = points[o].position.y;
            allVerts[i + (vertices.Length * 3)].y = points[o].position.y;

            o += width;
        }

        allVerts[width * height - width].y = allVerts[0].y;
        allVerts[width * height - width + vertices.Length].y = allVerts[0].y;
        allVerts[width * height - width + (vertices.Length * 2)].y = allVerts[0].y;
        allVerts[width * height - width + (vertices.Length * 3)].y = allVerts[0].y;

        pointsBuffer.Dispose();
    }

    public void WaterHeight(float h, bool r)
    {
        if (r && allVerts[0].y < 123f)
            e = h * 0.1f;
        else if (!r && allVerts[0].y > 108.5f)
            e = h * 0.1f;
        else
            e = 0;
    }

    void Start()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.uv = uvs;
        //mesh.normals = CalcVNormals();

        a = script.a;
        w = script.w;
        u = script.u;
        n = script.n;

        A = new float[n];
        W = new float[n];
        U = new float[n];

        A = script.A;
        W = script.W;
        U = script.U;

        //for (int i = 0; i < n; i++)
        //{
        //    A[i] = Random.Range(a / 2, a) * (w / 100f);
        //    W[i] = w;
        //    //W[i] = A[i];
        //    U[i] = Random.Range(u / 2, u);

        //    //A[i] = a;
        //    //W[i] = w;
        //    //U[i] = u;
        //}


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        t = script.time;

        RanColGPU();

        //int p = 0;

        //Vector3[] allVerts;
        //int[] allTris;
        //allVerts = new Vector3[vertices.Length * 3];
        //allTris = new int[triangles.Length * 3];

        //for (int i = 0; i < allVerts.Length; i++)
        //{
        //    if (i != 0 && i % vertices.Length == 0)
        //    {
        //        p += 1;
        //    }
        //    //Debug.Log(p);
        //    allVerts[i] = vertices[i - (vertices.Length * p)];

        //    allVerts[i] = allVerts[i] + new Vector3(p * height, 0, 0);
        //}

        //p = 0;

        //for (int i = 0; i < allTris.Length; i++)
        //{
        //    if (i != 0 && i % triangles.Length == 0)
        //    {
        //        p += 1;
        //    }
        //    Debug.Log(p);
        //    allTris[i] = triangles[i - (triangles.Length * p)] + triangles.Length * p;
        //}

        mesh.Clear();
        mesh.vertices = allVerts;
        mesh.triangles = allTris;
        mesh.RecalculateNormals();

        light.i1 = (allVerts[0].y - 108.5f) * 0.25f;
    }
}
