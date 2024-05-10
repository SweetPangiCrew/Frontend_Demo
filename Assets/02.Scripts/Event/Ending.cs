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
    //public TextMeshProUGUI text;

    private int assembled_num;
    float timer;
    bool location;

    // Start is called before the first frame update
    void Start()
    {
        assembled_num = 0;
        timer = 0;
        location = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(NPCServer.Clock.Instance.GetCurrentTime());
        DateTime dateTime = new DateTime(DateTime.Now.Year, 8, 1, 12, 0, 0);
        Debug.Log(dateTime);

        // 종교 집회 시작
        if(DateTime.Compare(NPCServer.Clock.Instance.GetCurrentTime(), dateTime) > 0)
        {
            Debug.Log("종교 집회 갑시다");
            endingCollider.SetActive(true);

            // NPC 종교 집회로 location 설정
/*            if (location)
            {
                GameObject[] NPC;
                NPC = GameObject.FindGameObjectsWithTag("NPC");
                foreach (GameObject npc in NPC)
                {
                    int rIndex = ReligiousIndexNetworkManager.Instance.RIndexInfo[npc.name];
                    if(rIndex >= 0)
                    {
                        npc.GetComponent<NPC>().AddWaypoint(GameManager.Instance.location[13], 1000);
                    }
                }
                location = false;
            }*/

            panel.SetActive(true);
            assembled_num = gameObject.GetComponentInChildren<EndingCollider>().assembled_num;
            panel.GetComponentInChildren<TextMeshProUGUI>().text = "나주교의 종교 집회: " + assembled_num + "명";

            // 테스트용 10초 뒤 엔딩
/*            timer += Time.deltaTime;
            if (timer > 10)
            {
                SceneManager.LoadScene("FailEvent");
            }*/
        }

    }
}
