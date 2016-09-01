using UnityEngine;
using System.Collections;

public class UIDirectionalControl : MonoBehaviour {

    public bool UseRelativeRotation = true;

    private Quaternion RelatviceRotation;
	// Use this for initialization
	void Start ()
    {
        RelatviceRotation = transform.parent.localRotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
        if (UseRelativeRotation)
        {
            transform.rotation = RelatviceRotation;
        }
	}
}
