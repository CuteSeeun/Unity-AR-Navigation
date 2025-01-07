using UnityEngine;

namespace ARNavi.Sumin
{
    /// <summary>
    /// ARPet �����ϴ� ��ũ��Ʈ
    /// </summary>
    public class PetSelector : MonoBehaviour
    {
        // ĳ���͵��� �迭
        public PetDataBase petDB;
        public GameObject petimages;
        // �������� ������ �����ϱ� ���� �ּ� �̵���
        public float swipeThreshold = 50f;
        // ��ġ ���� ��ġ
        private Vector2 touchStartPosition;
        // ���� pet�� �ε���
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
        /// ���������� AR �� ����
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

                        // �������� ���⿡ ���� ĳ���� ����
                        if (Mathf.Abs(swipeDistance) > swipeThreshold)
                        {
                            // ���������� ��������
                            if (swipeDistance > 0)
                            {
                                PrevPet();
                            }
                            // �������� ��������
                            else if (swipeDistance < 0)
                            {
                                NextPet();
                            }
                        }
                        break;

                    default:
                        break;

                }
            }
        }

        /// <summary>
        /// ���丮�� UI
        /// </summary>
        public void TutorialUi()
        {
            // PlayerPrefs���� "FirstRun" Ű�� ���� �����ɴϴ�.
            bool isFirstRun = PlayerPrefs.GetInt("FirstRun", 1) == 1;

            // isFirstRun�� true�̸� ó�� ����Ǵ� ���̹Ƿ� UI�� Ȱ��ȭ�մϴ�.
            if (isFirstRun)
            {
                initialUI.SetActive(true);
                // ���� ���� ó�� ����Ǿ����� ��Ÿ���� ���� false�� �����Ͽ� ���� ���� �� UI�� ��Ÿ���� �ʵ��� �մϴ�.
                PlayerPrefs.SetInt("FirstRun", 0);
                // ����� PlayerPrefs ����
                PlayerPrefs.Save();
            }
            else
            {
                // ó�� ����Ǵ� ���� �ƴϹǷ� UI�� ��Ȱ��ȭ�մϴ�.
                initialUI.SetActive(false);
            }
        }

        /// <summary>
        /// ���� �� ��ȯ
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
        /// ���� �� ��ȯ
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
        /// ARPet �ҷ�����
        /// </summary>
        /// <param name="selectPet"></param>
        private void UpdatePet(int selectPet)
        {
            Pet pet = petDB.GetPet(selectPet);
            petimages = pet.PetImage;

            if (pet != null) Destroy(_pet);
            _pet = GameObject.Instantiate(petimages, Vector3.zero, Quaternion.identity);

            // z ���� 1�� �����մϴ�.
            Vector3 position = _pet.transform.position;
            position.y = -2f;
            position.z = -1f;
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
        /// ARPet ����
        /// </summary>
        private void Save()
        {
            PlayerPrefs.SetInt("selectPet", selectPet);
        }
    }
}
