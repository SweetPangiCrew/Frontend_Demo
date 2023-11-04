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
        int index = gameObject.GetComponent<NPCchat>().screenIndex;
        Debug.Log(index);
        ChatSystem chatSystem = GameObject.Find("ChatSystem").GetComponent<ChatSystem>();
        //chatSystem.chatScreen[index].GetComponent<GameObject>().SetActive(true);
        chatScreen = GameObject.Find("ChatScreen").transform.GetChild(index).gameObject;
        chatScreen.SetActive(true);
        
    }
    public void ExitChatScreen()
    {
        Debug.Log(gameObject.GetComponentInParent<ChatScreen>());
        gameObject.GetComponentInParent<ChatScreen>().gameObject.SetActive(false);
    }
}
