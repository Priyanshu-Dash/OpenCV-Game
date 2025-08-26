using UnityEngine;

public class SwitchingModes : MonoBehaviour
{
    public GameObject todo;
    public GameObject prac;
    public GameObject line1;
    public GameObject line2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        todo.SetActive(false);
        prac.SetActive(true);
        line1.SetActive(false);
        line2.SetActive(true);
    }

    public void OpenTodo()
    {
        todo.SetActive(true);
        prac.SetActive(false);
        line1.SetActive(true);
        line2.SetActive(false);
    }
    public void OpenPrac()
    {
        todo.SetActive(false);
        prac.SetActive(true);
        line1.SetActive(false);
        line2.SetActive(true);
    }
}
