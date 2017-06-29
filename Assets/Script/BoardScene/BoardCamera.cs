using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCamera : MonoBehaviour
{

    private Transform target;
    public Vector3 offset;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = target.transform.position + offset;
    }


    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position+offset, 10 * Time.deltaTime);
    }
}
