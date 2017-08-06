using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    private bool positioned = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Should only need to be called once
    public void positionCamera(int width, int height)
    {
        if (positioned) return;

        gameObject.transform.position = new Vector3(width / 2, -height / 2, -10);
        Debug.Log("width: " + width + ", height: " + height);

        positioned = true;
    }
}
