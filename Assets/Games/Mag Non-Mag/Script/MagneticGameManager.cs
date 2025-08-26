using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add TextMeshPro support

public class MagneticGameManager : MonoBehaviour
{
    [Header("Image Components")]
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    

    [Header("UI Elements")]
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject LosePanel;


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
    
    void Start()
    {
        // Check if images are assigned
        if (leftImage == null)
            Debug.LogWarning("Left Image not assigned!");
            
        if (rightImage == null)
            Debug.LogWarning("Right Image not assigned!");
            
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
    
    // Reset the game for the next round
    public void ResetForNextRound()
    {
        // Reshuffle sprites for a new challenge
        ReshuffleSprites();
        
        // Reset question text to show new question
        if (questionText != null && showQuestions)
        {
            DisplayRandomQuestion();
        }
        
        Debug.Log("Game reset for next round - new sprites and question assigned");
    }
    
    // Get current game state information
    public string GetGameStateInfo()
    {
        string leftSpriteName = leftImage != null && leftImage.sprite != null ? leftImage.sprite.name : "None";
        string rightSpriteName = rightImage != null && rightImage.sprite != null ? rightImage.sprite.name : "None";
        string correctAnswer = GetCorrectAnswer();
        
        return $"Left: {leftSpriteName} | Right: {rightSpriteName} | Correct: {correctAnswer}";
    }
}
