using ARNavi.C;
using ARNavi.M;
using UnityEngine;

namespace ARNavi.V
{
    /// <summary>
    /// ARPet 선택하는 스크립트
    /// </summary>
    public class PetSelector : MonoBehaviour
    {
        // 캐릭터들의 배열
        public PetDatabase petDB;
        public GameObject petimages;
        // 스와이프 동작을 감지하기 위한 최소 이동량
        public float swipeThreshold = 50f;
        // 터치 시작 위치
        private Vector2 touchStartPosition;
        // 현재 pet의 인덱스
        private int selectPet = 0;

        private GameObject _pet;
        public GameObject initialUI;

        private void Start()
        {
            if (!PlayerPrefs.HasKey("selectPet"))
            {
                selectPet = 0;
            }
            else
            {
                Load();
            }
            UpdatePet(selectPet);

            TutorialUi();
        }

        /// <summary>
        /// 스와이프로 AR 펫 변경
        /// </summary>
        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPosition = touch.position;
                        break;

                    case TouchPhase.Ended:
                        float swipeDistance = touch.position.x - touchStartPosition.x;
                        // 스와이프 방향에 따라 캐릭터 변경
                        if (Mathf.Abs(swipeDistance) > swipeThreshold)
                        {
                            // 오른쪽으로 스와이프
                            if (swipeDistance > 0) 
                            {
                                NextPet();
                            }
                            // 왼쪽으로 스와이프
                            else if (swipeDistance < 0)
                            {
                                PrevPet();
                            }
                        }
                        break;

                    default:
                        break;

                }
            }
        }

        /// <summary>
        /// 듀토리얼 UI
        /// </summary>
        public void TutorialUi()
        {
            // PlayerPrefs에서 "FirstRun" 키의 값을 가져옵니다.
            bool isFirstRun = PlayerPrefs.GetInt("FirstRun", 1) == 1;

            // isFirstRun이 true이면 처음 실행되는 것이므로 UI를 활성화합니다.
            if (isFirstRun)
            {
                initialUI.SetActive(true);
                // 이제 앱이 처음 실행되었음을 나타내는 값을 false로 설정하여 다음 실행 때 UI가 나타나지 않도록 합니다.
                PlayerPrefs.SetInt("FirstRun", 0);
                // 변경된 PlayerPrefs 저장
                PlayerPrefs.Save(); 
            }
            else
            {
                // 처음 실행되는 것이 아니므로 UI를 비활성화합니다.
                initialUI.SetActive(false);
            }
        }

        /// <summary>
        /// 다음 펫 전환
        /// </summary>
        public void NextPet()
        {
            selectPet++;

            if (selectPet >= petDB.PetCount)
            {
                selectPet = 0;
            }
            UpdatePet(selectPet);
            Save();
        }

        /// <summary>
        /// 이전 팻 전환
        /// </summary>
        public void PrevPet()
        {
            selectPet--;
            if (selectPet < 0)
            {
                selectPet = petDB.PetCount - 1;
            }
            UpdatePet(selectPet);
            Save();
        }

        /// <summary>
        /// ARPet 불러오기
        /// </summary>
        /// <param name="selectPet"></param>
        private void UpdatePet(int selectPet)
        {
            Pet pet = petDB.GetPet(selectPet);
            petimages = pet.PetImage;

            if (pet != null) Destroy(_pet);
            _pet = GameObject.Instantiate(petimages, Vector3.zero, Quaternion.identity);

            // z 축을 1로 설정합니다.
            Vector3 position = _pet.transform.position;
            position.y = -2f;
            position.z = -1f;
            _pet.transform.position = position;
            // 각도를 180도로 회전합니다.
            _pet.transform.Rotate(Vector3.up, 180f);
        }

        /// <summary>
        /// 현재 ARPet 활성화
        /// </summary>
        private void Load()
        {
            selectPet = PlayerPrefs.GetInt("selectPet");
        }

        /// <summary>
        /// ARPet 저장
        /// </summary>
        private void Save()
        {
            PlayerPrefs.SetInt("selectPet", selectPet);
        }
    }
}
