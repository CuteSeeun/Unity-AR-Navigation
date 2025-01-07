using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum TMapColor
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
public class TMapLocation
{
    public float latitude;
    public float longitude;
}

[Serializable]
public class TMapMarker
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
public class TMapPath
{
    public int weight = 5;
    public TMapColor color;
    public bool fill = false;
    public TMapColor fillColor;
    public List<TMapLocation> locations = new List<TMapLocation>();
}

public class TMap : MonoBehaviour
{
    public RawImage rawImage;
    Animator animator;

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
    public float _minDistanceBetweenFingers = 50f;

    string appKey = "D4jfdqypAc4VA1iGdDH6R2rU519L6FfD4cxRQgmD";
    string coordType = "WGS84GEO";
    string format = "PNG";
    string markers = "126.987038,37.565207";

    bool overlap = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
        // 터치 입력 처리
        if (Input.touchCount > 0 && !overlap)
        {
            HandleTouchInput();
        }
    }

    //겹침상태 온
    public void Overlaps()
    {
        overlap = true;
    }
    //겹침상태 오프D
    public void UnOverlaps()
    {
        overlap = false;
    }

    /// <summary>
    ///  터치 및 줌 인 아웃
    /// </summary>
    private void HandleTouchInput()
    {
        // 스와이프
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            // 터치된 위치가 RawImage 안에 있는지 확인
            if (RectTransformUtility.RectangleContainsScreenPoint(rawImage.rectTransform, touch.position))
            {
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
        }
        //줌 인 ? 줌 아웃
        else if (Input.touchCount >= 2)
        {
            Touch firstTouch = Input.touches[0];
            Touch secondTouch = Input.touches[1];
            // 양 손가락 모두 RawImage 안에 있는지 확인
            if (RectTransformUtility.RectangleContainsScreenPoint(rawImage.rectTransform, firstTouch.position)
                && RectTransformUtility.RectangleContainsScreenPoint(rawImage.rectTransform, secondTouch.position))
            {
                if (firstTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Moved)
                {
                    float touchDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
                    float touchDeltaDistance = (touchDistance - Vector2.Distance(firstTouch.position
                        - firstTouch.deltaPosition, secondTouch.position - secondTouch.deltaPosition)) * _zoomSpeed * Time.deltaTime;
                    _zoom = Mathf.Clamp(_zoom + Mathf.RoundToInt(touchDeltaDistance), 1, 20);

                    RefreshMap();
                }
            }
        }
    }

    public void SearchWebView()
    {
        animator.SetBool("extend", false);
    }

    public void ExtendWebView()
    {
        animator.SetBool("extend", true);
    }
   
    public void ResetWebView() 
    {
        // 가로세로 크기를 원래 크기로 설정
        rawImage.rectTransform.sizeDelta = new Vector2(2048, 2048);

        // 위치를 (0, 0)으로 이동
        rawImage.rectTransform.localPosition = Vector3.zero;
    }

    public void RefreshMap()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(GetMapImage());
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        RefreshMap();
    }
#endif
  
}