using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerFactoryScript : MonoBehaviour
{

    public GameObject flower;

    public DepthSourceManager _DepthManager;

    public int dispVert, dispHoriz;

    private ArrayList[] flowerLists;
    //private Space[,] grid;
    private bool[,] grid;

    private int width, height, cellWidth, cellHeight;
    private int emptySpaces;

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

        cellWidth = width / dispHoriz;
        cellHeight = height / dispVert;
        Debug.Log("dispHoriz: " + dispHoriz + ", dispVert: " + dispVert);
        Debug.Log("cellWidth: " + cellWidth + ", cellHeight: " + cellHeight);
        int spaceCount = dispHoriz * dispVert;
        emptySpaces = spaceCount;

        // Instantiating large flowers (3 x 3)
        while (spaceCount > emptySpaces * 2 / 3)
        {
            GameObject flowerClone = Instantiate(flower);
            //flowerClone.transform.localScale.Set(gridWidth * 3, gridHeight * 3, 1);
            flowerClone.transform.localScale.Set(cellWidth, cellHeight, 1);
            flowerLists[0].Add(flowerClone);
            spaceCount -= 9;
        }
        // Instantiating medium flowers (2 x 2)
        while (spaceCount > emptySpaces / 3)
        {
            GameObject flowerClone = Instantiate(flower);
            //flowerClone.transform.localScale.Set(gridWidth * 2, gridHeight * 2, 1);
            flowerClone.transform.localScale.Set(cellWidth, cellHeight, 1);
            flowerLists[1].Add(flowerClone);
            spaceCount -= 4;
        }
        // Instantiating small flowers (1 x 1)
        while (spaceCount > 0)
        {
            GameObject flowerClone = Instantiate(flower);
            flowerClone.transform.localScale.Set(cellWidth, cellHeight, 1);
            flowerLists[2].Add(flowerClone);
            spaceCount--;
        }

        //grid = new Space[gridWidth, gridHeight];
        grid = new bool[gridWidth, gridHeight];
        setupGrid();
        moveFlowers(width, height);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void moveFlowers(int width, int height)
    {
        int horizIncrem = (width / dispHoriz) / 2;
        int vertIncrem = -(height / dispVert) / 2;

        Debug.Log("horizIncrem: " + horizIncrem);

        //for (int i = 0; i < dispHoriz; i++)
        for (int i = 0; i < 5; i++)
        {
            //for (int j = 0; j < dispVert; j++)
            for (int j = 0; j < 1; j++)
            {
                GameObject flower = ((GameObject)(flowerLists[2])[i * dispVert + j]);
                flower.transform.position = new Vector3(horizIncrem * i + horizIncrem,
                    vertIncrem * j + vertIncrem, 0);
                flower.transform.position = new Vector3(57,
                    -57, 0);
                flower.transform.localScale = new Vector3(40, 40, 1);
            }
        }

        /*for (int i = 0; i < flowerList.Count; i++)
        {

        }*/

        Debug.Log("flowerLists[0].Count: " + flowerLists[0].Count);
        Debug.Log("flowerLists[1].Count: " + flowerLists[1].Count);
        Debug.Log("flowerLists[2].Count: " + flowerLists[2].Count);

        /*foreach (GameObject flower in flowerLists[0])
        {
            Coord coord = findOpenSpace(3);
            Debug.Log("x: " + coord.getX() + ", y: " + coord.getY());
            if (coord.isValid())
            {
                flower.transform.position = new Vector3
                    (horizIncrem * coord.getX(), vertIncrem * coord.getY(), 0);
            }
            else break;
        }*/
    }

    /*class Space
    {
        public bool occupied = false;
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
    }*/
    class Coord
    {
        private int _x, _y;
        public Coord(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public int getX() { return _x; }
        public int getY() { return _y; }
        public bool isValid()
        {
            if (_x == -1 && _y == -1) return false;
            else return true;
        }
        public Coord getInvalid()
        {
            _x = -1;
            _y = -1;
            return this;
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

    private bool occupySpace(int x, int y, int size)
    {
        for (int i = x; i < x + size; i++)
        {
            for (int j = y; j < y + size; j++)
            {
                //if (grid[i, j].occupied == true) return false;
                if (grid[i, j] == true) { return false; }
            }
        }
        for (int i = x; i < x + size; i++)
        {
            for (int j = y; j < y + size; j++)
            {
                //grid[i, j].occupied = true;
                grid[i, j] = true;
                emptySpaces--;
            }
        }
        return true;
    }

    /*
     * Looks through the grid to find a space of certain size.
     * Tries looking randomly a number of times proportional to how many empty spaces are left.
     * Won't try looking randomly if the space's size is just 1.
     * If it didn't find one randomly, will check each cell until it finds an empty space.
     * If an empty space is found either way, it returns true. Otherwise it returns false.
     */
    private Coord findOpenSpace(int size)
    {
        int tries = emptySpaces;
        if (size > 1)
        {
            Debug.Log("here");
            while (tries-- > 0)
            {
                int x = (int) (Random.value * (gridWidth - size));
                int y = (int) (Random.value * (gridWidth - size));
                if (occupySpace(x, y, size)) return new Coord(x, y);
            }
        }
        Debug.Log("there");
        for (int i = 0; i <= gridWidth - size; i++)
        {
            for (int j = 0; j <= gridHeight - size; j++)
            {
                if (occupySpace(i, j, size)) return new Coord(i, j);
            }
        }
        return new Coord(-1, -1);
    }

    /*private void setupGrid(int gridWidth, int gridHeight)
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
    }*/
    private void setupGrid()
    {
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                grid[i, j] = false;
            }
        }
    }
}
