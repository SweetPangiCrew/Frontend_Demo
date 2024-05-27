using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SendMessage : MonoBehaviour
{
    // chatting transfer limit
    public int maxTransferNum = 5;
    public int transferNum = 0; // Number of transfers

    private int max_length = 100;
    
    // chatting UI
    public ChatManager _chatManager;
    public RectTransform ContentRect;
    public Scrollbar scrollBar;
    public TMP_InputField inputField;
    private UnityEngine.UI.Button sendBtn;
    public string targetPersonaName = "이자식";
    public TextMeshProUGUI remain;
    public TextMeshProUGUI NPCname;

    // npc quiz
    public GameObject NPCQuizPanel;
    
    private bool TestMode = false;
    
    private void Awake()
    {
        sendBtn = GetComponent<UnityEngine.UI.Button>();
        // 입력 필드의 onValueChanged 이벤트에 메서드를 추가합니다.
        inputField.onValueChanged.AddListener(OnTextChanged);

    }

  
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // InputField에 포커스가 있는 경우에만 실행
            if (inputField.isFocused)
            { 
                Transfer();
            }
        }
    }
    
    private void OnDisable()
    {
        inputField.text = "";
        inputField.interactable = true;
        sendBtn.interactable = true;
        transferNum = 0;
        Time.timeScale = 1;
        
        // 자식 GameObject를 모두 파괴합니다.
        for (int i = 0; i < ContentRect.transform.childCount; i++)
        {
            Destroy(ContentRect.transform.GetChild(i).gameObject);
        }
 
    }

    void OnEnable()
    {
        //대화시 시간 멈춤!
        Time.timeScale = 0f;
        NPCname.text = targetPersonaName;
        
        float reliability = PlayerAction.getCurrentReliability(); // 유저 신뢰도 연결
        
        if (reliability <= 5)
        {
            max_length = 10;
            maxTransferNum = 4;
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "전송(" + (maxTransferNum - transferNum) + "/" + maxTransferNum + ")";
        }
        else if (reliability <= 10)
        {
            max_length = 20;
            maxTransferNum = 5;
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "전송(" + (maxTransferNum - transferNum) + "/" + maxTransferNum + ")";
        }
        else if (reliability <= 20)
        {
            max_length = 30;
            maxTransferNum = 5;
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "전송(" + (maxTransferNum - transferNum) + "/" + maxTransferNum + ")";
        }
        else
        {
            maxTransferNum = 5; //이부분 이상한데?
            max_length = 100;
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "전송(" + (maxTransferNum - transferNum) + "/" + maxTransferNum + ")";
        }

        remain.text = "(" + inputField.text.Length + "/" + max_length + ")";
    }

    private void OnTextChanged(string text)
    {
        remain.text = "(" + inputField.text.Length + "/" + max_length + ")";
        // 텍스트의 길이가 100자를 초과하는 경우
        if (inputField.text.Length > max_length)
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
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "전송(" + (maxTransferNum - transferNum) + "/" + maxTransferNum + ")";

            //inputField.interactable = true;
            inputField.ActivateInputField(); // 포커스를 다시 InputField로 이동
        }
    }
    
    private void PCMessage(){
           // send message on chat
            Debug.Log(transferNum + 1 + "번째 메세지 발신");
            GameObject TextClone = Instantiate(_chatManager.textArea[0], ContentRect);
            AreaScript Area = TextClone.GetComponent<AreaScript>();


            Area.TextRect.GetComponent<TextMeshProUGUI>().text = inputField.text;
            Area.NameText.text = Database.Instance.username;
          
            //테스트용 코드 : 다른 상대와 대화 초기화하고 대화 시작
            //if (TestMode && transferNum == 0) transferNum = -1;
            
            StartCoroutine(UserChatAPIManager.Instance.SendMessageCoroutine(Database.Instance.gameName,targetPersonaName,inputField.text,transferNum));
            
            
            //if (TestMode && transferNum == 0) transferNum = 1;
              
            inputField.interactable = false;
            inputField.onValueChanged.RemoveListener(OnTextChanged);
            inputField.text = "답변을 기다리는 중입니다...";
            inputField.onValueChanged.AddListener(OnTextChanged);
            sendBtn.interactable = false;
            scrollBar.value = 0;
    }

    private void NPCMessage(){
        Debug.Log(transferNum + 1 + "번째 메세지 수신");
        GameObject TextClone = Instantiate(_chatManager.textArea[1], ContentRect);
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
        yield return new WaitForSecondsRealtime(0.1f);
        scrollBar.value = 0;
        while (true)
        {
            if(!UserChatAPIManager.Instance.isResponded)
            {

                //메세지 수신 기다리는 중
                yield return new WaitForSecondsRealtime(0.5f);
            }
            else
            {
                // NPc Message get
                NPCMessage();
                //inputField.onValueChanged.AddListener(OnTextChanged);
                inputField.interactable = true;
                sendBtn.interactable = true;
              
                if (transferNum == maxTransferNum)
                {
            
                    inputField.interactable = false;
                    sendBtn.interactable = false;
                    inputField.onValueChanged.RemoveListener(OnTextChanged);
                    inputField.text = "";
                    inputField.text = "더 이상 메세지를 전송할 수 없습니다.";
                    inputField.onValueChanged.AddListener(OnTextChanged);

                    if (transferNum == maxTransferNum)
                    {
                        if (targetPersonaName != "나주교" && !GameObject.Find(targetPersonaName).GetComponent<NPCQuiz>().quizEnd)
                        {
                            StartCoroutine("StartNPCQuiz");
                        }
                    }
                }
                break;
            } 
        }
        yield return new WaitForSecondsRealtime(0.2f);
        
        scrollBar.value = 0;
    }

    IEnumerator StartNPCQuiz()
    {
        yield return new WaitForSecondsRealtime(3f);
        
        Debug.Log("대화 종료. 퀴즈 시작");
        // 대화 종료 후 퀴즈 시작
        NPCQuizPanel.SetActive(true);
    }
    
}
    