using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Clock : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public bool timerOn;
    public float time;
    public float min;
    public float hour;
    public string AmPm;

    public TextMeshProUGUI dateText;
    string[] days;
    public int day;
    public int date;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        timerOn = true;
        days = new string[] {"월", "화", "수", "목", "금", "토", "일"};
        day = 0;
        date = 1;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {
            time += Time.deltaTime; //second
            min = ((int)time * 60) / 60 % 60;    //minute
            hour = ((int)time * 60) / 3600;   //hour

            if(hour >= 24)
            {
                ChangeDate();
            }
            
            //오전 오후 구분
            if(hour < 12)
            {
                AmPm = "오전";
            }
            else
            {
                AmPm = "오후";
            }

            hour %= 12;
            if(hour == 0)   // 00시를 12시로 표시
            {
                hour = 12;
            }

            // 10분마다 시간 텍스트 변경
            timeText.text = hour.ToString() + " : " + ((int)(min / 10)).ToString() + "0" + " " + AmPm;
            dateText.text = days[day] + ". " + date.ToString();
        }
    }
    
    public void ChangeDate()
    {
        time = 0;
        day = (day + 1) % 7; 
        date++;
    }

    public void StopTimer()
    {
        timerOn = false;
    }

    public void ResumeTimer()
    {
        timerOn = true;
    }
}
