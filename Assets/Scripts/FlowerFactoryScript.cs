using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerFactoryScript : MonoBehaviour
{

    public GameObject flower;

    public DepthSourceManager _DepthManager;

    public int dispVert, dispHoriz;
    
    private ArrayList[] flowerLists;
    private Space[,] grid;

    private int width, height;

    // Use this for initialization
    void Start()
    {
        flowerLists = new ArrayList[3];
        for (int i = 0; i < flowerLists.Length; i++)
        {
            flowerLists[i] = new ArrayList();
        }

        if (_DepthManager.getData() != null)
        {
            width = _DepthManager.getWidth();
            height = _DepthManager.getHeight();
        }

        int gridWidth = width / dispHoriz;
        int gridHeight = height / dispVert;
        int spaceCount = dispHoriz * dispVert;
        int spaceTotal = spaceCount;

        // Instantiating large flowers (3 x 3)
        while (spaceCount > spaceTotal * 2 / 3)
        {
            GameObject flowerClone = Instantiate(flower);
            flowerClone.transform.localScale.Set(gridWidth * 3, gridHeight * 3, 1);
            flowerLists[0].Add(flowerClone);
            Debug.Log("spaceCount: " + spaceCount);
            spaceCount -= 9;
        }
        // Instantiating medium flowers (2 x 2)
        while (spaceCount > spaceTotal / 3)
        {
            GameObject flowerClone = Instantiate(flower);
            flowerClone.transform.localScale.Set(gridWidth * 2, gridHeight * 2, 1);
            flowerLists[1].Add(flowerClone);
            spaceCount -= 4;
        }
        // Instantiating small flowers (1 x 1)
        while (spaceCount > 0)
        {
            GameObject flowerClone = Instantiate(flower);
            flowerClone.transform.localScale.Set(gridWidth, gridHeight, 1);
            flowerLists[2].Add(flowerClone);
            spaceCount--;
        }

        grid = new Space[gridWidth, gridHeight];
        setupGrid(gridWidth, gridHeight);
        moveFlowers(width, height);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void moveFlowers(int width, int height)
    {
        int horizIncrem = width / dispHoriz;
        int vertIncrem = -height / dispVert;

        Debug.Log("horizIncrem: " + horizIncrem);

        /*//for (int i = 0; i < dispHoriz; i++)
        for (int i = 0; i < 1; i++)
        {
            //for (int j = 0; j < dispVert; j++)
            for (int j = 0; j < 1; j++)
            {
                GameObject flower = ((GameObject)(flowerLists[2])[i * dispVert + j]);
                flower.transform.position = new Vector3(horizIncrem * i + (horizIncrem / 2),
                    vertIncrem * j + (vertIncrem / 2), 0);
                flower.transform.localScale = new Vector3(40, 40, 1);
            }
        }*/

        /*for (int i = 0; i < flowerList.Count; i++)
        {

        }*/

        Debug.Log("flowerLists[0].Count: " + flowerLists[0].Count);
        Debug.Log("flowerLists[1].Count: " + flowerLists[1].Count);
        Debug.Log("flowerLists[2].Count: " + flowerLists[2].Count);
    }

    class Space
    {
        bool taken = false;
        Space[] neighbors = new Space[4];

        public enum Direction { NORTH, EAST, SOUTH, WEST };

        public void setNeighbor(Space neighbor, Direction dir)
        {
            switch (dir)
            {
                case Direction.NORTH:
                    neighbors[0] = neighbor;
                    break;
                case Direction.EAST:
                    neighbors[1] = neighbor;
                    break;
                case Direction.SOUTH:
                    neighbors[2] = neighbor;
                    break;
                case Direction.WEST:
                    neighbors[3] = neighbor;
                    break;
            }
        }

        public void occupy(int size)
        {
            if (size == 0 || taken) return;
            taken = true;
            if (neighbors[1] != null) neighbors[1].occupy(size - 1);
            if (neighbors[2] != null) neighbors[2].occupy(size - 1);
        }
    }

    public void checkFlowers(int x, int y)
    {
        for (int i = 0; i < flowerLists.Length; i++)
        {
            foreach (GameObject flower in flowerLists[i])
            {
                flower.GetComponent<FlowerScript>().inBounds(x, y, 0);
            }
        }
    }

    public void triggerFlowers()
    {
        for (int i = 0; i < flowerLists.Length; i++)
        {
            foreach (GameObject flower in flowerLists[i])
            {
                flower.GetComponent<FlowerScript>().trigger();
            }
        }
    }

    private void setupGrid(int gridWidth, int gridHeight)
    {
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                grid[i, j] = new Space();
            }
        }

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                if (j == 0) grid[i, j].setNeighbor(null, Space.Direction.NORTH);
                else grid[i, j].setNeighbor(grid[i, j - 1], Space.Direction.NORTH);

                if (i == gridWidth - 1) grid[i, j].setNeighbor(null, Space.Direction.EAST);
                else grid[i, j].setNeighbor(grid[i + 1, j], Space.Direction.EAST);

                if (j == gridHeight - 1) grid[i, j].setNeighbor(null, Space.Direction.SOUTH);
                else grid[i, j].setNeighbor(grid[i, j + 1], Space.Direction.SOUTH);

                if (i == 0) grid[i, j].setNeighbor(null, Space.Direction.WEST);
                else grid[i, j].setNeighbor(grid[i - 1, j], Space.Direction.WEST);
            }
        }
    }
}
