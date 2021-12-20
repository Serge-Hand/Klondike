using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    Text timeUI;

    float startTime;//time when user clicks on play
    float ellapsedTime;//the ellapsed time
    bool startCounter;//flag to start the counter

    int minutes;
    int seconds;

    // Start is called before the first frame update
    void Start()
    {
        startCounter = false;

        timeUI = GetComponent<Text>();
    }

    public void StartTimeCounter()
    {
        startTime = Time.time;
        startCounter = true;
    }

    public void StopTimeCounter()
    {
        startCounter = false;
    }

    public float get_time()
    {
        return ellapsedTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (startCounter)
        {
            ellapsedTime = Time.time - startTime;

            minutes = (int)ellapsedTime / 60;
            seconds = (int)ellapsedTime % 60;

            timeUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
