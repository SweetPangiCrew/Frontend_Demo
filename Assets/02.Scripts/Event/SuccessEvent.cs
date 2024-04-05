using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SuccessEvent : MonoBehaviour
{
    GameObject player;
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("플레이어");
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

        yield return new WaitForSeconds(4.0f);

        // 나가 멘트 출력
        player.transform.Find("Canvas").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        // 나주교 놀람
        GameObject speech = GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject;
        speech.SetActive(true);
        speech.GetComponentInChildren<TextMeshProUGUI>().text = "!!";
        GameObject.Find("나주교").GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(2.0f);
        speech.SetActive(false);

        yield return new WaitForSeconds(1.0f);
        player.transform.Find("Canvas").gameObject.SetActive(false);

        // 나주교 앞으로 이동
        player.GetComponent<Animator>().SetBool("fight", true);
        yield return new WaitForSeconds(4.0f);

        // 뭐 뭐야
        speech.SetActive(true);
        speech.GetComponentInChildren<TextMeshProUGUI>().text = "뭐..뭐야!";

        // 나주교 한방 먹이기
        player.GetComponent<Animator>().SetBool("hit", true);
        yield return new WaitForSeconds(2.0f);
        speech.SetActive(false);

        yield return new WaitForSeconds(2.5f);
        GameObject.Find("나주교").GetComponent<Animator>().SetBool("hit", true);

        // ...
        yield return new WaitForSeconds(1.5f);
        speech.SetActive(true);
        speech.GetComponentInChildren<TextMeshProUGUI>().text = "...";
        yield return new WaitForSeconds(2.0f);

        //나주교 도망
        speech.SetActive(false);
        player.GetComponent<Animator>().SetBool("side", true);
        yield return new WaitForSeconds(1.0f);
        GameObject.Find("나주교").GetComponent<Animator>().SetBool("run", true);

        speech.SetActive(true);
        speech.GetComponentInChildren<TextMeshProUGUI>().text = "두고봐라..!";

        //자연신앙 만세
        yield return new WaitForSeconds(4.0f);
        GameObject.Find("오화가").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(true);
        GameObject.Find("오화가").transform.Find("NPC Canvas/Speach Bubble").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "자연신앙 만세!!!";
        yield return new WaitForSeconds(4.0f);

        //fade in
        StartCoroutine("FadeIn");

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
