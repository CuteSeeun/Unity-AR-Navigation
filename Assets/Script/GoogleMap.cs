using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#region enum
public enum MapType
{
    RoadMap,
    Satellite,
    Terrain,
    Hybrid
}

public enum GoogleMapColor
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
#endregion

#region 설정 클레스
[Serializable]
public class GoogleMapLocation
{
    public string address;
    public double latitude;
    public double longitude;
}

[Serializable]
public class GoogleMapMarker
{
    public enum GoogleMapMarkerSize
    {
        Tiny,
        Small,
        Mid
    }
    public GoogleMapMarkerSize size;
    public GoogleMapColor color;
    public string label;
    public GoogleMapLocation[] locations;

}

[Serializable]
public class GoogleMapPath
{
    public int weight = 5;
    public GoogleMapColor color;
    public bool fill = false;
    public GoogleMapColor fillColor;
    public GoogleMapLocation[] locations;
}
#endregion

/// <summary>
/// 구글 맵
/// </summary>
public class GoogleMap : MonoBehaviour
{
    //맵 넣을 이미지
    public RawImage rawImage;
    [Range(0f, 1f)]
    public float transparency = 1f;
    //현재 위치
    public GoogleMapLocation centerLocation;
    //맵 줌(1~20까지)
    [Range(1, 20)]
    public int mapZoom = 14;
    //맵 넓이

    public int mapWidth = 1920;
    public int mapHeight = 1920;
    //맵 타입
    public MapType mapType = MapType.RoadMap;
    //맵 크기
    [Range(1, 4)]
    public int scale = 4;

    // 이동 속도
    public float moveSpeed = 0.001f;
    // 조정된 이동 속도
    private float adjustedSpeed; 
    // 줌 속도
    private float zoomSpeed = 0.1f;
    // 맵 회전 속도
    public float rotationSpeed = 1f;

    // 각 손가락 간의 최소 거리
    public float minDistanceBetweenFingers = 50f;

    private bool isRotating = false;
    private Vector2 rotationStartPosition;

    public enum MarkerSize
    {
        tiny, mid, small,
    }
    public MarkerSize markerSize = MarkerSize.mid;

    public GoogleMapColor markerColor = GoogleMapColor.blue;
    public char label = 'C';

    public string apiKey;

    private UnityEngine.Color rawImageColor = UnityEngine.Color.white;
    public bool LocateCenter = false;
    public GoogleMapMarker[] markers;
    public GoogleMapPath[] paths;


    IEnumerator Map()
    {
        // 맵 이미지 초기화 및 투명도 설정
        rawImage.color = new Color(1f, 1f, 1f, transparency);

        string url = "https://maps.googleapis.com/maps/api/staticmap";
        string qs = "";
        if (!LocateCenter)
        {
            if (centerLocation.address != "")
                qs += "center=" + WWW.UnEscapeURL(centerLocation.address);
            else
                qs += "center=" + WWW.UnEscapeURL(string.Format("{0},{1}", centerLocation.latitude, centerLocation.longitude));

            qs += "&zoom=" + mapZoom.ToString();
        }
        qs += "&size=" + WWW.UnEscapeURL(string.Format("{0}x{0}", mapWidth));
        qs += "&scale=" + scale;
        qs += "&maptype=" + mapType.ToString().ToLower();
        var usingSensor = false;

        qs += "&sensor=" + (usingSensor ? "true" : "false");

        foreach (var i in markers)
        {
            qs += "&markers=" + string.Format("size:{0}|color:{1}|label:{2}", i.size.ToString().ToLower(), i.color, i.label);

            foreach (var loc in i.locations)
            {
                if (loc.address != "")
                    qs += "|" + WWW.UnEscapeURL(loc.address);
                else
                    qs += "|" + WWW.UnEscapeURL(string.Format("{0},{1}", loc.latitude, loc.longitude));
            }
        }

        foreach (var i in paths)
        {
            qs += "&path=" + string.Format("weight:{0}|color:{1}", i.weight, i.color);

            if (i.fill)
                qs += "|fillcolor:" + i.fillColor;

            foreach (var loc in i.locations)
            {
                if (loc.address != "")
                    qs += "|" + WWW.UnEscapeURL(loc.address);
                else
                    qs += "|" + WWW.UnEscapeURL(string.Format("{0},{1}", loc.latitude, loc.longitude));
            }
        }

        qs += "&key=" + WWW.UnEscapeURL(apiKey);

        WWW req = new WWW(url + "?" + qs);

        yield return req;

        rawImage.texture = req.texture;
        rawImage.SetNativeSize();
    }

