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
//        Debug.Log("�����ߴ�?");
//    }

//    /// <summary>
//    /// Request to access location data on Android
//    /// </summary>
//    /// <returns></returns>
//    IEnumerator AccessLocation()
//    {
//        Debug.Log("�ڷ�ƾ ����");
//        // 2�� �� ��ġ������� �˾�
//        yield return new WaitForSeconds(2f);

//        // ��ġ���� ������ ������
//        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
//        {
//            // Debug.Log(LocationServiceStatus.Running.ToString());

//            // ���� ��û
//            Permission.RequestUserPermission(Permission.FineLocation);

//            Debug.Log("���� ��û");

//            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
//            {
//                yield return null;
//            }
//        }

//        //��ġ �����͸� ��û -> ���� ���
//        Input.location.Start();

//        //GPS ���� ���°� �ʱ� ���¿��� ���� �ð� ���� �����
//        while (Input.location.status == LocationServiceStatus.Initializing && _waitTime < _maxWaitTime)
//        {
//            yield return new WaitForSeconds(1.0f);
//            _waitTime++;
//            Debug.Log("���� ���");

//        }

//        //���� ���� �� ������ ���еƴٴ� ���� ���
//        if (Input.location.status == LocationServiceStatus.Failed)
//        {
//            Debug.LogError("��ġ ���� ���� ����");
//        }

//        // ��ġ���� ���� ����
//        if (Input.location.isEnabledByUser)
//        {
//            Debug.Log("���� ���� + gps ������ ���� ����");

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
//        Debug.Log("��ġ ���� ����!");

//        LocationInfo gpsData = Input.location.lastData;

//        while (true)
//        {
//            // gps �� ����
//            _longitude = gpsData.longitude;
//            _latitude = gpsData.latitude;


//            double longi = gpsData.longitude;

//            double longi2 = gpsData.latitude * 1.0d;

//            Debug.Log($"�浵 float" + _longitude);
//            Debug.Log($"�浵 float" + longi);
//            Debug.Log($"�浵 float" + longi2);
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

    public static double first_Lat; //���� ����
    public static double first_Long; //���� �浵
    public static double current_Lat; //���� ����
    public static double current_Long; //���� �浵

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
        // ������ GPS ��������� ���� üũ
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS is not enabled");

            // ���� ��û
            Permission.RequestUserPermission(Permission.FineLocation);

            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }

        }

        //GPS ���� ����
        Input.location.Start();
        Debug.Log("Awaiting initialization");

        //Ȱ��ȭ�� �� ���� ���
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return second;
            maxWait -= 1;
        }

        //20�� ������� Ȱ��ȭ �ߴ�
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
        }

        //���� ����
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
        }
        else
        {
            //���� �㰡��, ���� ��ġ ���� �޾ƿ���
            location = Input.location.lastData;
            first_Lat = location.latitude * 1.0d;
            first_Long = location.longitude * 1.0d;
            gpsStarted = true;

            //���� ��ġ ����
            while (gpsStarted)
            {
                location = Input.location.lastData;
                current_Lat = location.latitude * 1.0d;
                current_Long = location.longitude * 1.0d;

                for(int i = 0; i < Anchor.Count; i++)
                {
                    // ���� ��ġ�� ��Ŀ ��ġ ���� �Ÿ� ���
                    double remainDistance = distance(current_Lat, current_Long, anchorLatitude[i], anchorLongitude[i]);

                    // ��ǥ ��ġ�� �����ϸ�
                    if(remainDistance <= 20f)
                    {
                        // �ش� ��Ŀ Ȱ��ȭ
                        Anchor[i].SetActive(true);
                        Anchor[i+1].SetActive(true);

                        if(i == Anchor.Count-1)
                        {
                            arrivePopUp.enabled = true;
                        }
                    }

                    else if(remainDistance >= 50f)
                    {
                        // ��Ŀ�� �Ÿ��� �ָ� �ش� ��Ŀ ��Ȱ��ȭ
                        Anchor[i].SetActive(false);
                    }
                }

                Debug.Log("���� : "+current_Lat);
                Debug.Log("�浵 : "+current_Long);

                yield return second;
            }
        }
    }

    // �� ���� ���� �Ÿ��� ����ϴ� �Լ� (���� ����)
    private double distance(double lat1, double lon1, double lat2, double lon2)
    {
        // �� ���� ���� ���� ���� ���
        double theta = lon1 - lon2;

        // ���� �� �浵�� ���� ������ ��ȯ�Ͽ� �Ÿ� ���
        double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) +
                      Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));

        // �Ÿ��� ������ ��ȯ
        dist = Math.Acos(dist);

        // ���� ���� ������ ��ȯ
        dist = Rad2Deg(dist);

        // �ػ� ����(m)�� ��ȯ
        dist = dist * 60 * 1.1515;

        // ���ͷ� ��ȯ
        dist = dist * 1609.344;

        return dist;
    }

    // ������ �������� ��ȯ�ϴ� �Լ�
    private double Deg2Rad(double deg)
    {
        return (deg * Mathf.PI / 180.0f);
    }

    // ������ ������ ��ȯ�ϴ� �Լ�
    private double Rad2Deg(double rad)
    {
        return (rad * 180.0f / Mathf.PI);
    }

}
