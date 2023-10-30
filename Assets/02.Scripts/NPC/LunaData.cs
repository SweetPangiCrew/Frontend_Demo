// Object of this class will hold the data
// And then this object will be converted to JSON
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LunaData : NPCData
{
    // this
    public string name;
    public Vector3 location;
    public List<GameObject> detectedObject;

    // NPC
    public string NPC_Name;
    public Vector3 NPC_Location;
 

}
