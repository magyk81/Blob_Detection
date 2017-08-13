using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerScript : MonoBehaviour {

    //public VideoClip[] movies;
    private MovieTexture[] movieTextures;
    public GameObject gameObject;

	// Use this for initialization
	void Start ()
    {
        movies[0] = new MovieTexture();
        //gameObject.GetComponent<MeshRenderer>();
        //gameObject.GetComponent<Material>().GetTexture = movie as MovieTexture;
        gameObject.GetComponent<Renderer>().GetComponent<Material>().mainTexture = movies[0] as MovieTexture;

        // for testing
        //movie.Play();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
