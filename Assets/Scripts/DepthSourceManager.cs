using UnityEngine;
using Windows.Kinect;

public class DepthSourceManager : MonoBehaviour {

    public Camera mainCamera;

    private KinectSensor _Sensor;
    private DepthFrameReader _Reader;
    private ushort[] _Data = null;
    private int _Width, _Height;

	// Use this for initialization
	void Start ()
    {
        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.DepthFrameSource.OpenReader();
            var frameDesc = _Sensor.DepthFrameSource.FrameDescription;
            _Data = new ushort[frameDesc.LengthInPixels];
            _Width = frameDesc.Width;
            _Height = frameDesc.Height;
            mainCamera.GetComponent<CameraScript>().positionCamera(_Width, _Height);
        }
        else _Data = null;
        // Check if _Sensor is null by calling getData()

        if (!_Sensor.IsOpen) { _Sensor.Open(); }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_Reader == null) return;

        var frame = _Reader.AcquireLatestFrame();
        if (frame == null) return;
        frame.CopyFrameDataToArray(_Data);

        frame.Dispose();
        frame = null;
	}

    public ushort[] getData()
    {
        if (_Sensor != null) return _Data;
        else return null;
    }

    public int getWidth() { return _Width; }

    public int getHeight() { return _Height; }

    private void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}