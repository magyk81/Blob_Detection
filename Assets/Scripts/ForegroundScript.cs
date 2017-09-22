using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundScript : MonoBehaviour
{
    public DepthSourceManager _DepthManager;

    // Use this for initialization
    void Start()
    {
        if (_DepthManager.getData() != null)
        {
            int width = _DepthManager.getWidth();
            int height = _DepthManager.getHeight();
            gameObject.transform.position = new Vector3(width / 2, -height / 2, 2);
            gameObject.transform.localScale = new Vector3(width, height, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
