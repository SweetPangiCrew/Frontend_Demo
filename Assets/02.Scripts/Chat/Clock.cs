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
        days = new string[] {"��", "ȭ", "��", "��", "��", "��", "��"};
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
            curr_time = "February " + date.ToString() + ", 2023, " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString();

            //�Ƴ��α� �ð� �ٴ�
            rotate = (hour * 30 + (min * 0.5f)) % 360; 
            hand.rotation = Quaternion.Euler(0, 0, -rotate);

            //���� 2�ÿ� ��¥ �Ѿ��
            if(hour == 2)
            {
                ChangeDate();
            }
            
            //���� ���� ����
            if(hour < 12)
            {
                AmPm = "����";
            }
            else
            {
                AmPm = "����";
            }

            hour %= 12;
            if(hour == 0)   // 00�ø� 12�÷� ǥ��
            {
                hour = 12;
            }

            // 10�и��� �ð� �ؽ�Ʈ ����
            timeText.text = hour.ToString() + " : " + ((int)(min / 10)).ToString() + "0" + " " + AmPm;
            dateText.text = days[day] + ". " + date.ToString();
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

    void GetCurrentTime()
    {
        // ex) feburary 13, 2023, 17:20:14
        Debug.Log(curr_time);
    }
}
