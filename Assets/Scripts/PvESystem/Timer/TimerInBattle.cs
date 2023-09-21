using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class TimerInBattle : MonoBehaviour
{
    public static TimerInBattle instance { get; private set; }

    [SerializeField] private TextMeshProUGUI timeTxt;
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool timerIsRunning;
    [SerializeField] private bool timeOut;

    public float timeRemaining_ => timeRemaining;
    public bool timerIsRunning_ => timerIsRunning;
    public bool timeOut_ => timeOut;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        timeTxt = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        timerIsRunning = false;
        timeOut = false;
    }

    void FixedUpdate()
    {
        CountdownTime();
    }

    public void SetDuration(float _duration)
    {
        timeRemaining = _duration;
    }

    public void TimerSwitch(bool _boolean)
    {
        timerIsRunning = _boolean;
    }

    void CountdownTime()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.fixedDeltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                //Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                timeOut = true;
            }
        }
        else
        {
            DisplayTime(timeRemaining);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int minutes = (int)timeToDisplay/60;
        int seconds = (int)timeToDisplay%60;
        if(minutes == 0 && seconds <= 10)
        {
            timeTxt.color = Color.red;
        }
        timeTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
