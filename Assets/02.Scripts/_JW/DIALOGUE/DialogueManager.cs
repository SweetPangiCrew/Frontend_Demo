using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private Speaker[] speakers;

    [SerializeField]
    private DialogueData[] dialogues;

    public bool isAutoStart = true;
    public bool isChatting = false;
    public bool isFirst = true;
    public int currentDialogueIndex = -1;

    private void Setup()
    {
        for(int i = 0; i < speakers.Length; i++)
        {
            SetActiveObjects(speakers[i], false);
        }
    }

    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.imageDialogue.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);
    }

    public void StartDialogue(string speaker, string dialogue)
    {
        StartCoroutine(UpdateDialogue(speaker,dialogue));
        isChatting = true;
    }

    IEnumerator UpdateDialogue(string speaker, string dialogue)
    {
        // Display speaker's name
        //speakerText.text = speaker;

        // Split dialogue into sentences (assuming each dialogue is a separate sentence)
       // string[] sentences = dialogueDataList[currentDialogueIndex].dialogue.Split('\n');

        //// Display each sentence with a delay
        //foreach (string sentence in sentences)
        //{
        //   // dialogueText.text = ""; // Clear previous text

        //    foreach (char letter in sentence)
        //    {
        //        //dialogueText.text += letter;
        //        yield return null; // Wait for a frame before showing the next letter
        //    }

        yield return new WaitForSeconds(1f); // Wait for 1 second before showing the next sentence
        //}

        // Clear dialogue after it's finished
        //dialogue.text = "";
        isChatting = false;
    }
}

[SerializeField]
public struct Speaker
{
    public Image imageDialogue;
    public TextMeshProUGUI textDialogue;
}

[SerializeField]
public struct DialogueData
{
    public string dialogue;

}
