using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalRegionQuizManager : MonoBehaviour
{
    [System.Serializable]
    public class AnimalData
    {
        public string name;
        public Sprite sprite;
        public string region; // e.g., "Forest", "Ocean"
    }

    [System.Serializable]
    public class RegionData
    {
        public string regionName;
        public Sprite regionSprite;
    }

    [Header("UI References")]
    public Image regionImage;
    public TextMeshProUGUI questionText;
    public Image leftAnimalImage;
    public Image rightAnimalImage;
    public Button leftAnimalButton;
    public Button rightAnimalButton;

    [Header("Feedback UI")]
    public Image resultImage;           // UI image to show yay/oops
    public Sprite yaySprite;
    public Sprite oopsSprite;
    public AudioClip yayAudio;
    public AudioClip oopsAudio;
    public AudioSource audioSource;

    [Header("Data")]
    public List<AnimalData> animalList;
    public List<RegionData> regionList;

    private string currentCorrectRegion;
    private AnimalData correctAnimal;
    private bool inputLocked = false;

    void Start()
    {
        // Auto-assign AudioSource if not manually set
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Ensure 2D sound
        audioSource.spatialBlend = 0f;

        resultImage.gameObject.SetActive(false);

        // Attach button listeners
        leftAnimalButton.onClick.AddListener(() => OnAnimalSelected(leftAnimalImage.sprite));
        rightAnimalButton.onClick.AddListener(() => OnAnimalSelected(rightAnimalImage.sprite));

        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        inputLocked = false;
        resultImage.gameObject.SetActive(false);

        // Pick a random region
        RegionData selectedRegion = regionList[Random.Range(0, regionList.Count)];
        currentCorrectRegion = selectedRegion.regionName;

        regionImage.sprite = selectedRegion.regionSprite;
        questionText.text = $"Select the animal belongs to the {currentCorrectRegion} region?";

        // Get matching and non-matching animals
        List<AnimalData> matching = animalList.FindAll(a => a.region == currentCorrectRegion);
        List<AnimalData> nonMatching = animalList.FindAll(a => a.region != currentCorrectRegion);

        if (matching.Count == 0 || nonMatching.Count == 0)
        {
            Debug.LogError("Insufficient animal data for selected region.");
            return;
        }

        correctAnimal = matching[Random.Range(0, matching.Count)];
        AnimalData wrongAnimal = nonMatching[Random.Range(0, nonMatching.Count)];

        bool correctOnLeft = Random.value > 0.5f;

        if (correctOnLeft)
        {
            leftAnimalImage.sprite = correctAnimal.sprite;
            rightAnimalImage.sprite = wrongAnimal.sprite;
        }
        else
        {
            leftAnimalImage.sprite = wrongAnimal.sprite;
            rightAnimalImage.sprite = correctAnimal.sprite;
        }
    }

    void OnAnimalSelected(Sprite selectedSprite)
    {
        if (inputLocked) return;
        inputLocked = true;

        bool isCorrect = (selectedSprite == correctAnimal.sprite);

        if (isCorrect)
        {
            resultImage.sprite = yaySprite;
            audioSource.clip = yayAudio;
        }
        else
        {
            resultImage.sprite = oopsSprite;
            audioSource.clip = oopsAudio;
        }

        resultImage.gameObject.SetActive(true);
        audioSource.Play();

        StartCoroutine(NextQuestionAfterDelay(2f));
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GenerateQuestion();
    }
    public void SelectLeftOptionFromGesture()
    {
        if (inputLocked) return;
        OnAnimalSelected(leftAnimalImage.sprite);
    }

    public void SelectRightOptionFromGesture()
    {
        if (inputLocked) return;
        OnAnimalSelected(rightAnimalImage.sprite);
    }
}
