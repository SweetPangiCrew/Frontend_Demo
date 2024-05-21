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


    private void Start()
    {
        nameTxt.text = Database.Instance.username;
        guideChurch.SetActive(true);
        
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
    }

 
}
