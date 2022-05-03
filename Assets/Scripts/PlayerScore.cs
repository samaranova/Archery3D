using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    // If we won a game, we will reset this value to however long it took
    public float winningTime = -1;

    private int targetsLeft = 10;
    private float bestTime = 0;
    private Text targetsLeftText;
    private Text bestTimeText;
    private Text winningScreenText;

    private GameObject winningScreen;

    void Start()
    {
        // Initially set our game over screen to inactive and get a reference to our winning screen text box
        winningScreen = GameObject.Find("Panel_PlayGame_WinningScreen");
        winningScreenText = GameObject.Find("Text_PlayGame_WinningScreen_Info").GetComponent<Text>();
        winningScreen.SetActive(false);

        targetsLeftText = GameObject.Find("Text_PlayGame_TargetsLeft").GetComponent<Text>();
        bestTimeText = GameObject.Find("Text_PlayGame_BestTime").GetComponent<Text>();

        // Display how many targets we have left and what our best time is so far
        targetsLeftText.text = "Targets Left: " + targetsLeft;
        bestTimeText.text = "Best Time: " + System.Math.Round(bestTime, 1).ToString();
    }

    // Whenever a target is hit, display to the user the number of targets left
    public void DecreaseTargetCount()
    {
        targetsLeft--;
        targetsLeftText.text = "Targets Left: " + targetsLeft.ToString();

        // Check how many targets we have left
        CheckNumTargets();
    }

    public float GetBestTime()
    {
        return bestTime;
    }

    // If we don't know whether a new time is a best time, use this method
    public void SetBestTime()
    {
        // If the new winning time is lower than the users best time and best time is not 0 (indicating first game)
        if (winningTime < bestTime || bestTime == 0)
        {
            // Set the winning time as the new best time and display new best time at top of screen
            bestTime = winningTime;
            bestTimeText.text = "Best Time: " + System.Math.Round(bestTime, 1).ToString();
            
            // Set the winning screen text box to tell the user they have a new best time
            winningScreenText.text = "New Best Time: " + System.Math.Round(bestTime, 1).ToString() + " Seconds";
        }
        else
        {
            // If they didn't have a new best time, just tell the user his winning time
            winningScreenText.text = "Your Winning Time: " + System.Math.Round(bestTime, 1).ToString() + " Seconds";
        }
    }

    // If we already know its a best time, call this method
    public void SetBestTime(float time)
    {
        bestTime = time;
    }

    // Check if we have 0 targets left
    public void CheckNumTargets()
    {
        if (targetsLeft == 0)
        {
            // Set our winning screen panel to active
            winningScreen.SetActive(true);

            // Get the user's winning time
            var countdownTimer = GameObject.Find("CountdownTimer").GetComponent(typeof(CountdownTimer)) as CountdownTimer;
            winningTime = countdownTimer.countdownTimeCopy - countdownTimer.countdownTime;

            // Check whether it is a best time, if so set the new best time
            SetBestTime();

            // Destroy the player script and countdown timer script to indicate game over
            var player = GameObject.Find("Player").GetComponent(typeof(PlayerMovement)) as PlayerMovement;
            player.DestroyPlayer();
        }
    }
}
