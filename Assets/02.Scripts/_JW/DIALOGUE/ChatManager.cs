using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;


public class ChatManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject YellowArea, OrangeArea, DateArea;
    public RectTransform ContentRect;
    public Scrollbar scrollBar;

    private bool isStart = false;
    public bool isChatting = false;
    private int currentDialogueIndex = -1;
    private int currentSpeakerIndex = 0;
    public bool isFirst = false;
    public int npcIndex;

    [SerializeField] public List<Speaker> speakers;
    [SerializeField] public List<DialoguesList> dialogues = new List<DialoguesList>();
    [SerializeField] public List<DialogueData> dialogueHistoryList = new List<DialogueData>();

    public bool isChattingHistory = false;

    void Start()
    {
        if (dialogues == null)
        {
            for (int j = 0; j < gameManager.NPC.Count+1; j++)
            {   
                dialogues.Add(new DialoguesList());
            }
        }
    }

    public void StartDialogue(int npcIndex)
    {
     
        if (isChatting&& !isChattingHistory)
        {
            StartCoroutine(AutoDialogue(npcIndex));                     
        }
    }

    public void LoadDialogue(List<List<string>> chatList, int npcIndex)
    {
        for (int k = 0; k < chatList.Count; k++)
        {
            var chat = chatList[k];
            string speaker = chat[0].ToString();
            string dialogue = chat[1].ToString();

            if (dialogues.Count < gameManager.NPC.Count)
            {
                for (int j = 0; j < gameManager.NPC.Count; j++)
                {   
                    dialogues.Add(new DialoguesList());
                }
            }

            dialogues[npcIndex].dialogues.Add(new DialogueData
            {
                dialogue = dialogue,
                name = speaker,
                speakerIndex = k % 2
            });                         

        }

        isChatting = true;
    }
    
    
    public void showDialogue(List<List<string>> chatInfo)
    {
        
        
      
        int speakerIndex = 0;
        
        for (int k = 0; k < chatInfo.Count; k++)
        {
            string speaker = chatInfo[k][0];
            string dialogue = chatInfo[k][1];
            if (dialogueHistoryList.Count <= k)
            {
                if (k % 2 == 0)
                    speakerIndex = 0;
                else
                    speakerIndex = 1;

                dialogueHistoryList.Add(new DialogueData { dialogue = dialogue, name = speaker, speakerIndex = speakerIndex });
            }
        }

        currentDialogueIndex = 0;
        while (dialogueHistoryList.Count > currentDialogueIndex )
        {  //Debug.Log("---------"+dialogues.Count + ": "+currentDialogueIndex);
            SetNextDialogueOnlyForHistory();
            currentDialogueIndex++;
        }
        
        isChatting = false;
    }
    
    private void SetNextDialogueOnlyForHistory()
    {
        currentSpeakerIndex = dialogueHistoryList[currentDialogueIndex].speakerIndex;

        // Kakao Talk Dialogue
        GameObject TextClone = Instantiate(speakers[currentSpeakerIndex].textArea, ContentRect);
        AreaScript Area = TextClone.GetComponent<AreaScript>();
        
        Debug.Log("prefab 생성 대화");
        Area.TextRect.GetComponent<TextMeshProUGUI>().text = dialogueHistoryList[currentDialogueIndex].dialogue;
        Area.NameText.text = dialogueHistoryList[currentDialogueIndex].name;
        scrollBar.value = 1f;
    }

    private IEnumerator AutoDialogue(int npcIndex)
    {
        while (dialogues[npcIndex].dialogues.Count > currentDialogueIndex + 1)
        {
            SetNextDialogue(npcIndex);
            yield return new WaitForSeconds(2);
        }

        isChatting = false;
    }

    private void SetNextDialogue(int npcIndex)
    {
        SetActiveObjects(speakers[currentSpeakerIndex], false);
        currentDialogueIndex++;
        
        if (currentDialogueIndex < dialogues[npcIndex].dialogues.Count)
        {
            if(dialogues[npcIndex].dialogues != null)
            {
                currentSpeakerIndex = dialogues[npcIndex].dialogues[currentDialogueIndex].speakerIndex;

                // Kakao Talk Dialogue
                GameObject TextClone = Instantiate(speakers[npcIndex].textArea, ContentRect);
                AreaScript Area = TextClone.GetComponent<AreaScript>();

                Area.TextRect.GetComponent<TextMeshProUGUI>().text = dialogues[npcIndex].dialogues[currentDialogueIndex].dialogue;
                Area.NameText.text = dialogues[npcIndex].dialogues[currentDialogueIndex].name;

                // Speech Bubble Dialogue
                SetActiveObjects(speakers[npcIndex], true);
                speakers[npcIndex].dialogueText.text = dialogues[npcIndex].dialogues[currentDialogueIndex].dialogue;

                scrollBar.value = 0;
            }
        }
        else
        {
            Debug.Log("no dialogues exist!");
            //isChatting = false; 
        }
        scrollBar.value += 0.1f;
    }

    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.dialougeImage.gameObject.SetActive(visible);
        speaker.dialogueText.gameObject.SetActive(visible);
    }   
}


[System.Serializable]
public struct Speaker
{       
    public string name;
    public GameObject textArea;
    public Image dialougeImage;
    public TextMeshProUGUI dialogueText;

}

[System.Serializable]
public class DialoguesList
{
    public List<DialogueData> dialogues = new List<DialogueData>();
}

[System.Serializable]
public struct DialogueData
{
    public int speakerIndex;
    public string name;
    public string dialogue;
}
