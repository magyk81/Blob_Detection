using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerFactoryScript : MonoBehaviour {

    public GameObject flower;

    public DepthSourceManager _DepthManager;

    public int displayedVertically, displayedHorizontally;

    private ArrayList flowerList;

	// Use this for initialization
	void Start () {

        flowerList = new ArrayList();

        for (int i = 0; i < displayedVertically * displayedHorizontally; i++)
        {
            flowerList.Add(Instantiate(flower));
        }

        if (_DepthManager.getData() != null)
        {
            moveFlowers(_DepthManager.getWidth(), _DepthManager.getHeight());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void moveFlowers(int width, int height)
    {
        int horizIncrem = width / displayedHorizontally;
        int vertIncrem = -height / displayedVertically;

        for (int i = 0; i < displayedHorizontally; i++)
        {
            for (int j = 0; j < displayedVertically; j++)
            {
                GameObject flower = ((GameObject)flowerList[i * displayedVertically + j]);
                flower.transform.position = new Vector3(horizIncrem * i + (horizIncrem / 2),
                    vertIncrem * j + (vertIncrem / 2), 0);
                flower.transform.localScale = new Vector3(7, 7, 7);
            }
        }

        for (int i = 0; i < flowerList.Count; i++)
        {

        }
    }

    public void bloomFlowers(int x, int y)
    {
        foreach(GameObject flower in flowerList)
        {
            flower.GetComponent<FlowerScript>().bloom(x, y, true);
        }
    }
}
