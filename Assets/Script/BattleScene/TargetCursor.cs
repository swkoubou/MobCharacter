using UnityEngine;
using System.Collections;

public class TargetCursor : MonoBehaviour
{
    private Transform target;
    public float speed = 6f;
	public float width = 1.2f;
	public float height = 0.5f;

    void Start()
    {
        target = transform.parent.GetComponent<Transform>();    
    }

    void Update () {
		float x = Mathf.Cos(Time.time * speed) * width;
		float y = Mathf.Sin(Time.time * speed) * height;
		float z = 0f;
		transform.position = target.position +  new Vector3(x, y, z);
	}
}