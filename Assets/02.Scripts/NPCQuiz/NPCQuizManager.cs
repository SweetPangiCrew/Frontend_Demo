using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCQuizManager : MonoBehaviour
{
    public TextMeshProUGUI quizText;
    public TMP_InputField answerText;

    public GameObject quizPanel;
    public GameObject answerPanel;
    public GameObject submit;

    public GameObject correctPanel;
    public GameObject wrongPanel;

    NPCQuiz npcQuiz;
    List<QuizData> quizList;
    int quizNum;

    private void OnEnable()
    {
        Time.timeScale = 0;

        quizPanel.SetActive(true);
        answerPanel.SetActive(true);
        submit.SetActive(true);

        //NPC 퀴즈 내용 불러오기
        npcQuiz = GameObject.Find("이자식").GetComponent<NPCQuiz>();
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

        correctPanel.SetActive(true);

        //2초 후 창 사라짐
        float timer = 0;
        while(timer < 2)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        correctPanel.SetActive(false);

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

        wrongPanel.SetActive(true);

        //2초 후 창 사라짐
        float timer = 0;
        while (timer < 2)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        wrongPanel.SetActive(false);

        //시간 원상태 후 창 사라짐
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    private IEnumerator QuizEnd()
    {
        correctPanel.SetActive(true);
        correctPanel.GetComponentInChildren<TextMeshProUGUI>().text = "모든 퀴즈를 맞히셨습니다!";

        float timer = 0;
        while (timer < 2)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        correctPanel.SetActive(false);

        //시간 원상태 후 창 사라짐
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
