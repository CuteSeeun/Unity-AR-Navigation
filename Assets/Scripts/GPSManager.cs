using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GPSManager : MonoBehaviour
{
    public Text text_ui;             // UI 텍스트 요소를 연결할 변수
    public GameObject welcome_popUp; // 환영 팝업 창을 연결할 변수
    public bool isFirst = false;     // 도착 팝업을 한 번만 표시하기 위한 플래그

    public double[] lats;            // 목표지점의 위도 배열
    public double[] longs;           // 목표지점의 경도 배열

    IEnumerator Start()
    {
        // 위치 권한이 부여되지 않았을 경우 권한 요청
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            yield return null;
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        // 위치 서비스가 사용자에 의해 활성화되어 있는지 확인
        if (!Input.location.isEnabledByUser)
            yield break;

        // 위치 서비스 시작 (정확성 10m, 갱신 주기 1초)
        Input.location.Start(10, 1);

        int maxWait = 20;
        // 위치 서비스 초기화 대기
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // 위치 서비스 초기화 타임아웃
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // 위치 서비스 초기화 실패
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // 위치 정보 표시
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

            // 위치 정보를 UI에 업데이트하는 무한 루프
            while (true)
            {
                yield return null;
                text_ui.text = Input.location.lastData.latitude + " / " + Input.location.lastData.longitude;
            }
        }

        // Input.location.Stop(); // 위치 서비스 중지 (필요시 주석 해제)
    }

    void Update()
    {
        // 위치 서비스가 실행 중이고, 유효한 위치 데이터가 있는 경우
        if (Input.location.status == LocationServiceStatus.Running)
        {
            double myLat = Input.location.lastData.latitude;
            double myLong = Input.location.lastData.longitude;

            // 현재 위치와 목표 위치 간의 거리 계산
            double remainDistance = distance(myLat, myLong, lats[0], longs[0]);

            // 목표 위치에 근접하면
            if (remainDistance <= 10f)  // 범위를 10m로 설정
            {
                // 도착 팝업을 한 번만 표시
                if (!isFirst)
                {
                    isFirst = true;
                    welcome_popUp.SetActive(true); // 환영 팝업 활성화
                }
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
