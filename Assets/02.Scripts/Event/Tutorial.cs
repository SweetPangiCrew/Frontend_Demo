using NPCServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public string[] speech;
    public TextMeshProUGUI text;
    public TextMeshProUGUI username_text;
    public GameObject speechObj,tutorialObj;
    
    public GameObject panel;
    
    public GameObject[] tutorial;

    private bool isActive;
    private int index , tindex;

    private string username = "고영희";
    // Start is called before the first frame update
    void Start()
    {
        isActive = true;
        index = 0;
       
        username = PlayerPrefs.GetString("username", "고영희"); 
        speech[1] = "나 "+username+"! 자연 신앙의 신도로서 용서할 수 없다!"; 
        
        text.text = speech[index];
        username_text.text = username;
        index++;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)||Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.D))
        {
            //튜토리얼
            if (tindex >= tutorial.Length-1)
            {
                // tutorial[tindex].SetActive(false);
                // tutorialObj.SetActive(false);
            }
            else if(index > speech.Length &&tindex<tutorial.Length-1)
            {
                tindex++;
                tutorial[tindex-1].SetActive(false);
                tutorial[tindex].SetActive(true);
               
            }
        
            if(tindex>=3 && NPCServerManager.Instance.serverOpened)
                Invoke("loadNextScene",1f);
            
            //스피치
            if (index == speech.Length)
            {
                speechObj.SetActive(false);
                tutorialObj.SetActive(true);
                tindex = 0;
                tutorial[tindex].SetActive(true);
                index++;

            }
            else if(index < speech.Length)
            {
                text.text = speech[index];
                index++;
            }

            
        }
    }

    void loadNextScene()
    {
        SceneManager.LoadScene("Map");
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
