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
    private bool isSpaceChecked = false;
    private bool isHint1Checked = false;
    private bool isRChecked = false;


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
      
            //isNPCChecked = true;
            panel.SetActive(true);
            panel.GetComponentInChildren<TextMeshProUGUI>().text = "F키로 NPC와 대화할 수 있습니다.";
            panel.GetComponent<Panel>().Show_Panel();
            
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
    
    private void OnTriggerStay2D(Collider2D collision)
    {
       
        if (collision.gameObject.tag == "NPC" && !isNPCChecked)
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
