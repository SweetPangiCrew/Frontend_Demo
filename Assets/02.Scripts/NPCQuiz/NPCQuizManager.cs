using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCQuizManager : MonoBehaviour
{
    public TextMeshProUGUI quizText;
    public TMP_InputField answerText;

    public GameObject correctPanel;
    public GameObject wrongPanel;

    NPCQuiz npcQuiz;
    List<QuizData> quizList;
    int quizNum;

    // Start is called before the first frame update
    void Start()
    {
/*        //NPC 퀴즈 내용 불러오기
        npcQuiz = GameObject.Find("이자식").GetComponent<NPCQuiz>();
        quizList = npcQuiz.Quiz;
        QuizOpen();*/
    }
    private void OnEnable()
    {
        Time.timeScale = 0;

        //NPC 퀴즈 내용 불러오기
        npcQuiz = GameObject.Find("이자식").GetComponent<NPCQuiz>();
        quizList = npcQuiz.Quiz;
        QuizOpen();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            //StartCoroutine("CorrectPanel");

            //다음 퀴즈 생성
            QuizOpen();
        }
        else
        {
            Debug.Log("오답!");

            //오답 패널 보여주기
            //StartCoroutine("WrongPanel");

            //시간 원상태 후 창 사라짐
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
    }

    private IEnumerator CorrectPanel()
    {
        correctPanel.SetActive(true);

        yield return new WaitForSeconds(2);
        correctPanel.SetActive(false);

        //다음 퀴즈 생성
        QuizOpen();
    }

    private IEnumerator WrongPanel()
    {
        wrongPanel.SetActive(true);

        yield return new WaitForSeconds(2);
        wrongPanel.SetActive(false);

        //시간 원상태 후 창 사라짐
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
