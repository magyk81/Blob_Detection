﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerScript : MonoBehaviour {

    public MovieTexture movie;

	// Use this for initialization
	void Start ()
    {
        RawImage rawImage = gameObject.GetComponent<RawImage>();
        rawImage.texture = movie as MovieTexture;

        // for testing
        movie.Play();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
