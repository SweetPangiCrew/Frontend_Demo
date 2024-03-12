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
    public bool isChattingHistory = false;
    void Start()
    {
        if (isChattingHistory)
        {
            
        }
    }

    void Update()
    {
        if (isFirst&& !isChattingHistory)
        {
            StartCoroutine(AutoDialogue());
            isFirst = false;
        }

    }
    
    public void showDialogue(List<List<string>> chatInfo)
    {
      
        int speakerIndex = 0;
        
        for (int k = 0; k < chatInfo.Count; k++)
        {
            string speaker = chatInfo[k][0];
            string dialogue = chatInfo[k][1];
            if (dialogues.Count <= k)
            {
                if (k % 2 == 0)
                    speakerIndex = 0;
                else
                    speakerIndex = 1;

                dialogues.Add(new DialogueData { dialogue = dialogue, name = speaker, speakerIndex = speakerIndex });
            }
        }
        
      
        currentDialogueIndex = 0;
        while (dialogues.Count > currentDialogueIndex )
        {  //Debug.Log("---------"+dialogues.Count + ": "+currentDialogueIndex);
            SetNextDialogueOnlyForHistory();
            currentDialogueIndex++;
            
        }
        
        isChatting = false;
    }
    
    private void SetNextDialogueOnlyForHistory()
    {
        currentSpeakerIndex = dialogues[currentDialogueIndex].speakerIndex;

        // Kakao Talk Dialogue
        GameObject TextClone = Instantiate(speakers[currentSpeakerIndex].textArea, ContentRect);
        AreaScript Area = TextClone.GetComponent<AreaScript>();

        Area.TextRect.GetComponent<TextMeshProUGUI>().text = dialogues[currentDialogueIndex].dialogue;
        Area.NameText.text = dialogues[currentDialogueIndex].name;
        scrollBar.value = 1f;
    }

    private IEnumerator AutoDialogue()
    {
        while (dialogues.Count > currentDialogueIndex + 1)
        {
            SetNextDialogue();
            yield return new WaitForSeconds(2); // 1�� ���
        }

        // ��ȭ�� ���� �� ó��
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
        SetActiveObjects(speakers[currentSpeakerIndex], false);

        currentDialogueIndex++;
        currentSpeakerIndex = dialogues[currentDialogueIndex].speakerIndex;

        // Kakao Talk Dialogue
        GameObject TextClone = Instantiate(speakers[currentSpeakerIndex].textArea, ContentRect);
        AreaScript Area = TextClone.GetComponent<AreaScript>();

        Area.TextRect.GetComponent<TextMeshProUGUI>().text = dialogues[currentDialogueIndex].dialogue;
        Area.NameText.text = dialogues[currentDialogueIndex].name;

        // Speech Bubble Dialogue
        SetActiveObjects(speakers[currentSpeakerIndex], true);
        speakers[currentSpeakerIndex].dialogueText.text = dialogues[currentDialogueIndex].dialogue;

        scrollBar.value += 0.1f;
    }

    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.dialougeImage.gameObject.SetActive(visible);
        speaker.dialogueText.gameObject.SetActive(visible);
    }

    public void SetIsChatting(bool _isChatting)
    {
        isChatting = _isChatting;
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


