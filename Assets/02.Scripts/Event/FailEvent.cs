using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FailEvent : MonoBehaviour
{
    public string[] speech;
    public TextMeshProUGUI text;
    public GameObject speechObj;

    public string[] npc;

    public GameObject hole;
    public GameObject panel;

    private bool isActive;
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        isActive = true;
        index = 0;

        text.text = speech[index];
        index++;

        npc = new string[] { "유리코", "오화가", "주아령", "고영이", "이화령", "유태호", "변호인", "최문식", "여이삭", "오서달", "이지양", "이자식", "이장남", "김태리" };
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

        // 기계신앙 만세
        yield return new WaitForSeconds(1.5f);
        GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(true);
        GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "기계신앙 만세!!!";

        yield return new WaitForSeconds(1f);

        // 말풍선 생성
        for (int i = 0; i < npc.Length; i++)
        {
            if (GameObject.Find(npc[i]) != null)
            {
                GameObject.Find(npc[i]).transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(true);
                GameObject.Find(npc[i]).transform.Find("NPC Canvas/Speach Bubble").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "와아";
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

        GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(false);


        // 구멍 생성
        yield return new WaitForSeconds(1.5f);
        hole.SetActive(true);

        // 갑시다
        yield return new WaitForSeconds(1.5f);
        GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(true);
        GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "갑시다!";

        yield return new WaitForSeconds(2f);

        GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(false);

        // 한명씩 점프
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < npc.Length; i++)
        {
            if (GameObject.Find(npc[i]) != null)
            {
                GameObject.Find(npc[i]).GetComponent<Animator>().enabled = true;
                yield return new WaitForSeconds(1.5f);
            }
        }

        yield return new WaitForSeconds(1.5f);
        GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject.SetActive(true);
        GameObject.Find("나주교").transform.Find("NPC Canvas/Speach Bubble").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "킄..크킄..";

        yield return new WaitForSeconds(3f);

        // ending
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeIn()
    {
/*        float elapsedTime = 0f;
        float fadedTime = 5.0f;

        while (elapsedTime <= fadedTime)
        {
            panel.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(0f, 1f, elapsedTime / fadedTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }*/

        Color color = panel.GetComponent<Image>().color;
        while (color.a < 1)
        {
            color.a += Time.deltaTime / 4;
            panel.GetComponent<Image>().color = color;
            yield return null;
        }
        
    }

}
