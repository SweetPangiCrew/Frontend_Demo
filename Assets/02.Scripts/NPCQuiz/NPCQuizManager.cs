using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCQuizManager : MonoBehaviour
{
    //퀴즈, 답 내용
    public TextMeshProUGUI quizText;
    public TMP_InputField answerText;

    //메인 퀴즈 창
    public GameObject quizPanel;
    public GameObject answerPanel;
    public GameObject submit;

    //정답, 오답 창
    public GameObject InfoPanel;

    //NPC 퀴즈 리스트
    NPCQuiz npcQuiz;
    List<QuizData> quizList;
    int quizNum;

    //퀴즈 대상 NPC 이름
    public string NPCName;

    private void OnEnable()
    {
        Time.timeScale = 0;

        quizPanel.SetActive(true);
        answerPanel.SetActive(true);
        submit.SetActive(true);

        //NPC 퀴즈 내용 불러오기
        //NPCName = "이자식";    //퀴즈 대상 불러오기

        npcQuiz = GameObject.Find(NPCName).GetComponent<NPCQuiz>();
        quizList = npcQuiz.Quiz;
        QuizOpen();
    }

    void QuizOpen()
    {
        answerText.text = null;

        //정답을 맞히지 않은 퀴즈 선택해서 보여주기
        for (int i=0; i<quizList.Count; i++)
        {
            if (!quizList[i].correct)
            {
                quizText.text = quizList[i].quiz;
                quizNum = i;
                break;
            }
        }
    }

    //제출하기 버튼에 붙는 함수
    public void CompareAnswer()
    {
        Debug.Log("입력: " + answerText.text.Trim() + answerText.text.GetType());
        Debug.Log("정답: " + quizList[quizNum].answer + quizList[quizNum].answer.GetType());

        //입력한 답과 정답 비교
        if (answerText.text.Trim() == quizList[quizNum].answer.Trim())
        {
            Debug.Log("정답!");

            //정답 시 correct 값 true로 변경
            QuizData data = quizList[quizNum];
            data.correct = true;
            npcQuiz.Quiz[quizNum] = data;

            //정답 패널 보여주기
            StartCoroutine("CorrectPanel");
        }
        else
        {
            Debug.Log("오답!");

            //오답 패널 보여주기
            StartCoroutine("WrongPanel");
        }
    }

    private IEnumerator CorrectPanel()
    {
        quizPanel.SetActive(false);
        answerPanel.SetActive(false);
        submit.SetActive(false);

        InfoPanel.SetActive(true);
        InfoPanel.GetComponentInChildren<TextMeshProUGUI>().text = "정답입니다! (" + NPCName + "의 종교 친화 지수 - 5)";

        //NPC 종교 친화 지수 5 감소
        Dictionary<string, int> updateInfo = new Dictionary<string, int>();
        updateInfo[NPCName] = -5;
        StartCoroutine(ReligiousIndexNetworkManager.Instance.UpdateRIndexCoroutine(updateInfo));
        Debug.Log(NPCName + ReligiousIndexNetworkManager.Instance.RIndexInfo[NPCName].ToString());

        //2초 후 창 사라짐
        float timer = 0;
        while(timer < 2)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        InfoPanel.SetActive(false);

        if (quizList.FindIndex(item => item.correct == false) == -1)
        {
            //퀴즈를 다 맞힌 경우
            npcQuiz.quizEnd = true;
            StartCoroutine("QuizEnd");
        }
        else
        {
            quizPanel.SetActive(true);
            answerPanel.SetActive(true);
            submit.SetActive(true);

            //다음 퀴즈 생성
            QuizOpen();
        }

    }

    private IEnumerator WrongPanel()
    {
        quizPanel.SetActive(false);
        answerPanel.SetActive(false);
        submit.SetActive(false);

        InfoPanel.SetActive(true);
        InfoPanel.GetComponentInChildren<TextMeshProUGUI>().text = "오답입니다!";

        //2초 후 창 사라짐
        float timer = 0;
        while (timer < 2)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        InfoPanel.SetActive(false);

        //시간 원상태 후 창 사라짐
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    private IEnumerator QuizEnd()
    {
        InfoPanel.SetActive(true);
        InfoPanel.GetComponentInChildren<TextMeshProUGUI>().text = "모든 퀴즈를 맞히셨습니다!";

        float timer = 0;
        while (timer < 2)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        InfoPanel.SetActive(false);

        //시간 원상태 후 창 사라짐
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
