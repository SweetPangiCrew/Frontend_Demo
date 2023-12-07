using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    public class Dialogue
    {
        public string speaker;
        public string dialogue;

        public Dialogue(string speaker, string dialogue)
        {
            this.speaker = speaker;
            this.dialogue = dialogue;
        }
    }

    Dictionary<string, List<Dialogue>> dialogueData;
    GameManager manager;

    void Awake()
    {
        dialogueData = new Dictionary<string, List<Dialogue>>();
    }

    public void GenerateData(string NPCName, List<string> dialogueList)
    {
        if (!dialogueData.ContainsKey(NPCName))
        {
            List<Dialogue> dialogues = new List<Dialogue>();

            foreach(var dialogueInfo in dialogueList)
            {
                //Dialogue dialogue = new Dialogue(dialogueInfo[0], dialogueInfo[1]);
                //dialogues.Add(dialogue);
            }
        
            
        }
    }
    
    /*
    public string GetDialogue(string NPCname, int dialogueIndex)
    {
        if(dialogueIndex == dialogueData[NPCname].Count)
            return null; // there is no setences left
        else
            return dialogueData[NPCname][dialogueIndex];
    }*/
}
