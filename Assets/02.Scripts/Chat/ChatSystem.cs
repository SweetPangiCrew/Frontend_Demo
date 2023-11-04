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
    public struct ChatBuffer
    {
        public Image chatImage;
        public Scratch myScratch;
    }

    public List<ChatBuffer> chatBuffer = new List<ChatBuffer>();
    public int chatCount=0;

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
            myScratch = JsonConvert.DeserializeObject<Scratch>(_textScratch.text);
            Conversation();
            isChatting = 0;
        }
        if (npcChat != null && index > npcChat.Count - 1)
        {
            CancelInvoke("PrintChat");
            //CancelInvoke("UpdateChatList");
            isChatting = 2;
        }
            

        if(isChatting == 2)
        {
            //GameObject.Find("JSONReader").GetComponent<JSONReader>()._textScratch = Resources.Load<TextAsset>("scratch(1)");
            _textScratch = Resources.Load<TextAsset>("scratch_2");
            myScratch = JsonConvert.DeserializeObject<Scratch>(_textScratch.text);
            Conversation();
            isChatting = 0;
        }
        if (npcChat != null && index > npcChat.Count - 1)
        {
            CancelInvoke("PrintChat");
            //CancelInvoke("UpdateChatList");
        }
            
    }

    void Conversation()
    {
        index = 0;
        npcChat = myScratch.chat;

        //카톡창 미리보기 생성
        ChatBuffer chatList = new ChatBuffer();
        chatScript = Instantiate(chatImage).GetComponent<NPCchat>();
        chatScript.transform.SetParent(contentRect.transform, false);
        chatScript.screenIndex = screenIndex++;

        //카톡 대화창 생성
        chatScreenScript = Instantiate(chatScript.chatScreen).GetComponent<ChatScreen>();
        chatScreenScript.transform.SetParent(screenRect.transform, false);

        InvokeRepeating("PrintChat", 0, 3.0f);
        //PrintChatList();
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
        chatScript.chatNum.text = (index + 1).ToString();

        //카톡 대화 생성
        Debug.Log("current:"+currentChatNPC.name);
        Debug.Log(chatScript.chattingWith.text.Split(',')[0]);
        //Debug.Log(string.Equals(currentChatNPC.name, chatScript.chattingWith.text.Split(',')[0].ToString()));
        if (string.Equals(currentChatNPC.name, chatScript.chattingWith.text.Split(',')[0]))
        {
            ChatArea chatArea = Instantiate(chatScript.yellowArea).GetComponent<ChatArea>();
            chatArea.transform.SetParent(chatScreenScript.chatSreenContent.transform);
            chatArea.chattingRect.sizeDelta = new Vector2(250, chatArea.chattingRect.sizeDelta.y);
            chatArea.text.text = npcChat[index][1];
            LayoutRebuilder.ForceRebuildLayoutImmediate(chatScript.yellowArea.GetComponent<RectTransform>());
        }
        else
        {
            ChatArea chatArea = Instantiate(chatScript.whiteArea).GetComponent<ChatArea>();
            chatArea.transform.SetParent(chatScreenScript.chatSreenContent.transform);
            chatArea.text.text = npcChat[index][1];
        }
        index++;
    }

    //말풍선 생성
/*    void PrintChat()
    {
        currentChatNPC = GameObject.Find(npcChat[index][0]);
        currentChatNPC.GetComponentInChildren<Image>(true).gameObject.SetActive(true);
        currentChatNPC.GetComponentInChildren<TextMeshProUGUI>().text = npcChat[index][1];

        Invoke("SetActiveFalse", delayTime);
        //index++;
    }*/
    void SetActiveFalse()
    {
        currentChatNPC.GetComponentInChildren<Image>(true).gameObject.SetActive(false);
    }

    //카톡창 미리보기 생성
/*    void PrintChatList()
    {
        ChatBuffer chatList = new ChatBuffer();
        chatScript = Instantiate(chatImage).GetComponent<NPCchat>();
        chatScript.transform.SetParent(contentRect.transform, false);
        InvokeRepeating("UpdateChatList", delayTime, delayTime);

    }
    void UpdateChatList()
    {
        Debug.Log(index);
        chatScript.chattingWith.text = myScratch.name + " ," + myScratch.chatting_with;
        chatScript.chat.text = npcChat[index][1];
        chatScript.chatNum.text = (index + 1).ToString();
        index++;
    }*/

    void ChatListUI()
    {
        ChatBuffer chatList = new ChatBuffer();
        Image newChatImage = chatImage;

        if (chatBuffer.Count != 0)
        {
            newChatImage = Instantiate(chatImage);
            newChatImage.rectTransform.position = new Vector2(chatBuffer[-1].chatImage.rectTransform.position.x, chatBuffer[-1].chatImage.rectTransform.position.y - 130.0f);
        }
        newChatImage.transform.Find("ChattingWith").gameObject.GetComponent<TextMeshProUGUI>().text = npcChat[0][0] + ", " + npcChat[1][0];
        newChatImage.transform.Find("Chat").gameObject.GetComponent<TextMeshProUGUI>().text = npcChat[index][1];
        newChatImage.transform.Find("ChatNum").GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = (index + 1).ToString();

        chatList.chatImage = newChatImage;
        chatList.myScratch = myScratch;
        chatBuffer.Add(chatList);
    }

}
