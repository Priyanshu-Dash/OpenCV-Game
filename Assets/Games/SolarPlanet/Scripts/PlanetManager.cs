using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetManager : MonoBehaviour
{
    public FollowHand followHand;
    public Planet[] planets;
    public LayerMask planetLayer;
    public GameObject TutorialObject;
    public GameObject gameCompleteScreen;
    public List<string> placeHolderList = new List<string>();
    int allPlanetAtLocationCount;

    // Check How Many Planet Place at right Location
    int correctLocationScore;

    void Awake()
    {
        for(int i = 0; i < planets.Length; i++)
        {
            planets[i].InIt(this);
        }
    }

    void Update()
    {
        
    }
    public void CheckTutorial()
    {
        if (TutorialObject.activeSelf)
        {
            TutorialObject.SetActive(false);
        }
    }

    //This function call Every time, when planet place at any location
    public void PlanetPlacedAtLocation(Planet currentPlanet)
    {
        allPlanetAtLocationCount++;
        if (currentPlanet.isPlanetAtCorrectLocation)
        {
            correctLocationScore++;
        }
        else
        {
            if(SceneManager.GetActiveScene().name== "InClassGames")
            {
                StartCoroutine(LoadScene("InClassScene"));
            }
        }
        if (allPlanetAtLocationCount == 8)
        {
            if (SceneManager.GetActiveScene().name == "gamemaker")
            {
                StartCoroutine(LoadScene("AtHome"));
            }
            else if (SceneManager.GetActiveScene().name == "InClassGames")
            {
                StartCoroutine(LoadScene("InClassScene"));
            }
        }
        
    }

    private IEnumerator LoadScene(string sceneName)
    {
        gameCompleteScreen.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(sceneName);

    }
    
}
