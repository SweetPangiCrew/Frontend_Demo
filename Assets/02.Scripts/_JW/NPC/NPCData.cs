// Object of this class will hold the data
// And then this object will be converted to JSON
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData
{
    public string name;
    public Vector3 location;
    public List<GameObject> detectedObject;

}
