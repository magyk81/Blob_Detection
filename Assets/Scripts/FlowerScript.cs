using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerScript : MonoBehaviour
{
    private UnityEngine.Video.VideoPlayer videoPlayer;
    //private UnityEngine.Video.VideoClip videoClip;
    public UnityEngine.Video.VideoClip forwardClip;
    public UnityEngine.Video.VideoClip reverseClip;

    private enum State { START, FORWARD, END, BACKWARD };
    private State currentState;

    private bool bloomed;
    private bool unbloomed;
    private bool timeToUnbloom;
    private bool timeToBloom;

    // Use this for initialization
    void Start()
    {
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        //videoClip = GetComponent<UnityEngine.Video.VideoClip>();

        currentState = State.START;

        useWhichClip(false);

        videoPlayer.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!videoPlayer.isPlaying)
        {
            if (usingWhichClip()) currentState = State.END;
            else currentState = State.START;
        }
    }

    public bool inBounds(int x, int y)
    {
        int localScaleX = (int)gameObject.transform.localScale.x / 1;
        int localScaleY = (int)gameObject.transform.localScale.y / 1;
        int positionX = (int)gameObject.transform.position.x;
        int positionY = (int)gameObject.transform.position.y * (-1);

        if (x > positionX + localScaleX) return false;
        else if (x < positionX - localScaleX) return false;
        else if (y > positionY + localScaleY) return false;
        else if (y < positionY - localScaleY) return false;

        return true;
    }

    public void trigger(bool inBounds)
    {
        //if (!videoPlayer.isPlaying) videoPlayer.Play();
        if (inBounds) tryGoingForward();
        else tryGoingBackward();
    }

    private void tryGoingForward()
    {
        switch (currentState)
        {
            case State.START:
                if (!usingWhichClip()) useWhichClip(true);
                videoPlayer.Play();
                currentState = State.FORWARD;
                break;
        }
    }

    private void tryGoingBackward()
    {
        switch (currentState)
        {
            case State.END:
                if (usingWhichClip()) useWhichClip(false);
                videoPlayer.Play();
                currentState = State.BACKWARD;
                break;
        }
    }

    /**
     * Returns true if using the forward clip.
     * Returns false if using the backward clip.
     */
    private bool usingWhichClip()
    {
        if (GetComponent<UnityEngine.Video.VideoPlayer>().clip
            == forwardClip) return true;
        return false;
    }

    /**
     * Switches to forward clip if parameter is true.
     * Switches to backward clip if parameter is false.
     * Will need to call videoPlayer.Play() to play the new clip.
     */
    private void useWhichClip(bool forward)
    {
        if (forward)
        {
            if (GetComponent<UnityEngine.Video.VideoClip>().
                Equals(forwardClip)) return;
            videoPlayer.Stop();
            GetComponent<UnityEngine.Video.VideoPlayer>().clip = forwardClip;
        }
        else
        {
            if (GetComponent<UnityEngine.Video.VideoClip>().
                Equals(reverseClip)) return;
            videoPlayer.Stop();
            GetComponent<UnityEngine.Video.VideoPlayer>().clip = reverseClip;
        }
    }
}
