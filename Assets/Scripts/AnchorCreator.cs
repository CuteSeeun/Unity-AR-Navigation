using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class AnchorCreator : MonoBehaviour
{
    [SerializeField]
    GameObject m_AnchorPrefab; // 생성할 앵커 프리팹

    public GameObject AnchorPrefab // 앵커 프리팹의 프로퍼티
    {
        get => m_AnchorPrefab;
        set => m_AnchorPrefab = value;
    }

    List<ARAnchor> m_AnchorPoints; // 생성된 앵커들을 저장할 리스트

    ARRaycastManager m_RaycastManager; // AR 레이캐스트 매니저
    ARAnchorManager m_AnchorManager; // AR 앵커 매니저
    ARPlaneManager m_PlaneManager; // AR 평면 매니저

    void Awake()
    {
        // 필요한 컴포넌트들을 가져옴
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();

        m_AnchorPoints = new List<ARAnchor>(); // 앵커 리스트 초기화
    }

    void Update()
    {
        // 터치 입력이 없으면 리턴
        if (Input.touchCount == 0)
            return;

        var touch = Input.GetTouch(0);

        // 터치가 시작되지 않았으면 리턴
        if (touch.phase != TouchPhase.Began)
            return;

        // 평면을 감지하고 있으면
        if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // 첫 번째 감지된 평면의 포즈, 트래커블 ID를 가져옴
            var hitPose = s_Hits[0].pose;
            var hitTrackableId = s_Hits[0].trackableId;

            // 트래커블 ID를 이용해 평면 정보를 가져옴
            var hitPlane = m_PlaneManager.GetPlane(hitTrackableId);

            // 평면에 앵커를 부착하고 앵커 객체를 가져옴
            var anchor = m_AnchorManager.AttachAnchor(hitPlane, hitPose);

            // 앵커 프리팹을 앵커 아래에 인스턴스화
            Instantiate(m_AnchorPrefab, anchor.transform);

            // 앵커 부착에 실패하면 로그를 출력하고, 성공하면 앵커 리스트에 추가
            if (anchor == null)
            {
                Debug.Log("Error creating anchor.");
            }
            else
            {
                m_AnchorPoints.Add(anchor);
            }
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>(); // AR 레이캐스트 히트를 저장할 리스트

    // 모든 앵커 제거
    public void RemoveAllAnchors()
    {
        foreach (var anchor in m_AnchorPoints)
        {
            Destroy(anchor); // 앵커 제거
        }
        m_AnchorPoints.Clear(); // 앵커 리스트 초기화
    }
}
