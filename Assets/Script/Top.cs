using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Top : SelectArrow
{
    public string startScene = "";

    // Use this for initialization

    // Update is called once per frame
    new void Update()
    {
        SelectArrow selectArrow = FindObjectOfType<SelectArrow>();
        if (Input.GetKeyDown(KeyCode.T))
        {
            selectArrow.StopSelect();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            selectArrow.StartSelect();
        }
    }

    public void startButton()
    {
        SceneManager.LoadScene(startScene);
    }

    public void aboutButton()
    {

    }

    public void otherButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
