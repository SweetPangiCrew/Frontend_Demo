using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SendMessage : MonoBehaviour
{
    // chatting transfer limit
    const int maxTransferNum = 4; // TODO : 바꿔야함
    private int transferNum = 0; // Number of transfers

    // chatting UI
    public ChatManager _chatManager;
    public RectTransform ContentRect;
    public Scrollbar scrollBar;
    public TMP_InputField inputField;
    private UnityEngine.UI.Button sendBtn;
    public string targetPersonaName = "이자식";
    
    

    private bool TestMode = false;

    private void Awake()
    {
        sendBtn = GetComponent<UnityEngine.UI.Button>();
    }

    public void Transfer(){
        if(transferNum < maxTransferNum){
            
            if(inputField.text == "") return;

            // player message send
            PCMessage();
            StartCoroutine(showNPCMessageCoroutine());
           
            transferNum++;
            
        } 
    }
    
    private void PCMessage(){
           // send message on chat
            Debug.Log(transferNum + 1 + "번째 메세지 발신");
            GameObject TextClone = Instantiate(_chatManager.YellowArea, ContentRect);
            AreaScript Area = TextClone.GetComponent<AreaScript>();
            
            Area.TextRect.GetComponent<TextMeshProUGUI>().text = inputField.text;
            Area.NameText.text = "player";
            
            inputField.interactable = false;
            inputField.text = "답변을 기다리는 중입니다...";
            sendBtn.interactable = false;
            scrollBar.value = 0;
            //테스트용 코드 : 다른 상대와 대화 초기화하고 대화 시작
            if (TestMode && transferNum == 0) transferNum = -1;
            
            StartCoroutine(UserChatAPIManager.Instance.SendMessageCoroutine(Database.Instance.gameName,targetPersonaName,inputField.text,transferNum));
            
            if (TestMode && transferNum == 0) transferNum = 1;
    }

    private void NPCMessage(){
        Debug.Log(transferNum + 1 + "번째 메세지 수신");
        GameObject TextClone = Instantiate(_chatManager.OrangeArea, ContentRect);
        AreaScript Area = TextClone.GetComponent<AreaScript>();
        inputField.text = "";
        // NPC message -> text
        Area.TextRect.GetComponent<TextMeshProUGUI>().text = UserChatAPIManager.Instance.ResponseMessageInfo["response"];

        // NPC message -> name
        Area.NameText.text = targetPersonaName;
        
        //scrollview
        scrollBar.value = 0;
    }
    
    public IEnumerator showNPCMessageCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        scrollBar.value = 0;
        while (true)
        {
            if(!UserChatAPIManager.Instance.isResponded)
            {

                //메세지 수신 기다리는 중
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                // NPc Message get
                NPCMessage();
                inputField.interactable = true;
                sendBtn.interactable = true;
              
                if (transferNum == maxTransferNum)
                {
                    inputField.text = "더 이상 메세지를 전송할 수 없습니다.";
                    inputField.interactable = false;
                    sendBtn.interactable = false;
                }
                break;
            } 
        }
        yield return new WaitForSeconds(0.2f);
        
        scrollBar.value = 0;
    }
    
}
    