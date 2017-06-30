using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Top : MonoBehaviour
{
    public AudioClip decide;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (FadeSceneManager.IsFading())
            FindObjectOfType<TopArrow>().enabled = false;
    }

    public void startButton()
    {
        FadeSceneManager.Execute(Loader.boardSceneName);
        GameObject.Find("SE").GetComponent<AudioSource>().PlayOneShot(decide, 1f);
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
