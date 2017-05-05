using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransObject : MonoBehaviour
{
    public static GameObject moveObj;
    public static Vector3 startPos;
    public static Vector3 endPos;
    public static float moveTime;
    public static float startTime;
    public static float journeyLength;
    public static bool isMoved;


    public static void MoveTo(GameObject obj, Vector3 pos, float time = 0)
    {
        moveObj = obj;
        startPos = obj.transform.position;
        endPos = new Vector3(obj.transform.position.x + pos.x, obj.transform.position.y + pos.y, obj.transform.position.z + pos.z);
        moveTime = time;
        startTime = Time.timeSinceLevelLoad;
        journeyLength = Vector3.Distance(startPos, endPos);
        isMoved = false;

        if(!moveObj.GetComponent<TransObject>())
            moveObj.AddComponent<TransObject>();
    }


    void Update()
    {
        float diff = Time.timeSinceLevelLoad - startTime;
        //float distCovered = (Time.timeSinceLevelLoad - startTime) * moveTime;
        //float fracJourney = distCovered / journeyLength;
        

        if (diff > moveTime)
        {
            transform.position = endPos;
            isMoved = true;
            Destroy(moveObj.GetComponent<TransObject>());
        }

        float rate = diff / moveTime;
        moveObj.transform.position = Vector3.Lerp(startPos, endPos, rate);
    }

}
