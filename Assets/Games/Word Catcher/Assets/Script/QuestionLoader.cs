using UnityEngine;

public class QuestionLoader : MonoBehaviour
{
    public QuestionList questionList;
    public BallQuizManager ballQuizManager;
    void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("WordCatcher");
        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found! Make sure 'questions.json' is in Resources folder.");
            return;
        }

        questionList = JsonUtility.FromJson<QuestionList>(jsonFile.text);
        if (questionList == null || questionList.questions == null)
        {
            Debug.LogError("Failed to parse JSON. Check JSON format and class definitions.");
            return;
        }

        ballQuizManager.SetQuestions(questionList);
    }
}

[System.Serializable]
public class Question
{
    public string question;
    public string[] options;
    public string correct;
}

[System.Serializable]
public class QuestionList
{
    public Question[] questions;
}
