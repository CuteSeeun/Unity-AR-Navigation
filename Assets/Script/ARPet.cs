using ARNavi.V;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace ARNavi.C
{
    /// <summary>
    /// ARPet 상호작용 스크립트
    /// </summary>
    public class ARPet : MonoBehaviour
    {
        private Camera arCamera;
        public Animator animator;
        // ARPet의 초기 거리
        public float initialDistance = 10f;
        public float initialRight = 1f;
        public float initialUp = 3f;

        // ARPet의 이동 속도
        public float moveSpeedMultiplier = 5f;

        // 목적지에 도착했는지 여부를 나타내는 변수
        private bool arrived = false;

        NavLine navLine;

        private void Awake()
        {
            // AR 카메라 설정
            arCamera = Camera.main;
            navLine = new NavLine();
            //애니메이션 재생
            Animator animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!arrived || !HasDestination())
            {
                FocusOn();
                Interact();
            }
        }

        /// <summary>
        /// ARPet이 화면 앞에 나옴
        /// </summary>
        void FocusOn()
        {
            // 목적지가 없는 경우에만 카메라의 위치를 따라가도록 함
            // ARPet의 위치를 항상 카메라의 위치로 설정하여 카메라의 움직임을 따라가도록 함
            transform.position = arCamera.transform.position + arCamera.transform.forward * initialDistance;
            transform.position += arCamera.transform.right * initialRight;
            transform.position += arCamera.transform.up * initialUp;

            // ARPet이 항상 카메라가 회전하는 방향을 바라보도록 함
            transform.forward = -arCamera.transform.forward;
        }

        /// <summary>
        /// 목적지로 이동하는 메서드
        /// </summary>
        public void MoveToDestination(Vector3 destination)
        {
            // 목적지까지의 거리가 일정 거리 이하일 때 도착으로 처리
            if (Vector3.Distance(transform.position, destination) < 0.1f)
            {
                arrived = true;
                return;
            }

            // 목적지 방향으로 이동
            Vector3 direction = (destination - transform.position).normalized;
            transform.position += direction * moveSpeedMultiplier * Time.deltaTime;
            // 목표를 향해 바라보도록 회전
            transform.forward = direction;
        }

        /// <summary>
        /// 도착한 목적지를 재설정하는 메서드
        /// </summary>
        public void ResetDestination()
        {
            arrived = false;
        }

        /// <summary>
        /// 목적지가 있는지 여부를 반환하는 메서드
        /// </summary>
        private bool HasDestination()
        {
            // 여기에 목적지가 있는지를 확인하는 코드를 구현해야 합니다.
            // 예를 들어, Destination 리스트가 비어 있는지를 확인하여 목적지의 유무를 판단할 수 있습니다.

            // 현재는 구현되지 않았으므로 항상 false를 반환합니다.
            return false;
        }

        /// <summary>
        /// 캐릭터 터치시 상호작용 애니메이션 작동
        /// </summary>
        void Interact()
        {
            // 사용자가 화면을 터치했을 때
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                // Ray가 캐릭터와 충돌하는지 확인
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // 충돌한 객체가 캐릭터인지 확인
                    if (hit.collider.gameObject == gameObject && animator != null)
                    {
                        animator.SetTrigger("jump");                       
                    }
                }
            }
        }

        /// <summary>
        ///  AR 내비게이션 작동시 펫이 이동하도록 설정
        /// </summary>
        public void PetMove()
        {
            // 현재 게임 오브젝트에 연결된 PlayableDirector 찾기
            PlayableDirector director = GetComponent<PlayableDirector>();
            if (director != null)
            {
                // 플레이어블 디렉터로 펫 이동하는 트랙 찾기
                TrackAsset petMoveTrack = null;

                // 플레이어블 디렉터의 트랙 수 만큼 반복
                foreach (PlayableBinding binding in director.playableAsset.outputs)
                {
                    // 트랙이 펫 이동 트랙인지 확인
                    if (binding.sourceObject is TrackAsset track && track.name == "PetMoveTrack")
                    {
                        petMoveTrack = track;
                        break;
                    }
                }

                // 펫 이동 트랙이 존재하는 경우에만 처리
                if (petMoveTrack != null)
                {
                    // 트랙에 연결된 클립 수 만큼 반복
                    foreach (TimelineClip clip in petMoveTrack.GetClips())
                    {
                        // 해당 클립의 시작 시간 및 종료 시간 가져오기
                        double startTime = clip.start;
                        double endTime = clip.end;
                        // 플레이어블 디렉터에서 해당 시간대로 플레이
                        director.time = startTime;
                        director.Evaluate();
                    }
                }
            }
        }
    }
}
