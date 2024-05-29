using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCrIndex : MonoBehaviour
{
    public TextMeshProUGUI rIndex_text;
    public GameObject[] rIndex_obj;

    public bool set = false;
    //private int curr_hour;
    public int next_hour;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 1시간 마다 종교 친화 지수 +3
        if (!set)
        {
            Debug.Log(NPCServer.Clock.Instance.GetCurrentTime());
            Debug.Log(NPCServer.Clock.Instance.GetCurrentTime().Hour);
            next_hour = NPCServer.Clock.Instance.GetCurrentTime().Hour + 1;
            set = true;
        }

        if(NPCServer.Clock.Instance.GetCurrentTime().Hour == next_hour && set == true)
        {
            Debug.Log("+3");
            set = false;
            ReligiousIndexNetworkManager.Instance.RIndexInfo[gameObject.name] += 1;
        }

        if (ReligiousIndexNetworkManager.Instance.RIndexInfo == null) return;
        if(ReligiousIndexNetworkManager.Instance.RIndexInfo.Count == 0 ) return;
        int rIndex = ReligiousIndexNetworkManager.Instance.RIndexInfo[gameObject.name];
        //Debug.Log(gameObject.name + rIndex.ToString());

        if(rIndex > 40)
        {
            ReligiousIndexNetworkManager.Instance.RIndexInfo[gameObject.name] = 40;
            rIndex = ReligiousIndexNetworkManager.Instance.RIndexInfo[gameObject.name];
        }

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
