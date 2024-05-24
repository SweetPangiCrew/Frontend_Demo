using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace NPCServer
{
    public class Clock : MonoBehaviour
    {
        public RectTransform hand;
        public float rotate;

        public TextMeshProUGUI timeText;
        public bool timerOn;
        public float time;
        public float sec;
        public float min;
        public float spareMin;
        public float hour;
        public string AmPm;

        public TextMeshProUGUI dateText;
        string[] days;
        public int day;
        public int date;

        public DateTime curr_time;
        private bool change = false;

        public static Clock Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

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
                sec = ((int)time * 108) % 60; //second
                min = (((int)time * 108) / 60 + spareMin )% 60; //minute
                hour = (8 + (((int)time * 108) / 3600)) % 24; //hour (8am ~ 2am)
                curr_time =  new DateTime(DateTime.Now.Year, 8, date, (int)hour, (int)min, (int)sec); 

                //아날로그 시계 바늘
                rotate = (hour * 30 + (min * 0.5f)) % 360;
                hand.rotation = Quaternion.Euler(0, 0, -rotate);

                //자정에 날짜 바꾸기
                if (hour == 0)
                {
                    if (!change)
                    {
                        day = (day + 1) % 7;
                        date++;
                        change = true;
                    }
                }

                //새벽 2시에 시간 초기화
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
                if (hour == 0) // 00시를 12시로 표시
                {
                    hour = 12;
                }

                // 10분마다 시간 텍스트 변경
                timeText.text = hour.ToString() + " : " + ((int)(min / 10)).ToString() + "0" + " " + AmPm;
                dateText.text = "8월 " + date.ToString() + "일. " + days[day];
                //GetCurrentTime();
            }
        }

        public void ChangeDate()
        {
            time = 0;
            rotate = 0;
            change = false;
        }

        public void StopTimer()
        {
            timerOn = false;
        }

        public void ResumeTimer()
        {
            timerOn = true;
        }
        
        public DateTime AddTime(int minutes)
        {

            Debug.Log("퀴즈 전"+curr_time);
            // 20분 더하기 6 / 10분
            spareMin += minutes ; //1200 60 *120 
          
            return curr_time;
            // ex) 08/01/2024 17:20:14
            //Debug.Log(curr_time);
        }
        
        public DateTime GetCurrentTime()
        {
            return curr_time;
            // ex) 08/01/2024 17:20:14
            //Debug.Log(curr_time);
        }
    }

}