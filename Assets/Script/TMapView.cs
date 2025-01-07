using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum TMapViewColor
{
    black,
    brown,
    green,
    purple,
    yellow,
    blue,
    gray,
    orange,
    red,
    white
}

[Serializable]
public class TMapViewLocation
{
    public float latitude;
    public float longitude;
}

[Serializable]
public class TMapViewMarker
{
    public enum TMapMarkerSize
    {
        Tiny,
        Small,
        Mid
    }
    public TMapMarkerSize size;
    public TMapColor color;
    public string label;
    public TMapLocation[] locations;
}

[Serializable]
public class TMapViewPath
{
    public int weight = 5;
    public TMapColor color;
    public bool fill = false;
    public TMapColor fillColor;
    public List<TMapLocation> locations = new List<TMapLocation>();
}

public class TMapView : MonoBehaviour
{
    public RawImage rawImage;
    [Range(0f, 1f)]
    public float _transparency = 1f;
    public TMapLocation centerLocation;
    [Range(1, 18)]
    public int _zoom = 14;
    private int _mapWidth = 512;
    private int _mapHeight = 512;
    public MapType mapType = MapType.RoadMap;
    public int _mapScale = 1;
    public TMapColor markerColor = TMapColor.red;
    public char _label = 'C';
    public float _mapCenterLongitude;
    public float _mapCenterLatitude;

    private string _url;
    private UnityEngine.Color _rawImageColor = UnityEngine.Color.white;
    public bool _IsLocaeCenter = false;

    public float _moveSpeed = 1f;
    private float _zoomSpeed = 0.1f;
    public float _rotationSpeed = 1f;
    public float _minDistanceBetweenFingers = 50f;

    private bool _isRotating = false;
    private Vector2 _rotationStartPosition;

    string appKey = "WOWlmlCdpG6sanl6D6Jam4YWhqZF9vTL6ybC4f8Z";
    string coordType = "WGS84GEO";
    string format = "PNG";
    string markers = "126.987038,37.565207";

    private void Start()
    {
        StartCoroutine(GetMapImage());
    }

    IEnumerator GetMapImage()
    {
        yield return new WaitForSeconds(0.5f);

        _rawImageColor.a = _transparency;
        rawImage.color = _rawImageColor;

        _label = Char.ToUpper(_label);

        string url = "https://apis.openapi.sk.com/tmap/staticMap?";
        string customUrl = "";

        customUrl += "appKey=" + appKey;
        customUrl += "&longitude=" + centerLocation.longitude;
        customUrl += "&latitude=" + centerLocation.latitude;
        customUrl += "&coordType=" + coordType;
        customUrl += "&zoom=" + _zoom;
        customUrl += "&markers=" + markers;

        customUrl += "&format=" + format;
        customUrl += "&width=" + _mapWidth;
        customUrl += "&height=" + _mapHeight;

        url += customUrl;

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            rawImage.texture = DownloadHandlerTexture.GetContent(request);
        }
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            HandleTouchInput();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = touch.deltaPosition;
                Vector2 moveDirection = Quaternion.Euler(0, 0, -rawImage.rectTransform.localEulerAngles.z) * touchDeltaPosition;

                float moveX = -moveDirection.x * _moveSpeed * Time.deltaTime;
                float moveY = -moveDirection.y * _moveSpeed * Time.deltaTime;

                centerLocation.latitude += moveY;
                centerLocation.longitude += moveX;

                RefreshMap();
            }
        }
        else if (Input.touchCount >= 2)
        {
            Touch firstTouch = Input.touches[0];
            Touch secondTouch = Input.touches[1];

            if (firstTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Moved)
            {
                float touchDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
                float touchDeltaDistance = (touchDistance - Vector2.Distance(firstTouch.position
                    - firstTouch.deltaPosition, secondTouch.position - secondTouch.deltaPosition)) * _zoomSpeed * Time.deltaTime;
                _zoom = Mathf.Clamp(_zoom + Mathf.RoundToInt(touchDeltaDistance), 1, 20);

                RefreshMap();
            }

            if (!_isRotating)
            {
                _rotationStartPosition = (firstTouch.position + secondTouch.position) / 2f;
                _isRotating = true;
            }

            if (_isRotating && (firstTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Moved))
            {
                Vector2 currentRotationPosition = (firstTouch.position + secondTouch.position) / 2f;
                Vector2 rotationVector = currentRotationPosition - _rotationStartPosition;
                float rotationAngle = Vector2.SignedAngle(rotationVector, Vector2.right) * _rotationSpeed * Time.deltaTime;

                RectTransform rt = rawImage.rectTransform;
                rt.RotateAround(rt.position, Vector3.forward, rotationAngle);

                _rotationStartPosition = currentRotationPosition;
            }
        }
        else
        {
            _isRotating = false;
        }
    }

    private void OnValidate()
    {
        RefreshMap();
    }

    public void RefreshMap()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(GetMapImage());
        }
    }
}
