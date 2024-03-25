using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCrIndex : MonoBehaviour
{
    public TextMeshProUGUI rIndex_text;
    public GameObject[] rIndex_obj;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int rIndex = ReligiousIndexNetworkManager.Instance.RIndexInfo[gameObject.name];

        if (rIndex < 10) //0단계
        {
            rIndex_obj[0].SetActive(false);
            rIndex_obj[1].SetActive(false);
            rIndex_obj[2].SetActive(false);
            rIndex_text.text = "(" + rIndex.ToString() + ")";
        }
        else if(rIndex < 20)    //1단계
        {
            rIndex_obj[0].SetActive(true);
            rIndex_obj[1].SetActive(false);
            rIndex_obj[2].SetActive(false);
            rIndex_text.text = "(" + rIndex.ToString() + ")";
        }
        else if (rIndex < 30)   //2단계
        {
            rIndex_obj[0].SetActive(true);
            rIndex_obj[1].SetActive(true);
            rIndex_obj[2].SetActive(false);
            rIndex_text.text = "(" + rIndex.ToString() + ")";
        }
        else //3단계
        {
            rIndex_obj[0].SetActive(true);
            rIndex_obj[1].SetActive(true);
            rIndex_obj[2].SetActive(true);
            rIndex_text.text = "(" + rIndex.ToString() + ")";
        }
    }
}
