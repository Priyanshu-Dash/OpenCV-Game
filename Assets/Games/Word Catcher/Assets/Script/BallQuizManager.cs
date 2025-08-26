using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Android;

public enum QuizMode { InClassroom, AtHome }

public class BallQuizManager : MonoBehaviour
{
    [Header("Start Game")]
    public GameObject startPanel;
    public GameObject livesPanel;

    [Header("Answer Ball")]
    public GameObject answerBallPrefab;
    public Transform[] spawnPoints;
    public Transform answerBallParent;
    public float ballSpawnInterval = 1.5f;

    [Header("UI")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI scoreText;

    [Header("References")]
    public BasketController basketController;
    public ParticleSystem correctAnswerEffect;
    public Transform shakeTarget;

    [Header("Result Display")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultQuestionText;
    public TextMeshProUGUI resultAnswerText;

    [Header("Question Text Animation")]
    private RectTransform questionTextRect;

    [Header("Current Question Answer")]
    public string question;
    public string answer;

    [Header("Mode Settings")]
    public QuizMode quizMode = QuizMode.InClassroom;
    public int classroomQuestionCount = 10;

    public GameObject[] heartIcons; // Assign 3 heart GameObjects in Inspector
    public int maxLives = 3;

    private int currentLives;

    [HideInInspector] public QuestionList loadedQuestions;

    private List<Question> questions = new List<Question>();
    private int currentQuestionIndex = 0;
    private int missedBalls = 0;
    private int totalBallsThisQuestion = 0;
    private bool answerCaught = false;
    private int lastUsedSpawnIndex = -1;
    private List<AnswerBall> activeBalls = new List<AnswerBall>();
    private bool stopSpawning = false;
    private bool allowFaceInput = false;


    void Start()
    {
        shakeTarget = Camera.main.transform;
        questionTextRect = questionText.GetComponent<RectTransform>();
        if (startPanel != null)
        {
            startPanel.SetActive(true);
            livesPanel.SetActive(false);
        }
        currentLives = maxLives;
        UpdateHeartsDisplay();
    }
    public void OnClickStartButton()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(false); // Hide start UI
            livesPanel.SetActive(true);
        }

        if (loadedQuestions != null && loadedQuestions.questions.Length > 0)
        {
            InitializeQuestions();
            ShowQuestion(currentQuestionIndex);
        }
        else
        {
            Debug.LogError("No questions assigned to BallQuizManager.");
        }
    }

    void InitializeQuestions()
    {
        questions.Clear();
        Question[] all = loadedQuestions.questions;

        if (quizMode == QuizMode.InClassroom)
        {
            List<Question> shuffled = new List<Question>(all);
            for (int i = 0; i < shuffled.Count; i++)
            {
                int rand = Random.Range(i, shuffled.Count);
                (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
            }

            for (int i = 0; i < Mathf.Min(classroomQuestionCount, shuffled.Count); i++)
            {
                questions.Add(shuffled[i]);
            }
        }
        else
        {
            questions.AddRange(all);
        }
    }

    void ShowQuestion(int index)
    {
        if (index >= questions.Count) return;
        stopSpawning = false;

        Question q = questions[index];
        questionPanel.SetActive(true);
        questionText.enabled = true;

        questionText.text = q.question;

        question = q.question;
        answer = q.correct;
        scoreText.text = "Question " + (index + 1) + "/" + questions.Count;
        missedBalls = 0;
        answerCaught = false;
        totalBallsThisQuestion = q.options.Length;

        StartCoroutine(AnimateQuestionTextThenSpawnBalls(q.question, q.options));
    }


    public void AnimateQuestionTextSimple()
    {
        StartCoroutine(AnimateQuestionTextRoutine());
    }

    IEnumerator AnimateQuestionTextRoutine()
    {
        Canvas.ForceUpdateCanvases();
        questionTextRect.anchoredPosition = new Vector2(questionTextRect.anchoredPosition.x, 600);
        yield return null;
        Sequence seq = DOTween.Sequence();
        seq.Append(questionTextRect.DOAnchorPosY(160f, 1f).SetEase(Ease.OutQuad));
        seq.AppendInterval(3f);
        seq.Append(questionTextRect.DOAnchorPosY(600f, 1f).SetEase(Ease.InQuad));
    }

    IEnumerator AnimateQuestionTextThenSpawnBalls(string question, string[] options)
    {
        questionText.text = question;
        questionPanel.SetActive(true);
        AnimateQuestionTextSimple();
        yield return new WaitForSeconds(6f);
        questionPanel.SetActive(false);
        scoreText.enabled = true;
        yield return new WaitForSeconds(1f);
        scoreText.enabled = false;
        basketController.allowMovement = true;
        StartCoroutine(SpawnAnswerBalls(options));
    }


    IEnumerator SpawnAnswerBalls(string[] options)
    {
        List<string> shuffled = new List<string>(options);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
        }

        for (int i = 0; i < shuffled.Count; i++)
        {
            if (stopSpawning) yield break;

            int newIndex;
            do
            {
                newIndex = Random.Range(0, spawnPoints.Length);
            } while (newIndex == lastUsedSpawnIndex && spawnPoints.Length > 1);

            lastUsedSpawnIndex = newIndex;

            Transform spawnPoint = spawnPoints[newIndex];
            GameObject obj = Instantiate(answerBallPrefab, spawnPoint.position, Quaternion.identity, answerBallParent);
            AnswerBall ball = obj.GetComponent<AnswerBall>();
            ball.Initialize(shuffled[i], this);
            activeBalls.Add(ball);

            yield return new WaitForSeconds(ballSpawnInterval);
        }

        allowFaceInput = true;
    }


