using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARNavi.V
{
    /// 위도와 경도를 나타내는 구조체 정의 
    [System.Serializable]
    public struct Vector2Double
    {
        // 위도
        public double latitude;
        // 경도
        public double longitude;
        // 생성자, 위도 경도는 더블로 나와야함
        public Vector2Double(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }

    /// <summary>
    /// 네비게이션에 화살표 표시하는 스크립트
    /// </summary>
    public class NavLine : MonoBehaviour
    {
        ARRaycastManager arManager;
        // 프리팹 할당
        public GameObject navigator;
        // 거리
        float interval = 5f;
        // 스폰 타이머(없으면 너무 많이 나옴)
        float timer = 0f;
        // 스폰 시간(없으면 너무 많이 나옴)
        float spawnCool = 1f;
        // 화살표가 지속하는 최대시간
        float arrowLifetime = 5f;
        // 화살표가 바닥으로부터 얼마나 떨어질지 결정하는 높이 오프셋
        float heightOffset = 0.03f;
        // 생성된 화살표들을 저장하는 리스트
        List<GameObject> arrows = new List<GameObject>();
        // 목적지의 경도와 위도 리스트
        public List<Vector2Double> destination = new List<Vector2Double>();
        // 다음 목적지 인덱스
        int nextDestinationIndex = 0;
        // 현재 목적지까지의 거리
        float distanceToNextDestination = float.MaxValue;
        // 도달한 목적지의 인덱스
        int reachedDestinationIndex = -1;

        /// <summary>
        /// Unity Inspector에서 사용자가 값을 설정할 수 있도록 함
        /// </summary>
        public List<Vector2Double> Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        void Start()
        {
            arManager = GetComponent<ARRaycastManager>();
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= spawnCool)
            {
                Ground();
                timer = 0f;
            }
        }

        /// <summary>
        /// 땅에서 화살표가 나타나게 하기
        /// </summary>    
        void Ground()
        {
            Vector2 screenSize = new Vector2(Screen.width / 2, Screen.height / 2);
            List<ARRaycastHit> hitinfo = new List<ARRaycastHit>();

            if (arManager.Raycast(screenSize, hitinfo, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
            {
                // 목적지 리스트에 항목이 있는지 확인
                if (destination.Count > 0)
                {
                    // 이전에 생성된 화살표들을 모두 제거
                    foreach (var arrow in arrows)
                    {
                        Destroy(arrow);
                    }
                    // 리스트 비우기
                    arrows.Clear();

                    // 바닥에 생성할 위치 계산
                    Vector3 spawnPosition = hitinfo[0].pose.position;

                    // interval 만큼 벡터 이동
                    Vector3 intervalVector = Camera.main.transform.forward * interval;

                    // 화살표를 생성하고 리스트에 추가
                    for (int i = 0; i <= 0; i++)
                    {
                        Vector3 nextPos = spawnPosition + intervalVector * i;
                        // 높이 오프셋 적용
                        nextPos.y -= heightOffset;
                        // 화살표의 방향을 설정하기 위한 회전값 계산
                        Quaternion rotation =
                            Quaternion.LookRotation(CalculateDestinationDirection
                            (hitinfo[0].pose.position, destination[nextDestinationIndex]))
                                             * Quaternion.Euler(90, 0, 0);
                        // 화살표를 생성하여 리스트에 추가
                        GameObject arrow = Instantiate(navigator, nextPos, rotation);
                        arrows.Add(arrow);
                    }

                    // 현재 위치와 다음 목적지까지의 거리 계산
                    distanceToNextDestination = Vector3.Distance(hitinfo[0].pose.position,
                        new Vector3((float)destination[nextDestinationIndex].longitude,
                        0f, (float)destination[nextDestinationIndex].latitude));

                    // 다음 목적지에 도달했을 때
                    if (distanceToNextDestination < 0.5f)
                    {
                        reachedDestinationIndex = nextDestinationIndex;
                        // 다음 목적지 인덱스 증가
                        nextDestinationIndex = (nextDestinationIndex + 1) % destination.Count;
                        // 도달한 목적지를 알리는 메시지 출력
                        Debug.Log("도착한 목적지: " + reachedDestinationIndex);
                    }

                    // 생성된 화살표들을 arrowLifetime 이후에 삭제
                    foreach (var arrow in arrows)
                    {
                        Destroy(arrow, arrowLifetime);
                    }
                }
                else
                {
                    // 목적지 리스트에 항목이 없는 경우에 대한 처리                  
                    foreach (var arrow in arrows)
                    {
                        // 생성된 화살표들을 모두 제거
                        Destroy(arrow);
                    }
                    // 리스트 비우기
                    arrows.Clear();
                    // 차후 목적지에 도착했습니다 하는 UI 만들기
                    Debug.LogWarning("목적지가 설정되지 않았습니다.");
                }
            }
        }

        // 현재 위치와 목적지 사이의 방향 벡터를 계산하는 메서드 
        Vector3 CalculateDestinationDirection(Vector3 currentPosition, Vector2Double destination)
        {
            // 목적지 위치를 Vector3로 변환
            Vector3 destinationPosition =
                new Vector3((float)destination.longitude, 0f, (float)destination.latitude);
            // 현재 위치와 목적지의 차이를 계산하여 방향 벡터를 얻음
            Vector3 difference = destinationPosition - currentPosition;
            // 방향 벡터를 정규화하여 반환
            return difference.normalized;
        }
    }
}