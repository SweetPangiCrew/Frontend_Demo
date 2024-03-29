using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FailEvent : MonoBehaviour
{
    public string[] speech;
    public TextMeshProUGUI text;
    public GameObject speechObj;

    public string[] npc;

    public GameObject hole;

    private bool isActive;
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        isActive = true;
        index = 0;

        text.text = speech[index];
        index++;

        npc = new string[] { "고영이", "김태리", "변호인", "여이삭", "오서달", "오화가", "유리코", "유태호", "이자식", "이장남", "이지양", "이화령", "주아령", "최문식" };
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (index >= speech.Length)
            {
                index = 0;
                isActive = false;
                speechObj.SetActive(false);

                StartCoroutine("Fail_ending");
            }

            text.text = speech[index];
            index++;
        }
    }

    IEnumerator Fail_ending()
    {
        yield return new WaitForSeconds(0.5f);

        // 말풍선 생성
        for(int i=0; i< npc.Length; i++)
        {
            if (GameObject.Find(npc[i]) != null)
            {
                GameObject.Find(npc[i]).transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }

        }

        yield return new WaitForSeconds(3f);

        // 말풍선 제거
        for (int i = 0; i < npc.Length; i++)
        {
            if (GameObject.Find(npc[i]) != null)
            {
                GameObject.Find(npc[i]).transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(false);
            }
        }

        // 구멍 생성
        yield return new WaitForSeconds(1.5f);
        hole.SetActive(true);

        // 갑시다
        yield return new WaitForSeconds(1.5f);
        GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(false);

        // 한명씩 점프
        yield return new WaitForSeconds(1.5f);

        GameObject.Find("유리코").GetComponent<Animator>().enabled = true;

    }

}