    public void OnAnswerBallMissed()
    {
        missedBalls++;
        if (!answerCaught && missedBalls >= totalBallsThisQuestion)
        {
            StartCoroutine(RestartCurrentQuestion());
        }
    }
    public void OnAnswerBallCaught(string answer)
    {
        stopSpawning = true;
        answerCaught = true;
        allowFaceInput = false;
        questionPanel.SetActive(false);
        basketController.allowMovement = false;

        foreach (AnswerBall b in activeBalls)
        {
            if (b != null && !b.hasEnded)
                Destroy(b.gameObject);
        }
        activeBalls.Clear();

        string correct = questions[currentQuestionIndex].correct;

        if (answer == correct)
        {
            StartCoroutine(PlayCorrectEffectAndNext());
        }
        else
        {
            StartCoroutine(ShakeAndRestart());
        }
    }


    IEnumerator RestartCurrentQuestion()
    {
        yield return new WaitForSeconds(1f);

        foreach (AnswerBall b in activeBalls)
        {
            if (b != null)
                Destroy(b.gameObject);
        }
        activeBalls.Clear();

        ShowQuestion(currentQuestionIndex);
    }

    IEnumerator PlayCorrectEffectAndNext()
    {
        if (correctAnswerEffect != null)
            correctAnswerEffect.Play();

        yield return new WaitForSeconds(1f);

        string qText = questions[currentQuestionIndex].question;
        string correctAnswer = questions[currentQuestionIndex].correct;
        resultPanel.SetActive(true);
        resultQuestionText.text = "Q: " + qText;
        resultAnswerText.text = "A: " + correctAnswer;

        yield return new WaitForSeconds(1.5f);
        resultPanel.SetActive(false);
        StartCoroutine(NextQuestion());
    }

    IEnumerator ShakeAndRestart()
    {
        if (shakeTarget != null)
            shakeTarget.DOShakePosition(0.5f, new Vector3(0.5f, 0.5f, 0f), 10, 90f, false, true);

        yield return new WaitForSeconds(1f);

        currentLives--;
        UpdateHeartsDisplay();

        if (currentLives <= 0)
        {
            StartCoroutine(RestartScene());
        }
        else
        {
            Debug.Log("Life lost. Remaining hearts: " + currentLives);
            ShowQuestion(currentQuestionIndex); // Or ShowRandomQuestion()
        }
    }
    void UpdateHeartsDisplay()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < currentLives);
        }
    }

    IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(0.5f);
        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            ShowQuestion(currentQuestionIndex);
        }
        else
        {
            Debug.Log(" All questions complete.");
            StartCoroutine(RestartScene());
        }
    }

    IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetQuestions(QuestionList data)
    {
        loadedQuestions = data;
    }
}
