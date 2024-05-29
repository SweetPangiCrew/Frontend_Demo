using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    public GameObject endingCollider;
    public GameObject panel;
    public GameObject time;
    //public TextMeshProUGUI text;

    private int assembled_num;
    bool location;

    // Start is called before the first frame update
    void Start()
    {
        assembled_num = 0;
        location = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(NPCServer.Clock.Instance.GetCurrentTime());
        DateTime dateTime = new DateTime(DateTime.Now.Year, 8, 1,23, 0, 0);
        //Debug.Log(dateTime);

        // 종교 집회 시작
        if(DateTime.Compare(NPCServer.Clock.Instance.GetCurrentTime(), dateTime) > 0)
        {
            Debug.Log("종교 집회 갑시다");
            endingCollider.SetActive(true);

            // NPC 종교 집회로 location 설정
            if (location)
            {
                GameObject[] NPC;
                NPC = GameObject.FindGameObjectsWithTag("NPC");
                foreach (GameObject npc in NPC)
                {
                    int rIndex = ReligiousIndexNetworkManager.Instance.RIndexInfo[npc.name];
                    if (rIndex >= 30)
                    {
                        npc.GetComponent<NPC>().locationTag = true;
                        npc.GetComponent<NPC>().isWaiting = false;
                        npc.GetComponent<NPC>().isEnding = true;
                        npc.GetComponent<NPC>().locationTags.Clear();
                        npc.GetComponent<NPC>().AddWaypoint(GameManager.Instance.location[53], 1000);

                        Debug.Log(npc.name + "종교 집회로 이동");
                    }
                }
                location = false;
            }

            panel.SetActive(true);
            time.SetActive(true);
            assembled_num = gameObject.GetComponentInChildren<EndingCollider>().assembled_num;
            panel.GetComponentInChildren<TextMeshProUGUI>().text = "현재 나주교의 종교 집회: " + assembled_num + "명 참석";

            // 엔딩
            DateTime endingTime = new DateTime(DateTime.Now.Year, 8,2,0,30,0);
            if (DateTime.Compare(NPCServer.Clock.Instance.GetCurrentTime(), endingTime) > 0)
            {
                Debug.Log("종교 집회 시작");
                if(assembled_num < 7)
                {
                    SceneManager.LoadScene("SuccessEvent");
                }
                else
                {
                    SceneManager.LoadScene("FailEvent");
                }
                
            }
        }

    }
}
