using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private MapGenerator mapGenerator;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
        mapGenerator.SetupScene(100);
    }


    void Update()
    {

    }
}
