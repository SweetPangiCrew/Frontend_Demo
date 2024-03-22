using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCrIndex : MonoBehaviour
{
    public TextMeshProUGUI rIndex_text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int rIndex = ReligiousIndexNetworkManager.Instance.RIndexInfo[gameObject.name];

        if(rIndex < 10)
        {
            rIndex_text.text = "(" + rIndex.ToString() + ")";
        }
        else if(rIndex < 20)
        {
            rIndex_text.text = "+ (" + rIndex.ToString() + ")";
        }
        else if (rIndex < 25)
        {
            rIndex_text.text = "++ (" + rIndex.ToString() + ")";
        }
        else
        {
            rIndex_text.text = "+++ (" + rIndex.ToString() + ")";
        }
    }
}
