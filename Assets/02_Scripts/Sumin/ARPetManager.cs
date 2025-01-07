using UnityEngine;

namespace ARNavi.Sumin
{
    /// <summary>
    /// ���ξ� ARPet ����
    /// </summary>
    public class ARPetManager : MonoBehaviour
    {
        // ĳ���͵��� �迭
        public PetDataBase petDB;
        public GameObject petimages;
        // ���� pet�� �ε���
        private int selectPet = 0;
        private GameObject _pet;

        // �� Ȱ��ȭ ���� ����
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
        /// ARPet �ҷ�����
        /// </summary>
        /// <param name="selectPet"></param>
        private void UpdatePet(int selectPet)
        {
            Pet pet = petDB.GetPet(selectPet);
            petimages = pet.PetImage;
            if (pet != null) Destroy(_pet);
            _pet = GameObject.Instantiate(petimages, Vector3.zero, Quaternion.identity);
            // z ���� 10�� �����մϴ�.
            Vector3 position = _pet.transform.position;
            position.z = 10f;
            _pet.transform.position = position;
            // ������ 180���� ȸ���մϴ�.
            _pet.transform.Rotate(Vector3.up, 180f);
        }

        /// <summary>
        /// ���� ARPet Ȱ��ȭ
        /// </summary>
        private void Load()
        {
            selectPet = PlayerPrefs.GetInt("selectPet");
        }

        /// <summary>
        /// �� Ȱ��ȭ ����
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
        /// �� ��Ȱ��ȭ
        /// </summary>
        private void DisablePet()
        {
            Destroy(_pet);
        }
    }
}
