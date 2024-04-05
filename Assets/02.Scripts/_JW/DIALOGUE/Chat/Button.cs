using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject chatBuffer;
    public GameObject chatScreen;

    public void ShowChatBuffer()
    {
        chatBuffer.SetActive(true);
    }
    public void ExitChatBuffer()
    {
        chatBuffer.SetActive(false);
    }

    public void ShowChatScreen()
    {
        NPCchat npcChat = gameObject.GetComponent<NPCchat>();
        int index = npcChat.screenIndex;
        ChatSystem chatSystem = GameObject.Find("ChatSystem").GetComponent<ChatSystem>();
        chatSystem.ResetChatNum(index);
        npcChat.chatNum.text = chatSystem.chatList[index].newChatNum.ToString();
        chatScreen = GameObject.Find("ChatScreen").transform.GetChild(index).gameObject;
        chatScreen.SetActive(true);
        
    }
    public void ExitChatScreen()
    {
        Debug.Log(gameObject.GetComponentInParent<ChatScreen>());
        gameObject.GetComponentInParent<ChatScreen>().gameObject.SetActive(false);
    }
}
