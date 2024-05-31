using NPCServer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Guide : MonoBehaviour
{
    public GameObject panel;
 
    public GameObject guideChurch;
    
    public TextMeshProUGUI nameTxt;

    private bool isNPCChecked = false;
    private bool isNPCDoing = false;
    private bool isSpaceChecked = false;
    private bool isHint1Checked = false;
    private bool isRChecked = false;
    private bool isQChecked = false;


    private void Start()
    {
        nameTxt.text = Database.Instance.username;
        guideChurch.SetActive(true);
        
    }

    private void FixedUpdate()
    {
        if (Clock.Instance.GetCurrentTime().Hour == 12 && !isRChecked)
        {
            isRChecked = true;
            Rkey();
        }
        
        if (Clock.Instance.GetCurrentTime().Hour == 11 && !isQChecked)
        {
            isQChecked = true;
            Qkey();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            guideChurch.SetActive(false);
        }
    }

    public void NPCPanel()
    {
            panel.SetActive(true);

            isNPCDoing = true;
            string[] messeges = {"F키로 NPC와 대화할 수 있습니다.","Q키로 퀴즈를 풀 수 있습니다.","NPC의 직업, 취향, 가족 등 다양한 질문을 하세요!","신뢰도를 늘리면 말을 더 많이 할 수 있습니다.","NPC는 가끔 거짓말을 하기도 합니다."}; 
            int randomIndex = UnityEngine.Random.Range(0, messeges.Length);

            if (!isNPCChecked) randomIndex = 0;
            panel.GetComponentInChildren<TextMeshProUGUI>().text = messeges[randomIndex];
            panel.GetComponent<Panel>().Show_Panel();
            StartCoroutine(SetBoolFalseAfterDelay(3.0f));
            isNPCChecked = true;            
    }
    
    // 코루틴 함수
    private IEnumerator SetBoolFalseAfterDelay(float delay)
    {
        // delay 시간만큼 대기
        yield return new WaitForSecondsRealtime(delay);
        // 불변수를 false로 설정
        isNPCDoing = false;
       
    }
    public void SpacePanel()
    {
      
       // isSpaceChecked = true;
        panel.SetActive(true);
        panel.GetComponentInChildren<TextMeshProUGUI>().text = "스페이스키로 물건이나 쓰레기를 주울 수 있습니다.";
        panel.GetComponent<Panel>().Show_Panel();
       
    }
    
    public void Hint()
    {
      
        isHint1Checked = true;
        panel.SetActive(true);
        panel.GetComponentInChildren<TextMeshProUGUI>().text = "여기서 힌트가 될 물건을 찾아보자";
        panel.GetComponent<Panel>().Show_Panel();
       
    }
    
    public void Rkey()
    {
        
        panel.SetActive(true);
        panel.GetComponentInChildren<TextMeshProUGUI>().text = "R키를 누르면 대화내역을 볼 수 있습니다.";
        panel.GetComponent<Panel>().Show_Panel();
       
    }
    
    public void Qkey()
    {
        
        panel.SetActive(true);
        panel.GetComponentInChildren<TextMeshProUGUI>().text = "NPC 옆에서 Q키를 누르면 퀴즈를 풀 수 있습니다.";
        panel.GetComponent<Panel>().Show_Panel();
       
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
       
        if (collision.gameObject.tag == "NPC"&&!isNPCDoing )
        {
         
            NPCPanel();
        }
        
        if (collision.gameObject.tag == "SpaceObject" && !isSpaceChecked)
        {
            SpacePanel();
        }
        
          
        if (collision.gameObject.tag == "Hint1" && !isHint1Checked)
        {
            Hint();
        }
        
    }

 
}
