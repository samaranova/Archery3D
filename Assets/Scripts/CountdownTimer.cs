using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float countdownTime;
    public float countdownTimeCopy;
    private Text countdownText;

    void Start()
    {
        // Get our text box that shows to the user how much time they have left
        countdownText = GameObject.Find("Text_PlayGame_TimeLeft").GetComponent<Text>();
        countdownTimeCopy = countdownTime;
    }

    void Update()
    {
        // while the countdown hasn't hit 0, decrease the time and display time left to user
        if (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            countdownText.text = "Time Left: " + System.Math.Round(countdownTime, 1).ToString();
        }
        else
        {
            // once time has hit 0, show the user he ran out of time
            countdownTime = 0;
            countdownText.text = "Time Left: 0.0";
        }
    }
}
