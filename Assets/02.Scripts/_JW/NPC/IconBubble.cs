using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using TMPro;

public class IconBubble : MonoBehaviour
{
    public GameObject descriptionBar;
    private Transform root;

    
    void Start()
    {
        root = this.transform.parent.parent;
    }

   

  
}
