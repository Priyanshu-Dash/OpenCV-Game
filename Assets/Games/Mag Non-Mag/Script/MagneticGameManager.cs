using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MagneticGameManager : MonoBehaviour
{
    [Header("Image Components")]
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    
    [Header("Image Positions")]
    [SerializeField] private Vector3 leftImageInitialPosition;
    [SerializeField] private Vector3 rightImageInitialPosition;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject LosePanel;
    
    [Header("Level System")]
    [SerializeField] private TextMeshProUGUI levelText; // Display current level
    [SerializeField] private TextMeshProUGUI scoreText; // Display current score
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int pointsPerCorrectAnswer = 10;
    [SerializeField] private float levelRestartDelay = 2f; // Delay before restarting level

    [Header("Question System")]
    [SerializeField] private TextMeshProUGUI questionText; // TMP text field for displaying questions
    [SerializeField] private string[] questions = {
        "Which object is magnetic?",
        "Can you identify the magnetic material?",
        "Which item will stick to the magnet?",
        "Find the magnetic object!",
        "Which one is attracted to magnets?"
    };
    [SerializeField] private bool showQuestions = true; // Toggle for question display
    
    [Header("Sprite Arrays")]
    [SerializeField] public Sprite[] magneticSprites;      // Made public for access
    [SerializeField] public Sprite[] nonMagneticSprites;  // Made public for access
    
    private bool isLevelComplete = false;
    private bool isWaitingForRestart = false;
    
    void Start()
    {
        // Check if images are assigned
        if (leftImage == null)
            Debug.LogWarning("Left Image not assigned!");
            
        if (rightImage == null)
            Debug.LogWarning("Right Image not assigned!");
            
        // Store initial positions of images
        StoreInitialPositions();
        
        // Initialize level system
        InitializeLevelSystem();
        
        // Randomly assign sprites to images
        AssignRandomSprites();
    }
    
    void AssignRandomSprites()
    {
        // Randomly decide which side gets magnetic and which gets non-magnetic
        bool leftGetsMagnetic = Random.Range(0, 2) == 0;
        
        if (leftImage != null)
        {
            if (leftGetsMagnetic && magneticSprites.Length > 0)
            {
                int randomIndex = Random.Range(0, magneticSprites.Length);
                leftImage.sprite = magneticSprites[randomIndex];
            }
            else if (!leftGetsMagnetic && nonMagneticSprites.Length > 0)
            {
                int randomIndex = Random.Range(0, nonMagneticSprites.Length);
                leftImage.sprite = nonMagneticSprites[randomIndex];
            }
        }
        
        if (rightImage != null)
        {
            if (!leftGetsMagnetic && magneticSprites.Length > 0)
            {
                int randomIndex = Random.Range(0, magneticSprites.Length);
                rightImage.sprite = magneticSprites[randomIndex];
            }
            else if (leftGetsMagnetic && nonMagneticSprites.Length > 0)
            {
                int randomIndex = Random.Range(0, nonMagneticSprites.Length);
                rightImage.sprite = nonMagneticSprites[randomIndex];
            }
        }
        
        // Display a random question after assigning sprites
        if (showQuestions)
        {
            DisplayRandomQuestion();
        }
    }
    
    // Public method to check if a sprite is magnetic
    public bool IsSpriteMagnetic(Sprite sprite)
    {
        if (sprite == null)
            return false;
            
        // Check if sprite is in magnetic array
        if (magneticSprites != null)
        {
            foreach (var magneticSprite in magneticSprites)
            {
                if (magneticSprite == sprite)
                    return true;
            }
        }
        
        // Check if sprite is in non-magnetic array
        if (nonMagneticSprites != null)
        {
            foreach (var nonMagneticSprite in nonMagneticSprites)
            {
                if (nonMagneticSprite == sprite)
                    return false;
            }
        }
        
        // Fallback: assume non-magnetic if we can't determine
        return false;
    }
    
    // Public method to reshuffle sprites
    public void ReshuffleSprites()
    {
        AssignRandomSprites();
    }
    
    // Display a random question
    public void DisplayRandomQuestion()
    {
        if (questionText != null && showQuestions && questions.Length > 0)
        {
            int randomIndex = Random.Range(0, questions.Length);
            questionText.text = questions[randomIndex];
            questionText.gameObject.SetActive(true);
            Debug.Log("Displaying question: " + questions[randomIndex]);
        }
    }
    
    // Display a specific question by index
    public void DisplayQuestion(int questionIndex)
    {
        if (questionText != null && showQuestions && questionIndex >= 0 && questionIndex < questions.Length)
        {
            questionText.text = questions[questionIndex];
            questionText.gameObject.SetActive(true);
            Debug.Log("Displaying question " + questionIndex + ": " + questions[questionIndex]);
        }
    }
    
    // Hide the question text
    public void HideQuestion()
    {
        if (questionText != null)
        {
            questionText.gameObject.SetActive(false);
            Debug.Log("Question hidden");
        }
    }
    
    // Set custom question text
    public void SetCustomQuestion(string customQuestion)
    {
        if (questionText != null)
        {
            questionText.text = customQuestion;
            questionText.gameObject.SetActive(true);
            Debug.Log("Displaying custom question: " + customQuestion);
        }
    }
    
    // Get current question text
    public string GetCurrentQuestion()
    {
        return questionText != null ? questionText.text : "";
    }
    
    // Check if questions are enabled
    public bool AreQuestionsEnabled()
    {
        return showQuestions;
    }
    
    // Toggle question display
    public void ToggleQuestions()
    {
        showQuestions = !showQuestions;
        if (!showQuestions)
        {
            HideQuestion();
        }
        Debug.Log("Questions " + (showQuestions ? "enabled" : "disabled"));
    }
    
    // Get total number of questions
    public int GetQuestionCount()
    {
        return questions.Length;
    }
    
    // Get all questions as an array
    public string[] GetAllQuestions()
    {
        return questions;
    }
    
    // Add a new question to the array
    public void AddQuestion(string newQuestion)
    {
        if (!string.IsNullOrEmpty(newQuestion))
        {
            System.Array.Resize(ref questions, questions.Length + 1);
            questions[questions.Length - 1] = newQuestion;
            Debug.Log("Added new question: " + newQuestion);
        }
    }
    
    // Clear all questions
    public void ClearQuestions()
    {
        questions = new string[0];
        if (questionText != null)
        {
            questionText.text = "";
            questionText.gameObject.SetActive(false);
        }
        Debug.Log("All questions cleared");
    }
    
    // Handle the result of an answer check
    public void HandleAnswerResult(bool isCorrect, string objectName, Sprite objectSprite)
    {
        if (isCorrect)
        {
            // Correct answer - show success feedback
            ShowSuccessFeedback(objectName);
            
            // Update question to show success
            if (questionText != null)
            {
                WinPanel.SetActive(true);
                //questionText.text = "✅ Correct! " + objectName + " is magnetic!";
                // You can change text color to green here if needed
            }
            
            // Update score and level
            UpdateScore(pointsPerCorrectAnswer);
            CompleteLevel();
            
            Debug.Log("🎉 SUCCESS: " + objectName + " is the correct magnetic object!");
        }
        else
        {
            // Wrong answer - show failure feedback
            ShowFailureFeedback(objectName);
            
            // Update question to show failure
            if (questionText != null)
            {
                LosePanel.SetActive(true);
                //questionText.text = "❌ Wrong! " + objectName + " is not magnetic. Try again!";
                // You can change text color to red here if needed
            }
            
            // For wrong answers, allow retry after a delay
            StartCoroutine(RetryLevelAfterDelay());
            
            Debug.Log("💥 FAILURE: " + objectName + " is not magnetic. Wrong choice!");
        }
        
        // You can add additional game logic here:
        // - Update score
        // - Play sound effects
        // - Show particle effects
        // - Reset for next round
    }
    
    // Show success feedback
    private void ShowSuccessFeedback(string objectName)
    {
        // Add success effects here
        // - Play success sound
        // - Show success particles
        // - Update UI elements
        // - Trigger animations
        
        Debug.Log("Success feedback triggered for: " + objectName);
    }
    
    // Show failure feedback
    private void ShowFailureFeedback(string objectName)
    {
        // Add failure effects here
        // - Play failure sound
        // - Show failure particles
        // - Update UI elements
        // - Trigger animations
        
        Debug.Log("Failure feedback triggered for: " + objectName);
    }
    
    // Get the correct answer (magnetic object)
    public string GetCorrectAnswer()
    {
        // Find which image has the magnetic sprite
        if (leftImage != null && leftImage.sprite != null)
        {
            if (IsSpriteMagnetic(leftImage.sprite))
            {
                return "Left object (" + leftImage.sprite.name + ")";
            }
        }
        
        if (rightImage != null && rightImage.sprite != null)
        {
            if (IsSpriteMagnetic(rightImage.sprite))
            {
                return "Right object (" + rightImage.sprite.name + ")";
            }
        }
        
        return "No magnetic object found";
    }
    
    // Check if a specific object is the correct answer
    public bool IsObjectCorrectAnswer(GameObject targetObject)
    {
        if (targetObject == null) return false;
        
        Sprite objectSprite = null;
        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        Image imageComponent = targetObject.GetComponent<Image>();
        
        if (spriteRenderer != null)
        {
            objectSprite = spriteRenderer.sprite;
        }
        else if (imageComponent != null)
        {
            objectSprite = imageComponent.sprite;
        }
        
        if (objectSprite == null) return false;
        
        return IsSpriteMagnetic(objectSprite);
    }
    
    // Store initial positions of images
    private void StoreInitialPositions()
    {
        if (leftImage != null)
        {
            leftImageInitialPosition = leftImage.transform.position;
            Debug.Log("Left image initial position stored: " + leftImageInitialPosition);
        }
        
        if (rightImage != null)
        {
            rightImageInitialPosition = rightImage.transform.position;
            Debug.Log("Right image initial position stored: " + rightImageInitialPosition);
        }
    }
    
    // Reset images to their initial positions
    private void ResetImagePositions()
    {
        if (leftImage != null)
        {
            leftImage.transform.position = leftImageInitialPosition;
            Debug.Log("Left image position reset to: " + leftImageInitialPosition);
        }
        
        if (rightImage != null)
        {
            rightImage.transform.position = rightImageInitialPosition;
            Debug.Log("Right image position reset to: " + rightImageInitialPosition);
        }
    }
    
    // Reset the game for the next round
    public void ResetForNextRound()
    {
        // Reset images to their initial positions
        ResetImagePositions();
        
        // Reshuffle sprites for a new challenge
        AssignRandomSprites();
        
        // Reset question text to show new question
        if (questionText != null && showQuestions)
        {
            DisplayRandomQuestion();
        }
        
        Debug.Log("Game reset for next round - positions reset, new sprites and question assigned");
    }
    
    // Get current game state information
    public string GetGameStateInfo()
    {
        string leftSpriteName = leftImage != null && leftImage.sprite != null ? leftImage.sprite.name : "None";
        string rightSpriteName = rightImage != null && rightImage.sprite != null ? rightImage.sprite.name : "None";
        string correctAnswer = GetCorrectAnswer();
        
        return $"Left: {leftSpriteName} | Right: {rightSpriteName} | Correct: {correctAnswer}";
    }
    
    // Initialize the level system
    private void InitializeLevelSystem()
    {
        currentLevel = 1;
        currentScore = 0;
        isLevelComplete = false;
        isWaitingForRestart = false;
        
        UpdateLevelUI();
        UpdateScoreUI();
        
        Debug.Log("Level system initialized - Starting at Level " + currentLevel);
    }
    
    // Update the level display
    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level " + currentLevel;
        }
    }
    
    // Update the score display
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }
    
    // Update the score
    public void UpdateScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
        Debug.Log("Score updated: +" + points + " points. Total: " + currentScore);
    }
    
    // Complete the current level
    private void CompleteLevel()
    {
        if (isLevelComplete || isWaitingForRestart) return;
        
        isLevelComplete = true;
        Debug.Log("🎯 LEVEL " + currentLevel + " COMPLETED! Score: " + currentScore);
        
        // Start the restart countdown
        StartCoroutine(RestartLevelAfterDelay());
    }
    
    // Restart the level after a delay
    private IEnumerator RestartLevelAfterDelay()
    {
        isWaitingForRestart = true;
        
        // Wait for the specified delay
        yield return new WaitForSeconds(levelRestartDelay);
        
        // Progress to next level
        ProgressToNextLevel();
        
        isWaitingForRestart = false;
    }
    
    // Retry the same level after a delay (for wrong answers)
    private IEnumerator RetryLevelAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(levelRestartDelay);
        
        // Hide lose panel and retry the same level
        if (LosePanel != null) LosePanel.SetActive(false);
        
        // Reset for retry (same level, same question)
        ResetForNextRound();
        
        Debug.Log("🔄 Retrying Level " + currentLevel);
    }
    
    // Progress to the next level
    private void ProgressToNextLevel()
    {
        currentLevel++;
        isLevelComplete = false;
        
        // Hide win/lose panels
        if (WinPanel != null) WinPanel.SetActive(false);
        if (LosePanel != null) LosePanel.SetActive(false);
        
        // Update UI
        UpdateLevelUI();
        
        // Reset for new level
        ResetForNextRound();
        
        Debug.Log("🚀 Progressing to Level " + currentLevel);
    }
    
    // Get current level
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    // Get current score
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    // Reset the entire game (start over from level 1)
    public void ResetGame()
    {
        currentLevel = 1;
        currentScore = 0;
        isLevelComplete = false;
        isWaitingForRestart = false;
        
        // Hide panels
        if (WinPanel != null) WinPanel.SetActive(false);
        if (LosePanel != null) LosePanel.SetActive(false);
        
        // Update UI
        UpdateLevelUI();
        UpdateScoreUI();
        
        // Reset for new game
        ResetForNextRound();
        
        Debug.Log("🔄 Game reset - Starting over from Level 1");
    }
    
    // Skip to a specific level (for testing)
    public void SkipToLevel(int targetLevel)
    {
        if (targetLevel < 1) return;
        
        currentLevel = targetLevel;
        currentScore = (targetLevel - 1) * pointsPerCorrectAnswer;
        
        UpdateLevelUI();
        UpdateScoreUI();
        
        ResetForNextRound();
        
        Debug.Log("⏭️ Skipped to Level " + targetLevel + " with Score " + currentScore);
    }
}