    public void RefreshMap()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(Map());
        }
    }

    private void Reset()
    {
        rawImage = gameObject.GetComponentInChildren<RawImage>();
        // scaleFactor를 설정하여 이미지의 크기를 늘림
        rawImage.rectTransform.localScale = new Vector3(2f, 2f, 1f);
        // 맵을 다시 그림
        RefreshMap();
    }

    private void Start()
    {
        Reset();
        // 조정된 이동 속도를 설정합니다.
        AdjustMoveSpeed();
    }

    private void Update()
    {
        // 안드로이드에서 터치 이벤트를 감지하여 지도 확대/축소
        if (Application.platform == RuntimePlatform.Android)
        {
            HandleTouchInput();
        }
    }

    // 줌 레벨에 따라 이동 속도 조정
    private void AdjustMoveSpeed()
    {
        // 최소 줌 레벨과 최대 줌 레벨을 설정합니다.
        int minZoomLevel = 1;
        int maxZoomLevel = 20;

        // 현재 줌 레벨을 1에서 20 사이의 값으로 클램핑합니다.
        int clampedZoom = Mathf.Clamp(mapZoom, minZoomLevel, maxZoomLevel);

        // 줌 레벨에 따라 이동 속도를 조정합니다.
        // 최소 속도와 최대 속도를 설정하여 사용할 수 있습니다.
        float minSpeed = 0.0001f;
        float maxSpeed = 0.1f;

        // 현재 줌 레벨에 따라 조정된 이동 속도를 계산합니다.
        adjustedSpeed = Mathf.Lerp(maxSpeed, minSpeed, (float)(clampedZoom - minZoomLevel) / (maxZoomLevel - minZoomLevel));
    }


    /// <summary>
    /// 맵 상호작용 (스와이프, 줌, 회전)
    /// </summary>
    private void HandleTouchInput()
    {
        // 스와이프
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = touch.deltaPosition;
                Vector2 moveDirection = Quaternion.Euler(0, 0, -rawImage.rectTransform.localEulerAngles.z) * touchDeltaPosition;
                float moveX = -moveDirection.x * adjustedSpeed * Time.deltaTime;
                float moveY = -moveDirection.y * adjustedSpeed * Time.deltaTime;

                centerLocation.latitude += moveY;
                centerLocation.longitude += moveX;

                RefreshMap();
            }
        }
        
        else if (Input.touchCount >= 2)
        {
            Touch firstTouch = Input.touches[0];
            Touch secondTouch = Input.touches[1];
            // 줌 인/아웃
            if (firstTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Moved)
            {
                float touchDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
                float touchDeltaDistance = (touchDistance - Vector2.Distance(firstTouch.position
                    - firstTouch.deltaPosition, secondTouch.position - secondTouch.deltaPosition)) * zoomSpeed * Time.deltaTime;
                mapZoom = Mathf.Clamp(mapZoom + Mathf.RoundToInt(touchDeltaDistance), 1, 20);

                RefreshMap();
            }

            //로테이트
            if (!isRotating)
            {
                rotationStartPosition = (firstTouch.position + secondTouch.position) / 2f;
                isRotating = true;
            }

            if (isRotating && (firstTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Moved))
            {
                Vector2 currentRotationPosition = (firstTouch.position + secondTouch.position) / 2f;
                Vector2 rotationVector = currentRotationPosition - rotationStartPosition;
                float rotationAngle = Vector2.SignedAngle(rotationVector, Vector2.right) * rotationSpeed * Time.deltaTime;

                RectTransform rt = rawImage.rectTransform;
                rt.RotateAround(rt.position, Vector3.forward, rotationAngle);

                rotationStartPosition = currentRotationPosition;
            }
        }
        else
        {
            isRotating = false;
        }
    }
   


#if UNITY_EDITOR
    private void OnValidate()
    {
        RefreshMap();
    }
#endif
}