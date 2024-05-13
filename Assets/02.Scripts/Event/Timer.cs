using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI time;
    private bool startTimer;

    // Start is called before the first frame update
    void Start()
    {
        startTimer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(NPCServer.Clock.Instance.GetCurrentTime().Hour == 8 && NPCServer.Clock.Instance.GetCurrentTime().Minute == 30)
        {
            startTimer=true;
        }

        if (startTimer)
        {
            DateTime endingTime = new DateTime(DateTime.Now.Year, 8, 1, 10, 0, 0);
            TimeSpan timeSpan = endingTime - NPCServer.Clock.Instance.GetCurrentTime();
            time.text = timeSpan.Hours.ToString() + " :" + timeSpan.Minutes.ToString() + " :" + timeSpan.Seconds.ToString();
            if(timeSpan.Minutes < 0)
            {
                startTimer = false;
            }
        }
    }
}
