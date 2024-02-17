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
    private ChatManager _chatManager;
    public RectTransform ContentRect;
    public Scrollbar scrollBar;
    public TMP_InputField inputField;
    
    public void Transfer(){
        if(transferNum <= 7){
            // send message on chat
            Debug.Log(transferNum + 1 + "번째 메세지");
            GameObject TextClone = Instantiate(_chatManager.YellowArea, ContentRect);
            AreaScript Area = TextClone.GetComponent<AreaScript>();

            Area.TextRect.GetComponent<TextMeshProUGUI>().text = inputField.text;
            Area.NameText.text = "player";

            transferNum++;
        }else{
            // cannot send message
            Debug.Log("더 이상 메세지를 전송할 수 없습니다.");
        }
    }
}
