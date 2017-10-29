using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerFactoryScript : MonoBehaviour
{

    public GameObject flower;

    public DepthSourceManager _DepthManager;

    public int displayedVertically, displayedHorizontally;

    private ArrayList flowerList;
    private ArrayList flowerChecklist;

    // Use this for initialization
    void Start()
    {
        flowerList = new ArrayList();
        flowerChecklist = new ArrayList();

        for (int i = 0; i < displayedVertically * displayedHorizontally; i++)
        {
            flowerList.Add(Instantiate(flower));
        }

        foreach (GameObject flower in flowerList)
        {
            flowerChecklist.Add(flower);
        }

        if (_DepthManager.getData() != null)
        {
            moveFlowers(_DepthManager.getWidth(), _DepthManager.getHeight());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void moveFlowers(int width, int height)
    {
        int horizIncrem = width / displayedHorizontally;
        int vertIncrem = -height / displayedVertically;

        Debug.Log("horizIncrem: " + horizIncrem);

        for (int i = 0; i < displayedHorizontally; i++)
        //for (int i = 0; i < 1; i++)
        {
            for (int j = 0; j < displayedVertically; j++)
            //for (int j = 0; j < 1; j++)
            {
                GameObject flower = ((GameObject)flowerList[i * displayedVertically + j]);
                /*flower.GetComponent<FlowerScript>().setPosition(horizIncrem * i + (horizIncrem / 2),
                    vertIncrem * j + (vertIncrem / 2), 0);*/
                flower.transform.position = new Vector3(horizIncrem * i + (horizIncrem / 2),
                    vertIncrem * j + (vertIncrem / 2), 0);
                flower.transform.localScale = new Vector3(40, 40, 1);
            }
        }

        for (int i = 0; i < flowerList.Count; i++)
        {

        }
    }

    public void checkFlowers(int x, int y)
    {
        foreach (GameObject flower in flowerList)
        {
            flower.GetComponent<FlowerScript>().inBounds(x, y, 0);
        }
    }

    public void triggerFlowers()
    {
        foreach (GameObject flower in flowerList)
        {
            flower.GetComponent<FlowerScript>().trigger();
        }
    }
}
