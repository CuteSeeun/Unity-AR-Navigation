using UnityEngine;

namespace ARNavi.Sumin
{
    /// <summary>
    /// 메인씬 ARPet 관리
    /// </summary>
    public class ARPetManager : MonoBehaviour
    {
        // 캐릭터들의 배열
        public PetDataBase petDB;
        public GameObject petimages;
        // 현재 pet의 인덱스
        private int selectPet = 0;
        private GameObject _pet;

        // 펫 활성화 상태 변수
        private bool isPetActive = true;

        void Start()
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
            // z 축을 10로 설정합니다.
            Vector3 position = _pet.transform.position;
            position.z = 10f;
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
        /// 팻 활성화 유무
        /// </summary>
        public void PetOption()
        {
            isPetActive = !isPetActive;
            if (isPetActive)
            {
                UpdatePet(selectPet);
            }
            else
            {
                DisablePet();
            }

        }

        /// <summary>
        /// 팻 비활성화
        /// </summary>
        private void DisablePet()
        {
            Destroy(_pet);
        }
    }
}
