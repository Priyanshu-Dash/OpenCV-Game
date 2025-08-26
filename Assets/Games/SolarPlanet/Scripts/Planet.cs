using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public SpriteRenderer selectionSprite;
    public Collider2D col;


    [HideInInspector] public bool isPlanetAtCorrectLocation;

    private Vector3 planetOrignalPostion;
    private bool IsPlanetSelect;
    
    private bool isPlanetSet=true;
    private GameObject correctLocationObject;
    private PlanetManager planetManager;
    private Transform handPoint;
    private Vector3 orignalSize;
    private string placedPlanet;
    public void InIt(PlanetManager planetManager)
    {
        this.planetManager = planetManager;
    }

    private void OnEnable()
    {
        planetManager.followHand.ClickEnter += ClickEnter;
        planetManager.followHand.ClickExit += ClickExit;
    }
    private void Start()
    {
        planetOrignalPostion = transform.position;
        orignalSize = transform.localScale;
    }
    private void ClickEnter(Vector3 postion,Transform selectedObject)
    {
        //if (isPlanetSet)
        //{
        //    return;
        //}
        
        GameObject hitObject = PerformRaycast(selectedObject.position);
        if (hitObject!=null && hitObject.Equals(gameObject))
        {
            Debug.Log("Obejct Equal:" + gameObject.name);
            handPoint = selectedObject;
            IsPlanetSelect = true;
            transform.localScale = orignalSize * 1.3f;
            selectionSprite.color = Color.black;
            //isPlanetSlected = true;
           
            isPlanetSet = false;
            if (correctLocationObject != null)
            {
                correctLocationObject.SetActive(true);
            }
            correctLocationObject = null;
            // col.enabled = true;
        }
        Debug.Log("Obejct select" + selectedObject.name);
    }
    private void ClickExit(Vector3 postion, Transform selectedObject)
    {
        //if (isPlanetSet)
        //{
        //    return;
        //}
        isPlanetSet = true;
        if (IsPlanetSelect)
        {


            if ( correctLocationObject != null)
            {
                transform.position = correctLocationObject.transform.position;
                //this.enabled = false;
                //correctLocationObject.SetActive(false);
                selectionSprite.gameObject.SetActive(true);
                transform.localScale = orignalSize;

                if (isPlanetAtCorrectLocation)
                {
                    selectionSprite.color = Color.green;
                }
                else
                {
                    selectionSprite.color = Color.red;
                }
                if (placedPlanet != "")
                {
                    planetManager.placeHolderList.Remove(placedPlanet);
                }
                placedPlanet = correctLocationObject.name;
                planetManager.placeHolderList.Add(correctLocationObject.name);
                planetManager.CheckTutorial();
                planetManager.PlanetPlacedAtLocation(this);
                correctLocationObject.SetActive(false);
            }
            else
            {
                transform.position = planetOrignalPostion;
                
            }
        }
        handPoint = null;
        IsPlanetSelect = false;
      
    }
  
    void Update()
    {
        //if (isPlanetSet)
        //{
        //    return;
        //}
        if (IsPlanetSelect)
        {
            transform.position = Vector3.Lerp(transform.position, handPoint.position, Time.deltaTime * 20);

        }
    }
    GameObject PerformRaycast(Vector2 screenPosition)
    {
        Camera cam = Camera.main;

        // --- 2D Raycast ---
        //Vector2 worldPoint2D = cam.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit2D = Physics2D.Raycast(screenPosition, Vector2.zero,planetManager.planetLayer);

        if (hit2D.collider != null)
        {
            GameObject hitObject2D = hit2D.collider.gameObject;
            Debug.Log(">> Hit 2D Object: " + hitObject2D.name);
            Debug.Log("2D Object Position: " + hitObject2D.transform.position);
            return hitObject2D; // 2D hit found, stop here
        }

        // --- 3D Raycast ---
        Ray ray3D = cam.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray3D, out RaycastHit hit3D))
        {
            GameObject hitObject3D = hit3D.collider.gameObject;
            Debug.Log(">> Hit 3D Object: " + hitObject3D.name);
            Debug.Log("3D Object Position: " + hitObject3D.transform.position);
        }
        else
        {
            Debug.Log("No 2D or 3D object hit!");
        }
        return null;
    }

    private void OnDisable()
    {
        planetManager.followHand.ClickEnter -= ClickEnter;
        planetManager.followHand.ClickExit -= ClickExit;
    }
   
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isPlanetSet)
        {
            return;
        }
        if (planetManager.placeHolderList.Contains(collision.name))
        {
            return;
        }
        selectionSprite.gameObject.SetActive(true);
        selectionSprite.color = Color.white;
        
        correctLocationObject = collision.gameObject;
        if (collision.name == gameObject.name)
        {
            //selectionSprite.color = Color.green;

            isPlanetAtCorrectLocation = true;
        }
        else
        {
           // selectionSprite.color = Color.red;
            isPlanetAtCorrectLocation = false;

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isPlanetSet)
        {
            return;
        }
        selectionSprite.gameObject.SetActive(false);
        //isPlanetAtCorrectLocation = false;
        correctLocationObject = null;
    }
}
