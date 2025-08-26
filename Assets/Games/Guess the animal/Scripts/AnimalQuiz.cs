using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AnimalQuiz : MonoBehaviour
{
    [System.Serializable]
    public class AnimalEntry
    {
        public string animalName;
        public Sprite animalSprite;
    }

    [Header("Animal Settings")]
    public List<AnimalEntry> animalList = new List<AnimalEntry>();
    private List<AnimalEntry> remainingAnimals = new List<AnimalEntry>();

    [Header("UI Elements")]
    public Image animalImage;
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public GameObject gameCompletedPanel;

    private string currentAnswer = "";

    void Start()
    {
        // Clone the original list to avoid repeating
        remainingAnimals = new List<AnimalEntry>(animalList);

        questionText.text = "What animal is this?";
        gameCompletedPanel.SetActive(false);
        LoadNewAnimal();
    }

    public void LoadNewAnimal()
    {
        feedbackText.text = "";

        if (remainingAnimals.Count == 0)
        {
            ShowGameCompleted();
            return;
        }

        int index = Random.Range(0, remainingAnimals.Count);
        AnimalEntry selected = remainingAnimals[index];
        remainingAnimals.RemoveAt(index);

        currentAnswer = selected.animalName.ToLower();
        animalImage.sprite = selected.animalSprite;
    }

    public void OnSpeechRecognized(string spokenWord)
    {
        feedbackText.text = "You said: " + spokenWord;

        if (spokenWord.ToLower().Contains(currentAnswer))
        {
            feedbackText.text += "\n Correct!";
            StartCoroutine(NextAnimalAfterDelay(1f));
        }
        else
        {
            feedbackText.text += "\n Try again!";
        }
    }

    private IEnumerator NextAnimalAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNewAnimal();
    }

    private void ShowGameCompleted()
    {
        animalImage.enabled = false;
        questionText.text = "";
        feedbackText.text = "";
        gameCompletedPanel.SetActive(true);
        SceneManager.LoadScene("AtHome");
    }
}
