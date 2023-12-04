using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    Dictionary<string, List<string>> dialogueData;

    GameManager manager;

    void Awake()
    {
        dialogueData = new Dictionary<string, List<string>>();
    }

    public void GenerateData(string NPCName, List<string> dialogueList)
    {
        if (!dialogueData.ContainsKey(NPCName))
        {
            dialogueData.Add(NPCName, dialogueList);
        }
    }
    
    public string GetDialogue(string NPCname, int dialogueIndex)
    {
        if(dialogueIndex == dialogueData[NPCname].Count)
            return null; // there is no setences left
        else
            return dialogueData[NPCname][dialogueIndex];
    }
}
