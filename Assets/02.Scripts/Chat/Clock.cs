using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Clock : MonoBehaviour
{
    public RectTransform hand;
    public float rotate;

    public TextMeshProUGUI timeText;
    public bool timerOn;
    public float time;
    public float sec;
    public float min;
    public float hour;
    public string AmPm;

    public TextMeshProUGUI dateText;
    string[] days;
    public int day;
    public int date;

    public string curr_time;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        timerOn = true;
        rotate = 0;
        days = new string[] { "월", "화", "수", "목", "금", "토", "일" };
        day = 0;
        date = 1;

    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {
            time += Time.deltaTime;
            sec = ((int)time * 108) % 60;   //second
            min = ((int)time * 108) / 60 % 60;    //minute
            hour = (8 + (((int)time * 108) / 3600)) % 24;   //hour (8am ~ 2am)
            curr_time = "August " + date.ToString() + ", 2023, " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString();

            //아날로그 시계 바늘
            rotate = (hour * 30 + (min * 0.5f)) % 360;
            hand.rotation = Quaternion.Euler(0, 0, -rotate);

            //새벽 2시에 날짜 넘어가기
            if (hour == 2)
            {
                ChangeDate();
            }

            //오전 오후 구분
            if (hour < 12)
            {
                AmPm = "오전";
            }
            else
            {
                AmPm = "오후";
            }

            hour %= 12;
            if (hour == 0)   // 00시를 12시로 표시
            {
                hour = 12;
            }

            // 10분마다 시간 텍스트 변경
            timeText.text = hour.ToString() + " : " + ((int)(min / 10)).ToString() + "0" + " " + AmPm;
            dateText.text = "8월 " + date.ToString() + "일. " + days[day];
            GetCurrentTime();
        }
    }

    public void ChangeDate()
    {
        time = 0;
        rotate = 0;
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

    public void GetCurrentTime()
    {
        // ex) feburary 13, 2023, 17:20:14
        Debug.Log(curr_time);
    }
}
