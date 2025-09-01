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

    [SerializeField] private GameObject FinalWinScreen;
    [SerializeField] private GameObject FinalLoseScreen;
    
    [Header("Level System")]
    [SerializeField] private TextMeshProUGUI scoreText; // Display current score
    [SerializeField] private TextMeshProUGUI healthText; // Display current health
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int currentHealth = 3; // Start with 3 lives
    [SerializeField] private int maxHealth = 3; // Maximum health
    [SerializeField] private int pointsPerCorrectAnswer = 10;
    [SerializeField] private float levelRestartDelay = 2f; // Delay before restarting level
    [SerializeField] private int correctAnswersToWin = 10; // Number of correct answers needed to win

    [Header("Question System")]
    [SerializeField] private TextMeshProUGUI questionText; // TMP text field for displaying questions
    [SerializeField] private string[] magneticQuestions = {
        "Which object is magnetic?",
        "Can you identify the magnetic material?",
        "Which item will stick to the magnet?",
        "Find the magnetic object!",
        "Which one is attracted to magnets?"
    };
    [SerializeField] private string[] nonMagneticQuestions = {
        "Which object is NOT magnetic?",
        "Can you identify the non-magnetic material?",
        "Which item will NOT stick to the magnet?",
        "Find the non-magnetic object!",
        "Which one is NOT attracted to magnets?",
        "Which object will repel from magnets?",
        "Identify the material that won't stick to magnets"
    };
    [SerializeField] private bool showQuestions = true; // Toggle for question display
    
    // Track current question type for answer checking
    private bool currentQuestionAsksAboutMagnetic = true; // true = magnetic question, false = non-magnetic question
    
    [Header("Sprite Arrays")]
    [SerializeField] public Sprite[] magneticSprites;      // Made public for access
    [SerializeField] public Sprite[] nonMagneticSprites;  // Made public for access
    
    private bool isLevelComplete = false;
    private bool isWaitingForRestart = false;
    private int totalCorrectAnswers = 0; // Track total correct answers for win condition
    
    void Start()
    {
        FinalWinScreen.SetActive(false);
        FinalLoseScreen.SetActive(false);
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
        if (questionText != null && showQuestions)
        {
            // Randomly decide whether to ask about magnetic or non-magnetic objects
            bool askAboutMagnetic = Random.Range(0, 2) == 0;
            
            if (askAboutMagnetic && magneticQuestions.Length > 0)
            {
                int randomIndex = Random.Range(0, magneticQuestions.Length);
                questionText.text = magneticQuestions[randomIndex];
                questionText.gameObject.SetActive(true);
                currentQuestionAsksAboutMagnetic = true; // Track question type
                Debug.Log("Displaying magnetic question: " + magneticQuestions[randomIndex]);
            }
            else if (!askAboutMagnetic && nonMagneticQuestions.Length > 0)
            {
                int randomIndex = Random.Range(0, nonMagneticQuestions.Length);
                questionText.text = nonMagneticQuestions[randomIndex];
                questionText.gameObject.SetActive(true);
                currentQuestionAsksAboutMagnetic = false; // Track question type
                Debug.Log("Displaying non-magnetic question: " + nonMagneticQuestions[randomIndex]);
            }
        }
    }
    
    // Display a random magnetic question
    public void DisplayRandomMagneticQuestion()
    {
        if (questionText != null && showQuestions && magneticQuestions.Length > 0)
        {
            int randomIndex = Random.Range(0, magneticQuestions.Length);
            questionText.text = magneticQuestions[randomIndex];
            questionText.gameObject.SetActive(true);
            currentQuestionAsksAboutMagnetic = true; // Track question type
            Debug.Log("Displaying random magnetic question: " + magneticQuestions[randomIndex]);
        }
    }
    
    // Display a random non-magnetic question
    public void DisplayRandomNonMagneticQuestion()
    {
        if (questionText != null && showQuestions && nonMagneticQuestions.Length > 0)
        {
            int randomIndex = Random.Range(0, nonMagneticQuestions.Length);
            questionText.text = nonMagneticQuestions[randomIndex];
            questionText.gameObject.SetActive(true);
            currentQuestionAsksAboutMagnetic = false; // Track question type
            Debug.Log("Displaying random non-magnetic question: " + nonMagneticQuestions[randomIndex]);
        }
    }
    
    // Display a specific question by index (0 = magnetic, 1 = non-magnetic)
    public void DisplayQuestion(int questionType, int questionIndex)
    {
        if (questionText != null && showQuestions)
        {
            if (questionType == 0 && questionIndex >= 0 && questionIndex < magneticQuestions.Length)
            {
                questionText.text = magneticQuestions[questionIndex];
                questionText.gameObject.SetActive(true);
                currentQuestionAsksAboutMagnetic = true; // Track question type
                Debug.Log("Displaying magnetic question " + questionIndex + ": " + magneticQuestions[questionIndex]);
            }
            else if (questionType == 1 && questionIndex >= 0 && questionIndex < nonMagneticQuestions.Length)
            {
                questionText.text = nonMagneticQuestions[questionIndex];
                questionText.gameObject.SetActive(true);
                currentQuestionAsksAboutMagnetic = false; // Track question type
                Debug.Log("Displaying non-magnetic question " + questionIndex + ": " + nonMagneticQuestions[questionIndex]);
            }
        }
    }
    
    // Display a specific magnetic question by index
    public void DisplayMagneticQuestion(int questionIndex)
    {
        if (questionText != null && showQuestions && questionIndex >= 0 && questionIndex < magneticQuestions.Length)
        {
            questionText.text = magneticQuestions[questionIndex];
            questionText.gameObject.SetActive(true);
            currentQuestionAsksAboutMagnetic = true; // Track question type
            Debug.Log("Displaying magnetic question " + questionIndex + ": " + magneticQuestions[questionIndex]);
        }
    }
    
    // Display a specific non-magnetic question by index
    public void DisplayNonMagneticQuestion(int questionIndex)
    {
        if (questionText != null && showQuestions && questionIndex >= 0 && questionIndex < nonMagneticQuestions.Length)
        {
            questionText.text = nonMagneticQuestions[questionIndex];
            questionText.gameObject.SetActive(true);
            currentQuestionAsksAboutMagnetic = false; // Track question type
            Debug.Log("Displaying non-magnetic question " + questionIndex + ": " + nonMagneticQuestions[questionIndex]);
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
        return magneticQuestions.Length + nonMagneticQuestions.Length;
    }
    
    // Get total number of magnetic questions
    public int GetMagneticQuestionCount()
    {
        return magneticQuestions.Length;
    }
    
    // Get total number of non-magnetic questions
    public int GetNonMagneticQuestionCount()
    {
        return nonMagneticQuestions.Length;
    }
    
    // Get all questions as an array
    public string[] GetAllQuestions()
    {
        string[] allQuestions = new string[magneticQuestions.Length + nonMagneticQuestions.Length];
        magneticQuestions.CopyTo(allQuestions, 0);
        nonMagneticQuestions.CopyTo(allQuestions, magneticQuestions.Length);
        return allQuestions;
    }
    
    // Get all magnetic questions
    public string[] GetMagneticQuestions()
    {
        return magneticQuestions;
    }
    
    // Get all non-magnetic questions
    public string[] GetNonMagneticQuestions()
    {
        return nonMagneticQuestions;
    }
    
    // Add a new magnetic question to the array
    public void AddMagneticQuestion(string newQuestion)
    {
        if (!string.IsNullOrEmpty(newQuestion))
        {
            System.Array.Resize(ref magneticQuestions, magneticQuestions.Length + 1);
            magneticQuestions[magneticQuestions.Length - 1] = newQuestion;
            Debug.Log("Added new magnetic question: " + newQuestion);
        }
    }
    
    // Add a new non-magnetic question to the array
    public void AddNonMagneticQuestion(string newQuestion)
    {
        if (!string.IsNullOrEmpty(newQuestion))
        {
            System.Array.Resize(ref nonMagneticQuestions, nonMagneticQuestions.Length + 1);
            nonMagneticQuestions[nonMagneticQuestions.Length - 1] = newQuestion;
            Debug.Log("Added new non-magnetic question: " + newQuestion);
        }
    }
    
    // Add a new question to the appropriate array (auto-detect type)
    public void AddQuestion(string newQuestion)
    {
        if (!string.IsNullOrEmpty(newQuestion))
        {
            // Auto-detect if it's a magnetic or non-magnetic question
            string lowerQuestion = newQuestion.ToLower();
            if (lowerQuestion.Contains("not") || lowerQuestion.Contains("non") || lowerQuestion.Contains("won't") || lowerQuestion.Contains("repel"))
            {
                AddNonMagneticQuestion(newQuestion);
            }
            else
            {
                AddMagneticQuestion(newQuestion);
            }
        }
    }
    
    // Clear all questions
    public void ClearQuestions()
    {
        magneticQuestions = new string[0];
        nonMagneticQuestions = new string[0];
        if (questionText != null)
        {
            questionText.text = "";
            questionText.gameObject.SetActive(false);
        }
        Debug.Log("All questions cleared");
    }
    
    // Clear only magnetic questions
    public void ClearMagneticQuestions()
    {
        magneticQuestions = new string[0];
        Debug.Log("Magnetic questions cleared");
    }
    
    // Clear only non-magnetic questions
    public void ClearNonMagneticQuestions()
    {
        nonMagneticQuestions = new string[0];
        Debug.Log("Non-magnetic questions cleared");
    }
    
    // Handle the result of an answer check
    public void HandleAnswerResult(bool isCorrect, string objectName, Sprite objectSprite)
    {
        if (isCorrect)
        {
            // Correct answer - show success feedback
            ShowSuccessFeedback(objectName);
            
            // Increment total correct answers
            totalCorrectAnswers++;
            Debug.Log("Correct answers: " + totalCorrectAnswers + "/" + correctAnswersToWin);
            
            // Check for win condition
            if (totalCorrectAnswers >= correctAnswersToWin)
            {
                GameWin();
                return; // Exit early, don't continue with normal level completion
            }
            
            // Update question to show success
            if (questionText != null)
            {
                WinPanel.SetActive(true);
                //questionText.text = "✅ Correct! " + objectName + " is the right choice!";
                // You can change text color to green here if needed
            }
            
            // Update score and level
            UpdateScore(pointsPerCorrectAnswer);
            CompleteLevel();
            
            Debug.Log("🎉 SUCCESS: " + objectName + " is the correct answer!");
        }
        else
        {
            Debug.Log("HandleAnswerResult: Wrong answer detected for " + objectName);
            
            // Wrong answer - show failure feedback
            ShowFailureFeedback(objectName);
            
            // Decrease health for wrong answer
            Debug.Log("HandleAnswerResult: Calling DecreaseHealth() for wrong answer");
            DecreaseHealth();
            
            // Update question to show failure
            if (questionText != null)
            {
                LosePanel.SetActive(true);
                //questionText.text = "❌ Wrong! " + objectName + " is not the right choice. Try again!";
                // You can change text color to red here if needed
            }
            
            // For wrong answers, allow retry after a delay
            StartCoroutine(RetryLevelAfterDelay());
            
            Debug.Log("💥 FAILURE: " + objectName + " is not the right choice! Health: " + currentHealth);
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
    
    // Get the correct answer based on current question type
    public string GetCorrectAnswer()
    {
        if (currentQuestionAsksAboutMagnetic)
        {
            // Question asks for magnetic objects
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
        else
        {
            // Question asks for non-magnetic objects
            if (leftImage != null && leftImage.sprite != null)
            {
                if (!IsSpriteMagnetic(leftImage.sprite))
                {
                    return "Left object (" + leftImage.sprite.name + ")";
                }
            }
            
            if (rightImage != null && rightImage.sprite != null)
            {
                if (!IsSpriteMagnetic(rightImage.sprite))
                {
                    return "Right object (" + rightImage.sprite.name + ")";
                }
            }
            
            return "No non-magnetic object found";
        }
    }
    
    // Get the correct answer for magnetic questions (magnetic object)
    public string GetCorrectAnswerForMagnetic()
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
    
    // Get the correct answer for non-magnetic questions (non-magnetic object)
    public string GetCorrectAnswerForNonMagnetic()
    {
        // Find which image has the non-magnetic sprite
        if (leftImage != null && leftImage.sprite != null)
        {
            if (!IsSpriteMagnetic(leftImage.sprite))
            {
                return "Left object (" + leftImage.sprite.name + ")";
            }
        }
        
        if (rightImage != null && rightImage.sprite != null)
        {
            if (!IsSpriteMagnetic(rightImage.sprite))
            {
                return "Right object (" + rightImage.sprite.name + ")";
            }
        }
        
        return "No non-magnetic object found";
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
        
        bool isObjectMagnetic = IsSpriteMagnetic(objectSprite);
        
        // For magnetic questions: correct answer is magnetic objects
        // For non-magnetic questions: correct answer is non-magnetic objects
        if (currentQuestionAsksAboutMagnetic)
        {
            return isObjectMagnetic; // Question asks for magnetic objects
        }
        else
        {
            return !isObjectMagnetic; // Question asks for non-magnetic objects
        }
    }
    
    // Check if a specific object is the correct answer for magnetic questions
    public bool IsObjectCorrectAnswerForMagnetic(GameObject targetObject)
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
    
    // Check if a specific object is the correct answer for non-magnetic questions
    public bool IsObjectCorrectAnswerForNonMagnetic(GameObject targetObject)
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
        
        return !IsSpriteMagnetic(objectSprite);
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
        currentHealth = maxHealth; // Reset health to max
        totalCorrectAnswers = 0; // Reset win condition
        isLevelComplete = false;
        isWaitingForRestart = false;
        
        UpdateScoreUI();
        UpdateHealthUI(); // Initialize health UI
        
        Debug.Log("Level system initialized - Starting at Level " + currentLevel);
    }
    
    // Update the score display
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }

    // Update the health display
    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth + "/" + maxHealth;
        }
    }
    
    // Update the score
    public void UpdateScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
        Debug.Log("Score updated: +" + points + " points. Total: " + currentScore);
    }

    // Decrease health
    public void DecreaseHealth()
    {
        Debug.Log("DecreaseHealth() called. Current health before: " + currentHealth);
        
        if (currentHealth > 0)
        {
            currentHealth--;
            UpdateHealthUI();
            Debug.Log("Health decreased. Current health after: " + currentHealth);
            
            // Check if game is over
            if (currentHealth <= 0)
            {
                Debug.Log("Health reached 0, calling GameOver()");
                GameOver();
            }
        }
        else
        {
            Debug.Log("Cannot decrease health - already at 0");
        }
    }
    
    // Handle game over
    private void GameOver()
    {
        FinalLoseScreen.SetActive(true);
        Debug.Log("💀 GAME OVER! No more lives remaining.");
        
        // // Show game over panel if available
        // if (LosePanel != null)
        // {
        //     LosePanel.SetActive(true);
        // }
        
        // Update question text to show game over
        if (questionText != null)
        {
            questionText.text = "💀 Game Over! No more lives!";
        }
        
        // Stop the game - no more levels can be played
        isLevelComplete = true;
        isWaitingForRestart = true;
    }
    
    // Handle game win
    private void GameWin()
    {
        FinalWinScreen.SetActive(true);
        Debug.Log("🏆 GAME WIN! You answered " + totalCorrectAnswers + " questions correctly!");
        
        // Show win panel if available
        // if (WinPanel != null)
        // {
        //     WinPanel.SetActive(true);
        // }
        
        // Update question text to show victory
        if (questionText != null)
        {
            questionText.text = "🏆 Congratulations! You won the game!";
        }
        
        // Stop the game - player has won
        isLevelComplete = true;
        isWaitingForRestart = true;
        
        // You can add additional win effects here:
        // - Play victory sound
        // - Show victory particles
        // - Display final score
        // - Show restart button
    }
    
    // Check if game is over
    public bool IsGameOver()
    {
        return currentHealth <= 0;
    }
    
    // Check if game is won
    public bool IsGameWon()
    {
        return totalCorrectAnswers >= correctAnswersToWin;
    }
    
    // Restore health (for power-ups or special events)
    public void RestoreHealth(int amount = 1)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            UpdateHealthUI();
            Debug.Log("Health restored by " + amount + ". Current health: " + currentHealth);
        }
    }
    
    // Complete the current level
    private void CompleteLevel()
    {
        if (isLevelComplete || isWaitingForRestart || IsGameOver()) return;
        
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

    // Get current health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    // Get total correct answers
    public int GetTotalCorrectAnswers()
    {
        return totalCorrectAnswers;
    }
    
    // Get correct answers needed to win
    public int GetCorrectAnswersToWin()
    {
        return correctAnswersToWin;
    }
    
    // Get win progress as percentage
    public float GetWinProgress()
    {
        return (float)totalCorrectAnswers / correctAnswersToWin;
    }
    
    // Reset the entire game (start over from level 1)
    public void ResetGame()
    {
        currentLevel = 1;
        currentScore = 0;
        currentHealth = maxHealth; // Reset health to max
        totalCorrectAnswers = 0; // Reset win condition
        isLevelComplete = false;
        isWaitingForRestart = false;
        
        // Hide panels
        if (WinPanel != null) WinPanel.SetActive(false);
        if (LosePanel != null) LosePanel.SetActive(false);
        
        // Update UI
        UpdateScoreUI();
        UpdateHealthUI(); // Reset health UI
        
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
        currentHealth = maxHealth; // Reset health to max
        
        UpdateScoreUI();
        UpdateHealthUI(); // Reset health UI
        
        ResetForNextRound();
        
        Debug.Log("⏭️ Skipped to Level " + targetLevel + " with Score " + currentLevel);
    }
    
    // Restart game after game over
    public void RestartGameAfterGameOver()
    {
        if (IsGameOver())
        {
            Debug.Log("🔄 Restarting game after game over...");
            ResetGame();
        }
    }
    
    // Get maximum health
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    // Check if player has full health
    public bool HasFullHealth()
    {
        return currentHealth >= maxHealth;
    }
    
    // Test method to manually decrease health (for debugging)
    public void TestDecreaseHealth()
    {
        Debug.Log("TestDecreaseHealth() called manually");
        DecreaseHealth();
    }
    
    // Test method to manually increase health (for debugging)
    public void TestIncreaseHealth()
    {
        Debug.Log("TestIncreaseHealth() called manually");
        RestoreHealth(1);
    }
    
    // Test method to manually trigger win (for debugging)
    public void TestWin()
    {
        Debug.Log("TestWin() called manually");
        totalCorrectAnswers = correctAnswersToWin;
        GameWin();
    }
    
    // Get current question type
    public bool IsCurrentQuestionAboutMagnetic()
    {
        return currentQuestionAsksAboutMagnetic;
    }
    
    // Get current question type as string
    public string GetCurrentQuestionType()
    {
        return currentQuestionAsksAboutMagnetic ? "Magnetic" : "Non-Magnetic";
    }
    
    // Debug current question state
    public void DebugCurrentQuestionState()
    {
        Debug.Log("Current Question Type: " + GetCurrentQuestionType());
        Debug.Log("Current Question Text: " + GetCurrentQuestion());
        Debug.Log("Correct Answer: " + GetCorrectAnswer());
    }
}
