using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SendMessage : MonoBehaviour
{
    // chatting transfer limit
    private int maxTransferNum = 5;
    private int transferNum = 0; // Number of transfers

    private int max_length = 100;
    // chatting UI
    public ChatManager _chatManager;
    public RectTransform ContentRect;
    public Scrollbar scrollBar;
    public TMP_InputField inputField;

    public string targetPersonaName = "이자식";

    private bool TestMode = false;
    
    
    void Start()
    {
        //대화시 시간 멈춤!
        Time.timeScale = 0;
        
        // 입력 필드의 onValueChanged 이벤트에 메서드를 추가합니다.
        inputField.onValueChanged.AddListener(OnTextChanged);
        
        int reliability = 30; // 유저 신뢰도 연결
        
        if (reliability <= 5)
        {
            max_length = 10;
            maxTransferNum = 5;
        }
        else if (reliability <= 10)
        {
            max_length = 30;
            maxTransferNum = 7;
           
        }
        else if (reliability <= 20)
        {
            max_length = 50;
            maxTransferNum = 10;
        }
        else
        {
            maxTransferNum = 10;
            max_length = 100;
        }
    }

    private void OnTextChanged(string text)
    {
        
        // 텍스트의 길이가 100자를 초과하는 경우
        if (text.Length > max_length)
        {
            // 텍스트를 100자로 제한합니다.
            inputField.text = text.Substring(0, max_length);
        }
    }

    //종료할 때 시간 재시작 에디터 내에서 연결 해주기
    public void clickCancleBtn()
    {
        Time.timeScale = 1; 
    }
    public void Transfer(){
        
        if(transferNum <= maxTransferNum){
            
            if(inputField.text == "") return;

            // player message send
            PCMessage();
            StartCoroutine(showNPCMessageCoroutine());
           
            transferNum++;
         
        } else{
            
            Time.timeScale = 1; 
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
    