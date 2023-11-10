using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Handler : MonoBehaviour
{
    bool menu = false;
    public Canvas WeatherUI;
    private float time = 5f;
    public GameObject Player;

    void Start()
    {
        WeatherUI.enabled = false;
    }


    void Update()
    {
        time += Time.deltaTime;
        if (menu == false && time > 0.5f)
        {
            if (Input.GetAxis("Fire3") > 0.5)
            {
                menu = true;
                menuOn();
                
            }
        }
        else if (menu == true && time > 0.5f)
        {
            if (Input.GetAxis("Fire3") > 0.5)
            {
                menu = false;
                menuOff();
            }
        }
    }

    public void menuOn()
    {
        WeatherUI.enabled = true;
        time = 0f;
        Player.GetComponent<CamMovement>().look = false;
    }
    public void menuOff()
    {
        WeatherUI.enabled = false;
        time = 0f;
        Player.GetComponent<CamMovement>().look = true;
    }
}
