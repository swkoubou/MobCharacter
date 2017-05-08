using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AjustAspect : MonoBehaviour
{
    public float baseAspect = 1200f / 800f;

    void Awake()
    {
        Camera cam = gameObject.GetComponent<Camera>();
        float nowAspect = (float)Screen.height / (float)Screen.width;
        float changeAspect;

        if (baseAspect > nowAspect)
        {
            changeAspect = nowAspect / baseAspect;
            cam.rect = new Rect((1 - changeAspect) * 0.5f, 0, changeAspect, 1);
        }
        else
        {
            changeAspect = baseAspect / nowAspect;
            cam.rect = new Rect(0, (1 - changeAspect) * 0.5f, 1, changeAspect);
        }
        Destroy(this);
    }


    void Start()
    {

    }


    void Update()
    {

    }
}
