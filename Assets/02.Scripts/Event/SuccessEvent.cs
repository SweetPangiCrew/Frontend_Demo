using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SuccessEvent : MonoBehaviour
{
    GameObject player;
    public GameObject panel;
    public GameObject hole;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("플레이어");
        //StartCoroutine(Moving("김태리", "고영이", "변호인", "이화령"));

        StartCoroutine("Success_ending");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Success_ending()
    {
        yield return new WaitForSeconds(1.5f);

        player.GetComponent<Animator>().enabled = true;
        StartCoroutine(Moving("김태리", "고영이", "여이삭", "변호인"));

        yield return new WaitForSeconds(5.5f);

        // 나가 멘트 출력
        player.transform.Find("Canvas").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        // 나주교 놀람
        GameObject speech = GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject;
        speech.SetActive(true);
        speech.GetComponentInChildren<TextMeshProUGUI>().text = "!!";
        GameObject.Find("나주교").GetComponent<Animator>().enabled = true;

        // 나주교 앞으로 이동
        player.GetComponent<Animator>().SetBool("fight", true);
        yield return new WaitForSeconds(2.0f);
        speech.SetActive(false);

        yield return new WaitForSeconds(1.0f);
        player.transform.Find("Canvas").gameObject.SetActive(false);
        yield return new WaitForSeconds(4.0f);

        // 뭐 뭐야
        speech.SetActive(true);
        speech.GetComponentInChildren<TextMeshProUGUI>().text = "뭐..뭐야!";

        // 나주교 한방 먹이기
        player.GetComponent<Animator>().SetBool("hit", true);
        yield return new WaitForSeconds(2.0f);
        speech.SetActive(false);

        yield return new WaitForSeconds(2.0f);
        GameObject.Find("나주교").GetComponent<Animator>().SetBool("hit", true);

        // ...
        yield return new WaitForSeconds(2.0f);
        speech.SetActive(true);
        speech.GetComponentInChildren<TextMeshProUGUI>().text = "...";
        yield return new WaitForSeconds(2.0f);

        //나가
        GameObject.Find("김태리").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(true);
        GameObject.Find("김태리").transform.Find("NPC Canvas/Speach Bubble").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "다 부수기 전에 나가라";
        yield return new WaitForSeconds(2.0f);
        GameObject.Find("김태리").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(false);

        GameObject.Find("고영이").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(true);
        GameObject.Find("고영이").transform.Find("NPC Canvas/Speach Bubble").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "부끄러운 줄 알아";
        yield return new WaitForSeconds(2.0f);
        GameObject.Find("고영이").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(false);


        // 나주교 도망
        speech.SetActive(false);
        player.GetComponent<Animator>().SetBool("side", true);
        yield return new WaitForSeconds(1.0f);
        GameObject.Find("나주교").GetComponent<Animator>().SetBool("run", true);

        // 구멍 생성 후 나주교 자살
        yield return new WaitForSeconds(2.0f);
        hole.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        speech.SetActive(true);
        speech.GetComponentInChildren<TextMeshProUGUI>().text = "두고봐라..!";
        yield return new WaitForSeconds(2.0f);
        speech.SetActive(false);
        GameObject.Find("나주교").GetComponent<Animator>().SetBool("dead", true);

        // 자연신앙 만세
        yield return new WaitForSeconds(4.0f);
        player.transform.Find("Canvas").gameObject.SetActive(true);
        player.transform.Find("Canvas/Image").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "훗.. 자연신앙 만세다";
        yield return new WaitForSeconds(4.0f);

        //fade in
        StartCoroutine("FadeIn");

    }

    // 등장
    IEnumerator Moving(string name1, string name2, string name3, string name4)
    {
        GameObject npc1 = GameObject.Find(name1);
        GameObject npc2 = GameObject.Find(name2);
        GameObject npc3 = GameObject.Find(name3);
        GameObject npc4 = GameObject.Find(name4);

        npc1.transform.position = new Vector3(24.4f, -33f, 0);
        npc2.transform.position = new Vector3(25.7f, -33f, 0);
        npc3.transform.position = new Vector3(24.5f, -32f, 0);
        npc4.transform.position = new Vector3(25.5f, -32f, 0);

        npc1.GetComponent<Animator>().enabled = true;
        npc2.GetComponent<Animator>().enabled = true;
        npc3.GetComponent<Animator>().enabled = true;
        npc4.GetComponent<Animator>().enabled = true;

        float time = 0;
        time = 0;
        while (time < 3.3)
        {
            npc1.transform.position = new Vector3(npc1.transform.position.x, npc1.transform.position.y - Time.deltaTime * 3.7f, 0);
            npc2.transform.position = new Vector3(npc2.transform.position.x, npc2.transform.position.y - Time.deltaTime * 3.4f, 0);
            npc3.transform.position = new Vector3(npc3.transform.position.x, npc3.transform.position.y - Time.deltaTime * 3.7f, 0);
            npc4.transform.position = new Vector3(npc4.transform.position.x, npc4.transform.position.y - Time.deltaTime * 3.5f, 0);
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        while (time < 2)
        {
            npc1.transform.position = new Vector3(npc1.transform.position.x + Time.deltaTime * 1.7f, npc1.transform.position.y, 0);
            npc2.transform.position = new Vector3(npc2.transform.position.x + Time.deltaTime * 1.5f, npc2.transform.position.y, 0);
            npc3.transform.position = new Vector3(npc3.transform.position.x + Time.deltaTime * 1.0f, npc3.transform.position.y, 0);
            npc4.transform.position = new Vector3(npc4.transform.position.x + Time.deltaTime * 1.0f, npc4.transform.position.y, 0);
            time += Time.deltaTime;
            yield return null;
        }
        Debug.Log("끗");
    }

    IEnumerator FadeIn()
    {
        Color color = panel.GetComponent<Image>().color;
        while (color.a < 1)
        {
            color.a += Time.deltaTime / 4;
            panel.GetComponent<Image>().color = color;
            yield return null;
        }

    }
}
