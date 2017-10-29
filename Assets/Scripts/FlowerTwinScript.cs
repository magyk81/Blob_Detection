using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerTwinScript : MonoBehaviour
{
    private UnityEngine.Video.VideoPlayer videoPlayer;
    public UnityEngine.Video.VideoClip reverseClip;

    private bool front;

    private int frame;
    private int frameCount;

    private Vector3 frontPosition;
    private Vector3 backPosition;

    // Use this for initialization
    void Start ()
    {
        reverseClip = Instantiate(reverseClip);
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();

        videoPlayer.Stop();
        front = false;

        //frameCount = 0;
        //frameCount = (int)videoPlayer.clip.frameCount;

        frontPosition = new Vector3();
        backPosition = new Vector3();
    }
	
	// Update is called once per frame
	void Update ()
    {
		frame = (int)videoPlayer.frame;
    }

    int getFrame()
    {
        return (int) videoPlayer.frame;
    }

    public bool isFront()
    {
        return front;
    }

    public void bringToFront(int frame)
    {
        videoPlayer.frame = frame;
        videoPlayer.Play();
        gameObject.transform.position = frontPosition;
        front = true;
    }

    public void bringToBack()
    {
        gameObject.transform.position = backPosition;
        videoPlayer.Stop();
        front = false;
    }

    public void setPosition(int x, int y, int z)
    {
        frontPosition.Set(x, y, z - 1);
        backPosition.Set(x, y, z + 1);
    }
}
