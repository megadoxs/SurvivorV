using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro; // using text mesh for the clock display

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement; // used to access the volume component

public class DayNightScript : MonoBehaviour
{
    private Light2D light2d;
    private static readonly float tick = 60;
    private float seconds;
    private int mins;
    private int hours = 12;
    public static int days = 1;
    public static bool isNight = false;

    void Start()
    {
        light2d = gameObject.GetComponent<Light2D>();
    }

    // Update is called once per frame
    void FixedUpdate() // we used fixed update, since update is frame dependant. 
    {
        CalcTime();
    }

    private void CalcTime() // Used to calculate sec, min and hours
    {
        seconds += Time.fixedDeltaTime * tick; // multiply time between fixed update by tick

        if (seconds >= 60) // 60 sec = 1 min
        {
            seconds = 0;
            mins += 1;
        }

        if (mins >= 60) //60 min = 1 hr
        {
            mins = 0;
            hours += 1;
        }

        if (hours >= 24) //24 hr = 1 day
        {
            hours = 0;
            days += 1;
        }
        ControlPPV(); // changes post processing volume after calculation
    }

    private void ControlPPV() // used to adjust the post processing slider.
    {
        if(SceneManager.GetActiveScene().buildIndex != 2) return;
        
        //ppv.weight = 0;
        if (hours >= 21 && hours < 22) // dusk at 21:00 / 9pm    -   until 22:00 / 10pm
        {
            light2d.intensity = Mathf.Max(0.1f, 1f - (mins * 60 + seconds) / 3600); // since dusk is 1 hr, we just divide the mins by 60 which will slowly increase from 0 - 1 
        }

        if (hours >= 6 && hours < 7) // Dawn at 6:00 / 6am    -   until 7:00 / 7am
        {
            light2d.intensity = Mathf.Max(0.1f, (mins * 60 + seconds) / 3600); // we minus 1 because we want it to go from 1 - 0
        }
    }
}
