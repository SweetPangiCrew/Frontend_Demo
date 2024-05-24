using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChat : MonoBehaviour
{
    public GameObject chatPanel;
    public GameObject chatHistoryPanel;
    public GameObject NPCQuizPanel;
    
    public SendMessage sendMessage;
    public NPCQuizManager npcQuizManager;

    private bool isNPCInTrigger = false;
    private string currentNPCName;
    
    private void Update()
    {
        if (isNPCInTrigger)
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                chatPanel.SetActive(true);
                Time.timeScale = 0;

                sendMessage.targetPersonaName = currentNPCName;
                npcQuizManager.NPCName = currentNPCName;
            }
            
            if (Input.GetKeyUp(KeyCode.Q))
            {
            
                Time.timeScale = 0;
                npcQuizManager.NPCName = currentNPCName;
                NPCQuizPanel.SetActive(true);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.R))
        {
            chatHistoryPanel.SetActive(true);
        }
        
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "NPC")
        {
            isNPCInTrigger = true;
            currentNPCName = collision.gameObject.name;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "NPC")
        {
            isNPCInTrigger = false;
            currentNPCName = null;
        }
    }
}
