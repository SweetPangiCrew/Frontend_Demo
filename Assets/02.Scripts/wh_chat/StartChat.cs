using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChat : MonoBehaviour
{
    public GameObject chatPanel;
    public GameObject chatHistoryPanel;
    public GameObject NPCQuizPanel;
    public GameObject NPCQuizBtn;
    
    public SendMessage sendMessage;
    public NPCQuizManager npcQuizManager;

    private bool isNPCInTrigger = false;
    private string currentNPCName;
    private bool isChatting = false;
    
    private List<string> nameList = new List<string>();
    private void Update()
    {
        if (isNPCInTrigger)
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                chatPanel.SetActive(true);
                Time.timeScale = 0;
                isChatting = true;
                sendMessage.targetPersonaName = currentNPCName;
                npcQuizManager.NPCName = currentNPCName;
                nameList.Add(currentNPCName);
            }
            
            if (Input.GetKeyUp(KeyCode.Q)&&!isChatting)
            {
            
                Time.timeScale = 0;
                npcQuizManager.NPCName = currentNPCName;
                NPCQuizPanel.SetActive(true);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.R)&&!isChatting)
        {
          
            chatHistoryPanel.SetActive(true);
        }
        
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "NPC")
        {
        
            isNPCInTrigger = true;
            sendMessage.targetPersonaName = currentNPCName;
            npcQuizManager.NPCName = currentNPCName;
            
            if(nameList.Contains(currentNPCName))
                NPCQuizBtn.SetActive(true);
            currentNPCName = collision.gameObject.name;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "NPC")
        {
            isChatting = false;
            isNPCInTrigger = false;
            currentNPCName = null;
        }
    }
}
