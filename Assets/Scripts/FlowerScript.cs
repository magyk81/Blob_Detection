using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerScript : MonoBehaviour
{

    //public VideoClip[] movies;
    private MovieTexture[] movieTextures;
    private MovieTexture movie;
    public Material movieMaterial;

    private UnityEngine.Video.VideoPlayer videoPlayer;
    private UnityEngine.Video.VideoClip videoClip;
    public UnityEngine.Video.VideoClip forwardClip;
    public UnityEngine.Video.VideoClip reverseClip;

    private bool bloomed;
    private bool unbloomed;
    private bool timeToUnbloom;
    private bool timeToBloom;

    //public VideoClip movie;
    //public Texture movieTexture;

    // Use this for initialization
    void Start()
    {
        //movies[0] = new MovieTexture();
        //gameObject.GetComponent<MeshRenderer>();
        //gameObject.GetComponent<Material>().GetTexture = movie as MovieTexture;
        //gameObject.GetComponent<Renderer>().GetComponent<Material>().mainTexture = movies[0] as MovieTexture;
        //movieTexture = new MovieTexture();
        //gameObject.GetComponent<MeshRenderer>();
        //gameObject.GetComponent<Material>().GetTexture = movie as MovieTexture;
        //gameObject.GetComponent<Renderer>().GetComponent<Material>().mainTexture = movieTexture as MovieTexture;

        // for testing
        //movie.Play();

        //movie = gameObject.GetComponent<Renderer>().material.mainTexture as MovieTexture;
        //movie = movieMaterial.mainTexture as MovieTexture;

        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoClip = GetComponent<UnityEngine.Video.VideoClip>(); // This may not work

        bloomed = false;
        unbloomed = true;

        timeToBloom = false;
        timeToUnbloom = false;

        videoPlayer.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (!unbloomed && !bloomed && !videoPlayer.isPlaying)
        {
            bloomed = true;
        }

        if (timeToBloom && unbloomed)
        {
            // This checks if the current clip is the reverse video.
            // If it is, it changes it to the non-reverse video.
            if (!GetComponent<UnityEngine.Video.VideoClip>().Equals(forwardClip))
            {
                GetComponent<UnityEngine.Video.VideoPlayer>().clip = forwardClip;
            }
            if (!videoPlayer.isPlaying) videoPlayer.Play();
            unbloomed = false;
        }

        if (bloomed && timeToUnbloom)
        {
            videoPlayer.Stop();
            GetComponent<UnityEngine.Video.VideoPlayer>().clip = reverseClip;
            videoPlayer.Play();
            bloomed = false;
        }

        /*if (!unbloomed)
        {
            if ((int)videoPlayer.frame < 2) unbloomed = true;
        }*/
    }

    public void trigger(int x, int y, bool bloom)
    {
        int localScaleX = (int)gameObject.transform.localScale.x / 1;
        int localScaleY = (int)gameObject.transform.localScale.y / 1;
        int positionX = (int)gameObject.transform.position.x;
        int positionY = (int)gameObject.transform.position.y * (-1);

        if (x > positionX + localScaleX) return;
        else if (x < positionX - localScaleX) return;
        else if (y > positionY + localScaleY) return;
        else if (y < positionY - localScaleY) return;

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
