using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking; //UnityWebRequest 클래스 사용하여 HTTP요청 보내기. 그래서 스태틱 맵 받아오기.
using UnityEngine.UI; //UI컴포넌트를 사용하기 위해 추가.

#region enum
/// <summary>
/// T Maps 마커 색상
/// </summary>
public enum MiniMapColor
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

#region 설정 클래스
/// <summary>
/// T Map에서 사용되는 위치를 정의
/// </summary>
[Serializable]
public class MiniMapLocation
{
    public float latitude; // 위도
    public float longitude; // 경도
}

/// <summary>
/// T GetMapImage 마커 정의
/// </summary>
[Serializable]
public class MiniMapMarker
{
    // 마커 크기
    public enum TMapMarkerSize
    {
        Tiny,
        Small,
        Mid
    }
    public TMapMarkerSize size; // 마커 크기
    public TMapColor color; // 마커 색상
    public string label; // 마커 라벨
    public TMapLocation[] locations; // 마커 위치 배열
}

/// <summary>
/// T GetMapImage 경로를 정의
/// </summary>
[Serializable]
public class MiniMapPath
{
    public int weight = 5; // 경로 선 두께
    public TMapColor color; // 경로 선 색상
    public bool fill = false; // 경로 선 채우기 여부
    public TMapColor fillColor; // 경로 선 채우기 색상
    //public TMapLocation[] locations;
    public List<TMapLocation> locations = new List<TMapLocation>(); // 경로 위치 리스트
}
#endregion

public class MiniMap : MonoBehaviour, IPointerClickHandler
{

    public RawImage rawImage;
    [Range(0f, 1f)]
    public float _transparency = 1f; // 투명도
    public TMapLocation centerLocation; // 중심 위치    

    [Range(1, 18)]
    public int _zoom = 14; // 지도 줌 레벨
    private int _mapWidth = 512; // 지도 이미지 너비
    private int _mapHeight = 512; // 지도 이미지 높이

    public MapType mapType = MapType.RoadMap; // 지도 유형
    public int _mapScale = 1; // 지도 크기

    public TMapColor markerColor = TMapColor.red; // 마커 색상
    public char _label = 'C'; // 마커 라벨

    public float _mapCenterLongitude;
    public float _mapCenterLatitude;

    private string _url;
    private UnityEngine.Color _rawImageColor = UnityEngine.Color.white;
    public bool _IsLocaeCenter = false;

    string appKey = "D4jfdqypAc4VA1iGdDH6R2rU519L6FfD4cxRQgmD"; // TMap AppKey
    string coordType = "WGS84GEO";
    string format = "PNG";
    string markers = "126.987038,37.565207";
    // public TMapMarker[] markers;

    public MiniMap miniMap;
    public float maxZoom = 2.0f;
    public float minZoom = 1.0f;

    private bool isTouching = false;
    private Vector2 touchStartPosition;

    /// <summary>
    /// UnityWebRequest를 사용하여 url로부터 이미지 받아오기.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>

    public void Start()
    {
        StartCoroutine(GetMapImage()); // 코루틴 시작
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

    public void OnPointerClick(PointerEventData eventData)
    {
        float currentScale = rawImage.rectTransform.localScale.x;

        if (Mathf.Approximately(currentScale, 1.0f))
        {
            // 스케일을 2배로 변경
            rawImage.rectTransform.localScale = new Vector3(2f, 2f, 1f);
            // 이미지 위치를 중앙으로 이동
            rawImage.rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            // 스케일을 1배로 변경
            rawImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            // 이미지 위치를 원위치
            rawImage.rectTransform.anchoredPosition = new Vector2(0, -640);
        }

        // 맵 새로고침
        RefreshMap();
    }
}