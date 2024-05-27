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
    //private bool isQuizing = false;
    
    private List<string> nameList = new List<string>();
    private void Update()
    {
        if (isNPCInTrigger)
        {
            if (Input.GetKeyUp(KeyCode.F)&&!NPCQuizPanel.activeSelf)
            {
                chatPanel.SetActive(true);
                Time.timeScale = 0;
                isChatting = true;
                sendMessage.targetPersonaName = currentNPCName;
                npcQuizManager.NPCName = currentNPCName;
                if(!ContainsName(currentNPCName))
                    nameList.Add(currentNPCName);
            }
            
            if (Input.GetKeyUp(KeyCode.Q)&&!chatPanel.activeSelf)
            {
                if (currentNPCName != "나주교" && !GameObject.Find(currentNPCName).GetComponent<NPCQuiz>().quizEnd)
                {
                   // isQuizing = true;
                    Time.timeScale = 0;
                    npcQuizManager.NPCName = currentNPCName;
                    NPCQuizPanel.SetActive(true);
                }
            }
        }
        
        if (Input.GetKeyUp(KeyCode.R)&&!isChatting)
        {
          
            chatHistoryPanel.SetActive(true);
        }
        
        
    }

    private bool ContainsName(string name1)
    {   if (nameList == null)
        {
            return false;
        }
        
        foreach (string storedName in nameList)
        {
            if (string.Equals(storedName.Trim(), name1.Trim(), System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public void isTrueQuiz()
    {
        //isQuizing = true;
    }

    public void isFalseQuiz()
    {
        //isQuizing = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "NPC")
        {
        
            isNPCInTrigger = true;
            currentNPCName = collision.gameObject.name;
            sendMessage.targetPersonaName = currentNPCName;
            npcQuizManager.NPCName = currentNPCName;

            if (ContainsName(currentNPCName))
            {
                if (currentNPCName != "나주교" && !GameObject.Find(currentNPCName).GetComponent<NPCQuiz>().quizEnd)
                {
                    NPCQuizBtn.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "NPC")
        {
            isChatting = false;
            //isQuizing = false;
            NPCQuizBtn.SetActive(false);
            isNPCInTrigger = false;
            currentNPCName = null;
            
        }
    }
}
