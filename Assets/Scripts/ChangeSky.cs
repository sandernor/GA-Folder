using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeSky : MonoBehaviour
{
    public Material sun;
    public Material clouds;
    public ParticleSystem rain;
    public Light lighting;
    public Slider slider;
    public Slider slider2;
    public TMP_Text mmNumber;
    public TMP_Text msNumber;
    public WindZone wZone;

    public GameObject lake;
    public Lake lakeScript;
    public GameObject lake2;
    public Lake lakeScript2;

    public float sunny;
    public float dark;
    public float mm = 0f;
    public float ms = 0f;
    public float multi = 500f;

    public bool raining = false;

    void Start()
    {
        rain.enableEmission = false;
        lakeScript = lake.GetComponent("Lake") as Lake;
        lakeScript2 = lake2.GetComponent("Lake") as Lake;
    }

    public void Sun()
    {
        RenderSettings.skybox = sun;
        rain.enableEmission = false;
        lighting.intensity = sunny;
        raining = false;
    }

    public void Rain()
    {
        RenderSettings.skybox = clouds;
        rain.enableEmission = true;
        lighting.intensity = dark;
        raining = true;
    }

    public void mmChange()
    {
        mm = slider.value;
        mmNumber.text = mm.ToString();
        rain.emissionRate = mm * multi;
    }

    public void windChange()
    {
        ms = slider2.value;
        msNumber.text = ms.ToString();
        wZone.windMain = ms;
        //rain.forceOverLifetime.x = -ms;
        Physics.gravity = new Vector3 (-ms , -9.81f, 0);
        rain.startRotation3D = new Vector3 (0, 0, (-4.5f * ms) / 57f);
    }

    public void FixedUpdate()
    {
        if (raining)
        {
            lakeScript.WaterHeight(mm * 0.01f);
            lakeScript2.WaterHeight(mm * 0.01f);
        }
        else
        {
            lakeScript.WaterHeight(5 * (-0.01f));
            lakeScript2.WaterHeight(5 * (-0.01f));
        }
            
    }
}
