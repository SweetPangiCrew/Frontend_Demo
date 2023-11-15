using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using NPCServer;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

[SerializeField]
public class PerceivedInfo
{
    public string persona;

    public string curr_address;

    public List<PerceivedTile> perceived_tiles;

}

[SerializeField]
public class PerceivedTile
{
    public float dist;    
    public List<string> @event;
}

public class Perceive
{
    public List<PerceivedInfo> perceived_info { get; set; }
}

