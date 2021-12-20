using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinTime : MonoBehaviour
{
    public GameObject oldTimer;
    //public GameObject newTimer;
    public Text currentTime;
    public Text highTime;

    // Start is called before the first frame update
    void Start()
    {
        //currentTime = GetComponent<Text>();
        //PlayerPrefs.SetFloat("BestTime", 1000f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Win()
    {
        float newTime;

        oldTimer.GetComponent<TimeCounter>().StopTimeCounter();
        newTime = oldTimer.GetComponent<TimeCounter>().get_time();

        printTime(currentTime, newTime);

        if (newTime < PlayerPrefs.GetFloat("BestTime"))
        {
            PlayerPrefs.SetFloat("BestTime", newTime);
        }

        printTime(highTime, PlayerPrefs.GetFloat("BestTime"));
    }

    void printTime(Text timer, float time)
    {
        int minutes, seconds;

        minutes = (int)time / 60;
        seconds = (int)time % 60;
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
