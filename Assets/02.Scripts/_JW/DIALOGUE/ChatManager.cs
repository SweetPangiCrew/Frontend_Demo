using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;


public class ChatManager : MonoBehaviour
{
    public GameObject YellowArea, OrangeArea, DateArea;
    public RectTransform ContentRect;
    public Scrollbar scrollBar;

    AreaScript LastArea;

    [SerializeField]
    public Speaker[] speakers;

    [SerializeField]
    public List<DialogueData> dialogues;

    [SerializeField]
    private bool isStart = false;
    public bool isChatting = false;
    private int currentDialogueIndex = -1;
    private int currentSpeakerIndex = 0;

    public bool isFirst = false;

    void Start()
    {

    }

    void Update()
    {
        if (isFirst)
        {
            StartCoroutine(AutoDialogue());
            isFirst = false;
        }

    }
    private IEnumerator AutoDialogue()
    {
        while (dialogues.Count > currentDialogueIndex + 1)
        {
            SetNextDialogue();
            yield return new WaitForSeconds(1); // 1초 대기
        }

        // 대화가 끝난 후 처리
        isChatting = false;
    }


    //public void UpdateDialogue()
    //{
    //    isChatting = true;

    //    if (isFirst)
    //    {
    //        if (isAutoStart) SetNextDialogue();

    //        isFirst = false;
    //    }
    //}

    private void SetNextDialogue()
    {
        currentDialogueIndex++;
        
        currentSpeakerIndex = dialogues[currentDialogueIndex].speakerIndex;

        GameObject TextClone = Instantiate(speakers[currentSpeakerIndex].textArea, ContentRect);

        AreaScript Area = TextClone.GetComponent<AreaScript>();

        Area.TextRect.GetComponent<TextMeshProUGUI>().text = dialogues[currentDialogueIndex].dialogue;
        Area.NameText.text = dialogues[currentDialogueIndex].name;

        speakers[currentSpeakerIndex].dialogueText.text = dialogues[currentDialogueIndex].dialogue;

        scrollBar.value = 0;
    }

}


[System.Serializable]
public struct Speaker
{
    public GameObject textArea;
    public Image dialougeImage;
    public TextMeshProUGUI dialogueText;
    
}

[System.Serializable]
public struct DialogueData
{
    public int speakerIndex;
    public string name;
    public string dialogue;
}


