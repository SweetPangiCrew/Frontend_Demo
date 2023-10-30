/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEditor.VersionControl;
using TMPro;

public class BlackboardController : MonoBehaviour
{
    public BehaviourTreeInstance behaviourTreeInstance; // Assign in the inspector
    public TextMeshProUGUI LogText;
    BlackboardKey<string> logReference; // Cache the key reference to avoid lookups each frame

    public void LogVisualizeUI()
    {
        // Simple version. Finds the key on each call to Get/Set
        string message = behaviourTreeInstance.GetBlackboardValue<string>("StateLog");

        // ex) luna is gardening
        behaviourTreeInstance.SetBlackboardValue("StateLog", "Luna is watering the plant...");

        // Cached version, find the key and store a reference to it.
        logReference = behaviourTreeInstance.FindBlackboardKey<string>("StateLog");
        
        LogText.text = logReference.value.ToString();
      
    }

    public void Interact(){
        List<GameObject> tiles = this.GetComponent<Luna>().detectedTiles;
        foreach (GameObject tile in tiles) 
        {
            if(tile.CompareTag("Player")){  // if player exists near
                // wait and stop rountine
                Debug.Log("I FOUND PLAYER!!!");
            }
        }     
    }
}
*/