using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credit : MonoBehaviour
{
    Vector2 startPoss;
    public float speed = 0f;
    public float distance = 0f;
    public string Scene = "";
    public float WaitSecond = 0f;
    // Use this for initialization
    void Start()
    {
        startPoss = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y - startPoss.y < distance)
        {
            transform.Translate(new Vector2(0f, speed * Time.deltaTime));
        }
        else
        {
            Invoke("SceneJump", WaitSecond);
        }
    }
    void SceneJump()
    {
        SceneManager.LoadScene(Scene);
    }
}
