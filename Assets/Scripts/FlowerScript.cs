using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerScript : MonoBehaviour {

    //public VideoClip[] movies;
    private MovieTexture[] movieTextures;
    private MovieTexture movie;
    public Material movieMaterial;

    private UnityEngine.Video.VideoPlayer videoPlayer;
    private UnityEngine.Video.VideoClip videoClip;
    public UnityEngine.Video.VideoClip reverseClip;

    private bool bloomed;
    private bool unbloomed;
    private bool timeToUnbloom;
    private bool timeToBloom;

	// Use this for initialization
	void Start ()
    {
        //movies[0] = new MovieTexture();
        //gameObject.GetComponent<MeshRenderer>();
        //gameObject.GetComponent<Material>().GetTexture = movie as MovieTexture;
        //gameObject.GetComponent<Renderer>().GetComponent<Material>().mainTexture = movies[0] as MovieTexture;

        // for testing
        //movie.Play();

        //movie = gameObject.GetComponent<Renderer>().material.mainTexture as MovieTexture;
        //movie = movieMaterial.mainTexture as MovieTexture;

        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoClip = GetComponent<UnityEngine.Video.VideoClip>();

        bloomed = false;
        unbloomed = true;

        timeToBloom = false;
        timeToUnbloom = false;

        videoPlayer.Pause();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (timeToBloom && unbloomed)
        {
            videoPlayer.Stop();
            GetComponent<UnityEngine.Video.VideoPlayer>().clip = videoClip;
            videoPlayer.Play();
            unbloomed = false;
        }

        if (!bloomed)
        {
            //Debug.Log("testing");
            if ((int)videoPlayer.frame == (int)videoPlayer.frameCount) bloomed = true;
        }

        if (bloomed && timeToUnbloom)
        {
            videoPlayer.Stop();
            GetComponent<UnityEngine.Video.VideoPlayer>().clip = reverseClip;
            videoPlayer.Play();
            bloomed = false;
        }

        if (!unbloomed)
        {
            if ((int)videoPlayer.frame < 2) unbloomed = true;
        }
	}

    public void bloom(int x, int y, bool bloom)
    {
        int localScaleX = (int)gameObject.transform.localScale.x / 2;
        int localScaleY = (int)gameObject.transform.localScale.y / 2;
        int positionX = (int)gameObject.transform.position.x;
        int positionY = (int)gameObject.transform.position.y;

        if (x > positionX + localScaleX) return;
        else if (x < positionX - localScaleX) return;
        //else if (y > positionY + localScaleY) return;
       // else if (y < positionY - localScaleY) return;

        if (bloom)
        {
            timeToBloom = true;
            timeToUnbloom = false;
        }

        else
        {
            timeToUnbloom = true;
            timeToBloom = false;
        }
    }
}
