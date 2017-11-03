using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FlowerScript : MonoBehaviour {

    public GameObject forwardFlower;
    public GameObject reverseFlower;

    public int blobMassThresh;

    private VideoPlayer forwardPlayer;
    private VideoPlayer reversePlayer;

    private VideoClip forwardClip;
    private VideoClip reverseClip;

    private enum State { START, FORWARD, END, BACKWARD };
    private State currentState;

    private int blobMass;
    private int blobMassMax;

    // Use this for initialization
    void Start ()
    {
        forwardPlayer = forwardFlower.GetComponent<VideoPlayer>();
        reversePlayer = reverseFlower.GetComponent<VideoPlayer>();

        forwardClip = forwardPlayer.clip;
        reverseClip = reversePlayer.clip;

        currentState = State.START;

        blobMass = 0;
        blobMassMax = blobMassThresh * 2;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    /*
     * Measures how much blob mass is in bounds.
     */
    public void inBounds(int x, int y, int mass)
    {
        int localScaleX = (int)gameObject.transform.localScale.x;
        int localScaleY = (int)gameObject.transform.localScale.y;
        int positionX = (int)gameObject.transform.position.x;
        int positionY = (int)gameObject.transform.position.y * (-1);

        if (x > positionX + localScaleX) return;
        else if (x < positionX - localScaleX) return;
        else if (y > positionY + localScaleY) return;
        else if (y < positionY - localScaleY) return;

        //blobMass += mass;
        blobMass += 2;
        if (blobMass > blobMassMax) blobMass = blobMassMax;
    }

    public void trigger()
    {
        if (blobMass > blobMassThresh) tryGoingForward();
        else if (blobMass == 0) tryGoingBackward();
        blobMass--;
        if (blobMass < 0) blobMass = 0;
    }

    private void tryGoingForward()
    {
        switch (currentState)
        {
            case State.START:
                forwardFlower.SetActive(true);
                currentState = State.FORWARD;
                break;
            case State.BACKWARD:
                reverseFlower.SetActive(false);
                forwardFlower.SetActive(true);
                currentState = State.FORWARD;
                break;
        }
    }

    private void tryGoingBackward()
    {
        switch (currentState)
        {
            case State.END:
                reverseFlower.SetActive(true);
                currentState = State.BACKWARD;
                break;
            case State.FORWARD:
                forwardFlower.SetActive(false);
                reverseFlower.SetActive(true);
                currentState = State.BACKWARD;
                break;
        }
    }
}
