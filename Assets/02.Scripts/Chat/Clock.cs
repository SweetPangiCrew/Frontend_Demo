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
        days = new string[] {"��", "ȭ", "��", "��", "��", "��", "��"};
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
