using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCQuiz : MonoBehaviour
{
    public bool quizEnd;

    [SerializeField]
    public List<QuizData> Quiz;

    // Start is called before the first frame update
    void Start()
    {
        quizEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

[System.Serializable]
public struct QuizData
{
    public string quiz;
    public string answer;
    public bool correct;
}
