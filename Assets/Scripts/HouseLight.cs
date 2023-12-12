using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseLight : MonoBehaviour
{
    public Light lighting; 
    public float i1 = 0;
    public float i2 = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lighting.intensity = i1 + i2;
    }
}
