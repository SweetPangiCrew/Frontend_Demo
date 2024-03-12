using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SendMessage : MonoBehaviour
{
    // chatting transfer limit
    const int maxTransferNum = 7;
    private int transferNum = 0; // Number of transfers

    // chatting UI
    public ChatManager _chatManager;
    public RectTransform ContentRect;
    public Scrollbar scrollBar;
    public TMP_InputField inputField;

    public string targetPersonaName = "이자식";

    private bool TestMode = false;
    public void Transfer(){
        if(transferNum <= 7){
            
            if(inputField.text == "") return;

            // player message send
            PCMessage();
            StartCoroutine(showNPCMessageCoroutine());
           
            transferNum++;
         
        } else{
            
            //종료창 띄워야함.
            inputField.interactable = true;
            // cannot send message
            Debug.Log("더 이상 메세지를 전송할 수 없습니다.");
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

            //테스트용 코드 : 다른 상대와 대화 초기화하고 대화 시작
            if (TestMode && transferNum == 0) transferNum = -1;
            
            StartCoroutine(UserChatAPIManager.Instance.SendMessageCoroutine(Database.Instance.gameName,targetPersonaName,inputField.text,transferNum));
            
            if (TestMode && transferNum == 0) transferNum = 1;
    }

    private void NPCMessage(){
        Debug.Log(transferNum + 1 + "번째 메세지 수신");
        GameObject TextClone = Instantiate(_chatManager.OrangeArea, ContentRect);

        AreaScript Area = TextClone.GetComponent<AreaScript>();
        
        // NPC message -> text
        Area.TextRect.GetComponent<TextMeshProUGUI>().text = UserChatAPIManager.Instance.ResponseMessageInfo["response"];

        // NPC message -> name
        Area.NameText.text = targetPersonaName;
    }
    
    public IEnumerator showNPCMessageCoroutine()
    {
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
                break;
             

            } 
        }
        yield return 0;
    }
    
}
    