using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerScript : MonoBehaviour
{
    private UnityEngine.Video.VideoPlayer videoPlayer;
    public UnityEngine.Video.VideoClip forwardClip;
    public UnityEngine.Video.VideoClip reverseClip;

    private enum State { START, FORWARD, END, BACKWARD };
    private State currentState;
    private int blobsInBound;
    public int touchThreshold;
    private int touch;

    private int frame;
    private int frameCount;
    private int frameChange;
    public int frameRate;

    // Use this for initialization
    void Start()
    {
        forwardClip = Instantiate(forwardClip);
        reverseClip = Instantiate(reverseClip);

        blobsInBound = 0;
        touch = 0;

        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();

        currentState = State.END;

        useWhichClip(true);

        videoPlayer.Stop();

        frame = 0;
        frameCount = (int) videoPlayer.clip.frameCount;
        frameChange = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (videoPlayer.isPlaying)
        //{
            frame = (int) videoPlayer.frame;
        //}
        
        if (frame >= frameCount - 1 && !usingWhichClip())
        {
            currentState = State.START;
        }
        else if (frame >= frameCount - 1 && usingWhichClip())
        {
            currentState = State.END;
        }

        //changeFrame(true);
    }

    /**
     * Adds to the counter if the (x, y) coordinates are within the
     * quad bounds of the flower. Otherwise, does not add to the counter.
     */ 
    public void inBounds(int x, int y)
    {
        int localScaleX = (int)gameObject.transform.localScale.x;
        int localScaleY = (int)gameObject.transform.localScale.y;
        int positionX = (int)gameObject.transform.position.x;
        int positionY = (int)gameObject.transform.position.y * (-1);

        if (x > positionX + localScaleX) return;
        else if (x < positionX - localScaleX) return;
        else if (y > positionY + localScaleY) return;
        else if (y < positionY - localScaleY) return;

        blobsInBound++;
    }

    /**
     * If there were any blobs in bound, it will try blooming.
     * If there were no blobs in bound, it will try unblooming.
     */
    public void trigger()
    {
        if (blobsInBound > 0) tryGoingForward();
        else tryGoingBackward();
        blobsInBound = 0;
    }

    /**
     * If it's ready to start, start blooming.
     * If it's already going forward, do nothing.
     * If it's going backward, make it go forward by replacing the clip with
     * the forward one and set at the mirrored time frame so that it appears seemless.
     */
    private void tryGoingForward()
    {
        switch (currentState)
        {
            case State.START:
                //Debug.Log("Start");
                //if (touch++ < touchThreshold) break;
                //Debug.Log("start -> forward");
                touch = 0;
                videoPlayer.Stop();
                if (!usingWhichClip()) useWhichClip(true);
                videoPlayer.Play();
                currentState = State.FORWARD;
                break;
            case State.BACKWARD:
                //Debug.Log("Backward");
                if (touch++ < touchThreshold) break;
                //Debug.Log("backward -> forward");
                touch = 0;
                videoPlayer.Stop();
                if (!usingWhichClip()) useWhichClip(true);
                mirrorFrame();
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
                //Debug.Log("End");
                //if (touch++ < touchThreshold) break;
                //Debug.Log("end -> backward");
                touch = 0;
                videoPlayer.Stop();
                if (usingWhichClip()) useWhichClip(false);
                videoPlayer.Play();
                currentState = State.BACKWARD;
                break;
            case State.FORWARD:
                
                if (touch++ < touchThreshold) break;
                //Debug.Log("Forward");
                //Debug.Log("forward -> backward");
                touch = 0;
                videoPlayer.Stop();
                if (usingWhichClip()) useWhichClip(false);
                mirrorFrame();
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
        if (videoPlayer.clip
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
            if (videoPlayer.clip == forwardClip) return;
            videoPlayer.Stop();
            videoPlayer.clip = forwardClip;
        }
        else
        {
            if (videoPlayer.clip == reverseClip) return;
            videoPlayer.Stop();
            videoPlayer.clip = reverseClip;
        }
    }

    private void mirrorFrame()
    {
        frame = (frameCount - 1) - frame;
        videoPlayer.frame = frame;
    }

    private void changeFrame(bool up)
    {
        if (up)
        {
            frameChange++;
            if (frameChange > frameRate)
            {
                frame++;
                videoPlayer.frame = frame;
                frameChange = 0;
            }
            if (frame > frameCount - 1)
            {
                frame = frameCount - 1;
            }
        }
        else
        {
            frameChange--;
            if (frameChange < (-1) * frameRate)
            {
                frame--;
                videoPlayer.frame = frame;
                frameChange = 0;
            }
            if (frame < 0)
            {
                frame = 0;
            }
        }
    }
}
