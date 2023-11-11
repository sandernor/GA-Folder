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

    public float sunny;
    public float dark;
    public float mm = 0f;
    public float ms = 0f;
    public float multi = 500f;

    void Start()
    {
        rain.enableEmission = false;
    }

    public void Sun()
    {
        RenderSettings.skybox = sun;
        rain.enableEmission = false;
        lighting.intensity = sunny;
    }

    public void Rain()
    {
        RenderSettings.skybox = clouds;
        rain.enableEmission = true;
        lighting.intensity = dark;
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
}
