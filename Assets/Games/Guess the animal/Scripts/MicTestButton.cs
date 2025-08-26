using UnityEngine;

public class MicTestButton : MonoBehaviour
{
    public string fakeInput;
    public AnimalQuiz animalQuiz;

    // Simulate speech input for testing
    public void SimulateSpeech()
    {
        Debug.Log("Simulated Speech: " + fakeInput);

        animalQuiz.OnSpeechRecognized(fakeInput);
    }
}
