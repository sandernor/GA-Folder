using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    //Movement
    float x;
    float y;
    float z;
    public float speed = 5;
    public float time = 0;

    //Camera
    float camX;
    float camY;
    [SerializeField] float sens = 500;
    public GameObject cam;

    public bool look;

    // sinewave stuff
    public float a;    // waveheight
    public float w;    // 2 / wavelength   -   essentially wavelength
    public float t;    // time
    public float u;    // 2 / wavelength * speed   -   essentially speed
    public int n;      // number of waves to sum   -   making up 1 more realistic wave

    public float[] A;
    public float[] W;
    public float[] U;

    private void Awake()
    {
        a = 2;
        w = 2f / 10f;
        u = 20f * w;
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

    void Start()
    {
        look = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;

        x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        y = Input.GetAxis("Jump") * Time.deltaTime * speed;

        camX = Input.GetAxis("Mouse X") * Time.deltaTime * sens;
        camY = Input.GetAxis("Mouse Y") * Time.deltaTime * sens;

        if (look == true)
        {
            transform.Rotate(0, camX, 0);
            cam.transform.Rotate(-camY, 0, 0);
            transform.Translate(x, y, z);
        }
    }
}