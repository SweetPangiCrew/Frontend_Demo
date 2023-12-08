using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class DialogueManager : MonoBehaviour
{
    
    public Speaker[] speakers;

    public List<DialogueData> dialogues;

    private bool isAutoStart = true;
    private bool isFirst = true;

    public bool isChatting = false;


    private int currentDialogueIndex = -1;
    private int currentSpeakerIndex = 0;

    private void Setup()
    {
        for(int i = 0; i < speakers.Length; i++)
        {
            SetActiveObjects(speakers[i], false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@mOUSE!!!!!!!!!!!!!!!!!!1");
            if (dialogues.Count > currentDialogueIndex + 1)
            {
                SetNextDialogue();
            }
            else
            {
                for (int i = 0; i < speakers.Length; i++)
                {
                    //SetActiveObjects(speakers[i], false);
                    isChatting = false;
                }

            }
        }
    }

    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.imageDialogue.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);
    }

    public void UpdateDialogue()
    {
        isChatting = true;
        
        if (isFirst)
        {
            //Setup();

            if (isAutoStart) SetNextDialogue();

            isFirst = false;
        }

        
        

    }

    private void SetNextDialogue()
    {
        //SetActiveObjects(speakers[currentSpeakerIndex], false);
        currentDialogueIndex++;
        //currentSpeakerIndex = dialogues[currentDialogueIndex].speakerIndex;

        //SetActiveObjects(speakers[currentDialogueIndex], true);
        speakers[0].textDialogue.text = dialogues[currentDialogueIndex].dialogue;
    }

}

[System.Serializable]
public struct Speaker
{
    public Image imageDialogue;
    public TextMeshProUGUI textDialogue;
}

[System.Serializable]
public struct DialogueData
{
    public string dialogue;

}
