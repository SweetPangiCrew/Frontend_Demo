using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChat : MonoBehaviour
{
    public GameObject chatPanel;
    public SendMessage sendMessage;
    public NPCQuizManager npcQuizManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "NPC")
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                chatPanel.SetActive(true);
                Time.timeScale = 0;

                sendMessage.targetPersonaName = collision.gameObject.name;
                npcQuizManager.NPCName = collision.gameObject.name;
            }
        }
    }
}
