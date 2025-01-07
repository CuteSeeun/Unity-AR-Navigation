//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Android;

//public class GPSData : MonoBehaviour
//{
//    private float _maxWaitTime = 10.0f;
//    private float _waitTime = 0;

//    public float _longitude;
//    public float _latitude;


//    void Start()
//    {
//        StartCoroutine(AccessLocation());
//        Debug.Log("시작했니?");
//    }

//    /// <summary>
//    /// Request to access location data on Android
//    /// </summary>
//    /// <returns></returns>
//    IEnumerator AccessLocation()
//    {
//        Debug.Log("코루틴 시작");
//        // 2초 후 위치접근허용 팝업
//        yield return new WaitForSeconds(2f);

//        // 위치접근 권한이 없으면
//        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
//        {
//            // Debug.Log(LocationServiceStatus.Running.ToString());

//            // 권한 요청
//            Permission.RequestUserPermission(Permission.FineLocation);

//            Debug.Log("권한 요청");

//            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
//            {
//                yield return null;
//            }
//        }

//        //위치 데이터를 요청 -> 수신 대기
//        Input.location.Start();

//        //GPS 수신 상태가 초기 상태에서 일정 시간 동안 대기함
//        while (Input.location.status == LocationServiceStatus.Initializing && _waitTime < _maxWaitTime)
//        {
//            yield return new WaitForSeconds(1.0f);
//            _waitTime++;
//            Debug.Log("수신 대기");

//        }

//        //수신 실패 시 수신이 실패됐다는 것을 출력
//        if (Input.location.status == LocationServiceStatus.Failed)
//        {
//            Debug.LogError("위치 정보 수신 실패");
//        }

//        // 위치정보 수신 가능
//        if (Input.location.isEnabledByUser)
//        {
//            Debug.Log("수신 가능 + gps 데이터 수신 시작");

//            UpdateGpsData();
//        }

//        yield break;
//    }


//    /// <summary>
//    /// Update the GPS data on Android
//    /// </summary>
//    /// <returns></returns>
//    void UpdateGpsData()
//    {
//        Debug.Log("위치 정보 수신!");

//        LocationInfo gpsData = Input.location.lastData;

//        while (true)
//        {
//            // gps 값 갱신
//            _longitude = gpsData.longitude;
//            _latitude = gpsData.latitude;


//            double longi = gpsData.longitude;

//            double longi2 = gpsData.latitude * 1.0d;

//            Debug.Log($"경도 float" + _longitude);
//            Debug.Log($"경도 float" + longi);
//            Debug.Log($"경도 float" + longi2);
//        }
//    }


//}

using Google.XR.ARCoreExtensions.GeospatialCreator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class GPSData : MonoBehaviour
{

    public static double first_Lat; //최초 위도
    public static double first_Long; //최초 경도
    public static double current_Lat; //현재 위도
    public static double current_Long; //현재 경도

    private static WaitForSeconds second;

    private static bool gpsStarted = false;

    private static LocationInfo location;

    public List<GameObject> Anchor = new List<GameObject>();

    private List<double> anchorLongitude = new List<double>();
    private List<double> anchorLatitude = new List<double>();

    private Canvas arrivePopUp;

    private void Awake()
    {
        second = new WaitForSeconds(1.0f);
    }

    private void Start()
    {

        for(int i = 0; i < Anchor.Count; i++)
        {
            anchorLongitude.Add(Anchor[i].GetComponent<ARGeospatialCreatorAnchor>().Longitude);
            anchorLatitude.Add(Anchor[i].GetComponent<ARGeospatialCreatorAnchor>().Latitude);
        }

        arrivePopUp = GameObject.Find("Canvas-popUp").GetComponent<Canvas>();

        StartCoroutine(AccessGpsData());
    }

    IEnumerator AccessGpsData()
    {
        // 유저가 GPS 사용중인지 최초 체크
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS is not enabled");

            // 권한 요청
            Permission.RequestUserPermission(Permission.FineLocation);

            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }

        }

        //GPS 서비스 시작
        Input.location.Start();
        Debug.Log("Awaiting initialization");

        //활성화될 때 까지 대기
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return second;
            maxWait -= 1;
        }

        //20초 지날경우 활성화 중단
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
        }

        //연결 실패
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
        }
        else
        {
            //접근 허가됨, 최초 위치 정보 받아오기
            location = Input.location.lastData;
            first_Lat = location.latitude * 1.0d;
            first_Long = location.longitude * 1.0d;
            gpsStarted = true;

            //현재 위치 갱신
            while (gpsStarted)
            {
                location = Input.location.lastData;
                current_Lat = location.latitude * 1.0d;
                current_Long = location.longitude * 1.0d;

                for(int i = 0; i < Anchor.Count; i++)
                {
                    // 현재 위치와 앵커 위치 간의 거리 계산
                    double remainDistance = distance(current_Lat, current_Long, anchorLatitude[i], anchorLongitude[i]);

                    // 목표 위치에 근접하면
                    if(remainDistance <= 20f)
                    {
                        // 해당 앵커 활성화
                        Anchor[i].SetActive(true);
                        Anchor[i+1].SetActive(true);

                        if(i == Anchor.Count-1)
                        {
                            arrivePopUp.enabled = true;
                        }
                    }

                    else if(remainDistance >= 50f)
                    {
                        // 앵커와 거리가 멀면 해당 앵커 비활성화
                        Anchor[i].SetActive(false);
                    }
                }

                Debug.Log("위도 : "+current_Lat);
                Debug.Log("경도 : "+current_Long);

                yield return second;
            }
        }
    }

    // 두 지점 간의 거리를 계산하는 함수 (미터 단위)
    private double distance(double lat1, double lon1, double lat2, double lon2)
    {
        // 두 지점 간의 각도 차이 계산
        double theta = lon1 - lon2;

        // 위도 및 경도를 라디안 값으로 변환하여 거리 계산
        double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) +
                      Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));

        // 거리를 각도로 변환
        dist = Math.Acos(dist);

        // 라디안 값을 각도로 변환
        dist = Rad2Deg(dist);

        // 해상도 단위(m)로 변환
        dist = dist * 60 * 1.1515;

        // 미터로 변환
        dist = dist * 1609.344;

        return dist;
    }

    // 각도를 라디안으로 변환하는 함수
    private double Deg2Rad(double deg)
    {
        return (deg * Mathf.PI / 180.0f);
    }

    // 라디안을 각도로 변환하는 함수
    private double Rad2Deg(double rad)
    {
        return (rad * 180.0f / Mathf.PI);
    }

}
