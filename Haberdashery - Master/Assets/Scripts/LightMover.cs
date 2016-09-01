using UnityEngine;
using System.Collections;

public class LightMover : MonoBehaviour
{
    public float angle;
    public float period;
    Vector3 startingPos;

    private float time;

    void Start()
    {
        startingPos = transform.localPosition;
    }
    
    void Update()
    {
        time = time + Time.deltaTime;
        float phase = Mathf.Sin(time / period); 

        transform.localPosition = new Vector3((phase * angle), startingPos.y, startingPos.z);
    }
}
