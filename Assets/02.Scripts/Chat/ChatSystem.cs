using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Newtonsoft.Json;

public class ChatSystem : MonoBehaviour
{
    public int isChatting = 0;
    public int index;
    public float delayTime = 2.0f;

    public TextAsset _textScratch;

    public GameObject currentChatNPC;
    public List<List<string>> npcChat;

    public Image chatImage;
    public RectTransform contentRect;
    public Scrollbar scrollBar;
    NPCchat chatScript;

    public RectTransform screenRect;
    public ChatScreen chatScreenScript;
    int screenIndex = 0;


    [System.Serializable]
    public struct ChatList
    {
        public string chattingWith;
        public int newChatNum;
    }

    public List<ChatList> chatList = new List<ChatList>();

    //scratch.json 파일 불러오기
    [System.Serializable]
    public class Scratch
    {
        public string name;
        public string chatting_with;
        public List<List<string>> chat;
    }
    public Scratch myScratch = new Scratch();

    // Start is called before the first frame update
    void Start()
    {
        isChatting = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isChatting == 1)
        {
            Debug.Log(isChatting);
            _textScratch = Resources.Load<TextAsset>("scratch");
            myScratch = JsonConvert.DeserializeObject<Scratch>(_textScratch.text);
            Conversation();
            isChatting = 0;
        }
        if (npcChat != null && index > npcChat.Count - 1)
        {
            CancelInvoke("PrintChat");
            if (isChatting == 0)
                isChatting = 2;
            else
                isChatting = 1;
        }
            

        if(isChatting == 2)
        {
            Debug.Log(isChatting);
            _textScratch = Resources.Load<TextAsset>("scratch_2");
            myScratch = JsonConvert.DeserializeObject<Scratch>(_textScratch.text);
            Conversation();
            isChatting = 3;
        }
        if (npcChat != null && index > npcChat.Count - 1)
        {
            CancelInvoke("PrintChat");
            isChatting = 1;
        }
            
    }

    void Conversation()
    {
        index = 0;
        npcChat = myScratch.chat;

        ChatList newChat = new ChatList();

        //기존에 대화한 기록이 있을 때 이전 것에 이어서 생성
        if (chatList.FindIndex(item => item.chattingWith.Equals(myScratch.name + ", " + myScratch.chatting_with)) != -1)
        {
            //카톡창 미리보기 불러오기
            chatScript = contentRect.transform.GetChild(chatList.FindIndex(item => item.chattingWith.Equals(myScratch.name + ", " + myScratch.chatting_with))).GetComponent<NPCchat>();
            
            //카톡 대화창 불러오기
            chatScreenScript = GameObject.Find("ChatScreen").transform.GetChild(chatScript.screenIndex).gameObject.GetComponent<ChatScreen>();
        }
        else if (chatList.FindIndex(item => item.chattingWith.Equals(myScratch.chatting_with + ", " + myScratch.name)) != -1)
        {
            //카톡창 미리보기 불러오기
            chatScript = contentRect.transform.GetChild(chatList.FindIndex(item => item.chattingWith.Equals(myScratch.chatting_with + ", " + myScratch.name))).GetComponent<NPCchat>();

            //카톡 대화창 불러오기
            chatScreenScript = GameObject.Find("ChatScreen").transform.GetChild(chatScript.screenIndex).gameObject.GetComponent<ChatScreen>();
        }
        //기존에 대화 내역이 없을 때 새로운 대화창 생성
        else
        {
            //카톡창 미리보기 생성
            chatScript = Instantiate(chatImage).GetComponent<NPCchat>();
            chatScript.transform.SetParent(contentRect.transform, false);
            chatScript.screenIndex = screenIndex++;

            //카톡 대화창 생성
            chatScreenScript = Instantiate(chatScript.chatScreen).GetComponent<ChatScreen>();
            chatScreenScript.transform.SetParent(screenRect.transform, false);

            newChat.chattingWith = myScratch.name + ", " + myScratch.chatting_with;
            chatList.Add(newChat);
        }

        InvokeRepeating("PrintChat", 0, 3.0f);

    }
    void PrintChat()
    {
        //말풍선 생성
        currentChatNPC = GameObject.Find(npcChat[index][0]);
        currentChatNPC.GetComponentInChildren<Image>(true).gameObject.SetActive(true);
        currentChatNPC.GetComponentInChildren<TextMeshProUGUI>().text = npcChat[index][1];

        Invoke("SetActiveFalse", 2.0f);

        //카톡창 미리보기 업데이트
        chatScript.chattingWith.text = myScratch.name + ", " + myScratch.chatting_with;
        chatScript.chat.text = npcChat[index][1];

        ChatList newChatNum = chatList[chatScript.screenIndex];
        newChatNum.newChatNum += 1;
        chatList[chatScript.screenIndex] = newChatNum;
        chatScript.chatNum.text = chatList[chatScript.screenIndex].newChatNum.ToString();

        //카톡 대화 생성
        if (string.Equals(currentChatNPC.name, chatScript.chattingWith.text.Split(',')[0]))
        {
            ChatArea chatArea = Instantiate(chatScript.yellowArea).GetComponent<ChatArea>();
            chatArea.transform.SetParent(chatScreenScript.chatSreenContent.transform);
            chatArea.transform.localScale = new Vector3(1, 1, 1);
            chatArea.chattingRect.sizeDelta = new Vector2(400, chatArea.chattingRect.sizeDelta.y);
            chatArea.name.text = currentChatNPC.name;
            chatArea.text.text = npcChat[index][1];
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        }
        else
        {
            ChatArea chatArea = Instantiate(chatScript.whiteArea).GetComponent<ChatArea>();
            chatArea.transform.SetParent(chatScreenScript.chatSreenContent.transform);
            chatArea.transform.localScale = new Vector3(1, 1, 1);
            chatArea.chattingRect.sizeDelta = new Vector2(400, chatArea.chattingRect.sizeDelta.y);
            chatArea.name.text = currentChatNPC.name;
            chatArea.text.text = npcChat[index][1];
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        }
        index++;
    }

    void SetActiveFalse()
    {
        currentChatNPC.GetComponentInChildren<Image>(true).gameObject.SetActive(false);
    }

    public void ResetChatNum(int screenIndex)
    {
        ChatList resetChatNum = chatList[screenIndex];
        resetChatNum.newChatNum = 0;
        chatList[screenIndex] = resetChatNum;
    }

}
