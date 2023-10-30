using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimData : NPCData
{
    public string name { get; set; }
    public Vector3 location { get; set; }
    public List<GameObject> detectedObject { get; set; }
}
