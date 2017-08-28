using System.Collections;
using Windows.Kinect;
using UnityEngine;

public class BlobFactoryScript : MonoBehaviour {

    // Cubes to show blobs, spheres to show blob groups
    public GameObject cube, sphere;

    public DepthSourceManager _DepthManager;
    public FlowerFactoryScript _FlowerFactory;

    public int frontCutOff, backCutOff;
    public int blobsDisplayed, spheresDisplayed;
    public int blobMinSize, groupMinSize;

    private int _MaxDistance = 8000;
    private int _CutOffIncrement = 100;
    private float _ScaleIncrement = 0.025F;
    private int _OffsetIncrement = 5;
    private float _MaxScale = 1.0F;
    private float _MinScale = 0.1F;

    private int _DownSample = 4;
    private int _DeadDepthValue = 500;
    private int _GroupsToDetect = 30;
    private int _ScenerySamples = 100;

    private float widthScale = 1.0F;
    private float heightScale = 1.0F;
    private int xOffset = 0;
    private int yOffset = 0;

    private int scenerySamples;

    private bool paused = false;
    private bool shiftKeyDown = false;

    private ushort[] depthData;

    private ArrayList cubeList, sphereList, blobList, sceneryBlobList;
    private ushort[] blobGroupRoster;

	// Use this for initialization
	void Start ()
    {
        cubeList = new ArrayList(); sphereList = new ArrayList();
        blobList = new ArrayList(); blobGroupRoster = new ushort[_GroupsToDetect];
        sceneryBlobList = new ArrayList();

        // Set up cubes equal to the number of max blobs displayed
        for (int i = 0; i < blobsDisplayed; i++)
        { cubeList.Add(Instantiate(cube)); }

        for (int i = 0; i < spheresDisplayed; i++)
        { sphereList.Add(Instantiate(sphere)); }

        // Make the initial cube and sphere invisible
        cube.GetComponent<MeshRenderer>().enabled = false;
        sphere.GetComponent<MeshRenderer>().enabled = false;

        scenerySamples = _ScenerySamples;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || scenerySamples < _ScenerySamples)
            makeScenery();

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (paused) paused = false;
            else paused = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)
            || Input.GetKeyDown(KeyCode.RightShift))
        {
            shiftKeyDown = true;
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift)
            || Input.GetKeyUp(KeyCode.RightShift))
        {
            shiftKeyDown = false;
        }

        if (shiftKeyDown)
        {
            bool printCutOff = false;
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                frontCutOff += _CutOffIncrement;
                printCutOff = true;
            }
            else if (Input.GetKeyDown(KeyCode.PageDown))
            {
                frontCutOff -= _CutOffIncrement;
                printCutOff = true;
            }
            if (frontCutOff <= backCutOff)
            {
                frontCutOff = backCutOff + _CutOffIncrement;
                if (frontCutOff <= _MaxDistance)
                {
                    frontCutOff = _MaxDistance;
                    backCutOff = frontCutOff - _CutOffIncrement;
                }
            }
            if (printCutOff) Debug.Log("Front Cut Off: " + frontCutOff
                + ", Back Cut Off: " + backCutOff);

            bool printScale = false;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                heightScale += _ScaleIncrement;
                printScale = true;
                if (heightScale > _MaxScale) heightScale = _MaxScale;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                heightScale -= _ScaleIncrement;
                printScale = true;
                if (heightScale < _MinScale) heightScale = _MinScale;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                widthScale -= _ScaleIncrement;
                printScale = true;
                if (widthScale < _MinScale) widthScale = _MinScale;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                widthScale += _ScaleIncrement;
                printScale = true;
                if (widthScale > _MaxScale) widthScale = _MaxScale;
            }
            if (printScale) Debug.Log("Width Scale: " + widthScale * 100
                + "%, Height Scale: " + heightScale * 100 + "%");
        }

        else
        {
            bool printCutOff = false;
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                backCutOff += _CutOffIncrement;
                printCutOff = true;
            }
            else if (Input.GetKeyDown(KeyCode.PageDown))
            {
                backCutOff -= _CutOffIncrement;
                printCutOff = true;
            }
            if (backCutOff >= frontCutOff)
            {
                backCutOff = frontCutOff - _CutOffIncrement;
            }
            if (printCutOff) Debug.Log("Front Cut Off: " + frontCutOff
                + ", Back Cut Off: " + backCutOff);

            bool printOffset = false;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                yOffset -= _OffsetIncrement;
                printOffset = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                yOffset += _OffsetIncrement;
                printOffset = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                xOffset -= _OffsetIncrement;
                printOffset = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                xOffset += _OffsetIncrement;
                printOffset = true;
            }
            if (printOffset) Debug.Log("X_Offset: " + xOffset
                + ", Y_Offset: " + yOffset);
        }

        if (paused) return;

        blobList.Clear();
        for (int i = 0; i < _GroupsToDetect; i++) { blobGroupRoster[i] = 0; }

        depthData = _DepthManager.getData();
        if (depthData == null) return; // This means _Sensor is null

        makeBlobs(depthData, _DepthManager.getWidth(), _DepthManager.getHeight());

        filterBlobs();

        makeGroups();

        filterGroups();

        //moveCubes();

        //moveSpheres();

        bloomFlowers();
    }

    private void makeBlobs(ushort[] data, int width, int height)
    {
        for (int y = 0; y < height; y += _DownSample)
        {
            for (int x = 0; x < width; x += _DownSample)
            {
                int value = getAverage(data, x, y, width);

                if (value < backCutOff || value > frontCutOff) continue;

                bool isScenery = false;
                foreach (Blob blob in sceneryBlobList)
                {
                    if (blob.isNear(x, y) > 0)
                    {
                        isScenery = true;
                        break;
                    }
                }
                if (isScenery) continue;

                bool found = false; // Set to true if a nearby blob is found
                // Tag of the closest blob that has been found so far
                int nearestBlobSoFar = -1;
                // Value of how close the nearest found blob is
                int nearestToABlobSoFar = 0;
                for (int i = 0; i < blobList.Count; i++)
                {
                    // howNear will be positive if it's found
                    int howNear = ((Blob)blobList[i]).isNear(x, y);
                    if (howNear <= 0) continue;
                    found = true;
                    if (howNear > nearestToABlobSoFar)
                    {
                        nearestToABlobSoFar = howNear;
                        nearestBlobSoFar = i;
                    }
                }
                if (found) { ((Blob)blobList[nearestBlobSoFar]).add(x, y); }
                else { Blob b = new Blob(x, y); blobList.Add(b); }
            }
        }
    }

    private void filterBlobs()
    {
        int width = _DepthManager.getWidth();
        int height = _DepthManager.getHeight();

        ArrayList temp = new ArrayList();
        for (int i = 0; i < blobList.Count; i++)
        {
            Blob blob = (Blob)blobList[i];
            if ((blob).size < blobMinSize) temp.Add(i);
            else relocateBlob(blob, width / 2, height / 2);
        }
        for (int i = 0; i < temp.Count; i++)
        {
            blobList.RemoveAt((int)temp[i] - i);
        }
    }

    private void makeGroups()
    {
        int currentTag = 1;
        foreach (Blob blob in blobList)
        {
            if (currentTag >= _GroupsToDetect) break;
            if (blob.groupTag > 0) continue;
            setUpBlobGroup(blob, currentTag);
            currentTag++;
        }
    }

    private void setUpBlobGroup(Blob blob, int currentTag)
    {
        if (blob.groupTag == 0) blob.groupTag = currentTag;
        else return;

        blobGroupRoster[currentTag]++;

        foreach (Blob other in blobList)
        {
            if (other.groupTag > 0) continue;
            if (other.isNextTo(blob)) setUpBlobGroup(other, currentTag);
        }
    }

    private void filterGroups()
    {
        ArrayList temp = new ArrayList();
        for (int i = 0; i < blobList.Count; i++)
        {
            Blob blob = (Blob)blobList[i];
            if (blobGroupRoster[blob.groupTag] < groupMinSize)
            {
                temp.Add(i);
            }
        }
        for (int i = 0; i < temp.Count; i++)
        {
            blobList.RemoveAt((int)temp[i] - i);
        }
    }

    private void moveCubes()
    {
        for (int i = 0; i < Mathf.Min(blobList.Count, cubeList.Count); i++)
        {
            Blob b = (Blob)blobList[i];
            GameObject cube = (GameObject)cubeList[i];
            
            cube.SetActive(true);
            cube.transform.position = new Vector3(b.cx, -b.cy, 0);
            cube.transform.localScale = new Vector3(b.w, b.h, 1);
        }

        if (blobList.Count > cubeList.Count) return;
        for (int i = blobList.Count; i < cubeList.Count; i++)
        { ((GameObject)cubeList[i]).SetActive(false); }
    }

    private void bloomFlowers()
    {
        foreach (Blob blob in blobList)
        {
            _FlowerFactory.bloomFlowers(blob.cx, blob.cy);
        }
    }

    private void moveSpheres()
    {
        int numberOfBlobGroups = countBlobGroups();
        int min = Mathf.Min(numberOfBlobGroups, spheresDisplayed);
        int[] avgX = new int[min];
        int[] avgY = new int[min];
        int[] divisor = new int[min];
        for (int i = 0; i < min; i++)
        {
            avgX[i] = 0;
            avgY[i] = 0;
            divisor[i] = 0;
        }
        foreach (Blob blob in blobList)
        {
            int blobGroup = blob.groupTag - 1;
            if (blobGroup < min)
            {
                avgX[blobGroup] += blob.cx;
                avgY[blobGroup] += blob.cy;
                divisor[blobGroup]++;
            }
        }

        for (int i = 0; i < min; i++)
        {
            GameObject sphere = (GameObject)sphereList[i];
            if (divisor[i] == 0)
            {
                sphere.SetActive(false);
                continue;
            }
            avgX[i] /= divisor[i];
            avgY[i] /= divisor[i];

            sphere.SetActive(true);
            sphere.transform.position = new Vector3(avgX[i], -avgY[i], -1);
            sphere.transform.localScale = new Vector3(divisor[i], divisor[i], 1);
        }

        if (numberOfBlobGroups > sphereList.Count) { return; }
        for (int i = numberOfBlobGroups; i < sphereList.Count; i++)
        {
            GameObject sphere = (GameObject)sphereList[i];
            sphere.SetActive(false);
        }
        //Debug.Log("number of blob groups:" + numberOfBlobGroups);
    }

    private void makeScenery()
    {
        if (scenerySamples == 0)
        {
            Debug.Log(sceneryBlobList.Count + " blobs in the scenery list");
            scenerySamples = _ScenerySamples;
        }
        else
        {
            sceneryBlobList.AddRange(blobList);
            scenerySamples--;
            Debug.Log("samples to go: " + scenerySamples);
        }
    }

    class Blob
    {
        public int x, y, w, h, cx, cy;
        int maxSize = 20;
        int _Threshold2 = 20;
        int _MinSize = 20;
        public int groupTag = 0;
        public int size = 0;

        public Blob(int x_, int y_)
        {
            x = x_;
            y = y_;
            w = 1;
            h = 1;
            cx = x;
            cy = y;
        }

        public void add(int x_, int y_)
        {
            if (x_ > x + w) w = x_ - x;
            else if (x_ < x) { w += x - x_; x = x_; }
            if (y_ > y + h) h = y_ - y;
            else if (y_ < y) { h += y - y_; y = y_; }

            cx = x + (w / 2);
            cy = y + (h / 2);

            size++;
        }

        /**
         * Negative = not near
         * Positive = near, Greater = more near
         */
        public int isNear(int x_, int y_)
        {
            return -(Mathf.Abs(cx - x_) + Mathf.Abs(cy - y_) - maxSize);
        }

        public bool isNextTo(Blob other)
        {
            if (other.x <= x + w + _Threshold2
                && other.x + other.w >= x - _Threshold2
                && other.y <= y + h + _Threshold2
                && other.y + other.h >= y - _Threshold2) return true;
            return false;
        }
    }

    private int getAverage(ushort[] data, int x, int y, int width)
    {
        int sum = 0;

        for (int i = 0; i < _DownSample; i++)
        {
            for (int j = 0; j < _DownSample; j++)
            {
                int value = data[((y + j) * width) + x + i];
                if (value == 0) sum += _DeadDepthValue;
                else sum += value;
            }
        }

        return sum / (_DownSample * _DownSample);
    }

    private int countBlobGroups()
    {
        int count = 0;
        foreach (Blob blob in blobList)
        {
            if (blob.groupTag > count) count = blob.groupTag;
        }
        return count;
    }

    private void relocateBlob(Blob blob, int midWidth, int midHeight)
    {
        blob.x = blob.x + (int)((float)(midWidth - blob.x) * (1.0 - widthScale));
        blob.w = (int)((float)blob.w * widthScale);
        
        blob.y = blob.y + (int)((float)(midHeight - blob.y) * (1.0 - heightScale));
        blob.h = (int)((float)blob.h * heightScale);
        
        blob.x += xOffset;
        blob.y += yOffset;

        blob.cx = blob.x + blob.w;
        blob.cy = blob.y + blob.h;
    }
}
