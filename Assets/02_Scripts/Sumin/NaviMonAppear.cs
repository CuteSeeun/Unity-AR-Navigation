using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARNavi.Sumin
{
    public class NaviMonAppear : MonoBehaviour
    {

        //���� �����ͺ��̽� ����
        public PetDataBase petDatabase;
        // ����ĳ��Ʈ ����
        public ARRaycastManager arRaycastManager;
        // ���� �� ���� �ð�
        float spawnInterval = 3f;
        // �� ���� Ÿ�̸�
        float spawnTimer = 0f;

        // ������ ���� �����ϱ� ���� ����Ʈ
        List<GameObject> spawnedPets = new List<GameObject>();


        void Update()
        {
            // ���� �������� �� ����
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                // ī�޶��� ��ġ���� �Ʒ������� ����ĳ��Ʈ �����Ͽ� ���� ��ġ�� ã��
                // ī�޶��� ��ġ�� ������
                Vector3 cameraPosition = Camera.main.transform.position;

                // x��� z�࿡ ���� -40���� -10 ���� �Ǵ� 10���� 40 ������ ���� �� ����
                float randomX = Random.Range(cameraPosition.x - 40f, cameraPosition.x - 10f);
                float randomZ = Random.Range(-20f, -5f);
                if (Random.Range(0, 2) == 1)
                {
                    randomX = Random.Range(cameraPosition.x + 10f, cameraPosition.x + 40f);
                }
                if (Random.Range(0, 2) == 1)
                {
                    randomZ = Random.Range(cameraPosition.z + 10f, cameraPosition.z + 40f);
                }

                // �� ���� ��ġ ����
                Vector3 spawnPosition = new Vector3(cameraPosition.x + randomX, cameraPosition.y, cameraPosition.z + randomZ);

                // �� ����
                SpawnPet(spawnPosition);

                // Ÿ�̸� �ʱ�ȭ
                spawnTimer = 0f;
            }
        }

        /// <summary>
        /// ���� �� �����ϴ� �ż���
        /// </summary>
        /// <param name="spawnPosition"></param>
        void SpawnPet(Vector3 spawnPosition)
        {
            // PetDatabase���� ������ �� �����͸� ����
            int randomIndex = Random.Range(0, petDatabase.PetCount);
            Pet randomPet = petDatabase.GetPet(randomIndex);

            // �� ����
            GameObject spawnedPet = Instantiate(randomPet.PetImage, spawnPosition, Quaternion.identity);
            spawnedPets.Add(spawnedPet);


            // ������ �������� �̵�
            Vector3 moveDirection = Random.onUnitSphere;
            // y ���� �������� �ʵ��� ����
            moveDirection.y = 0f;
            // ���� �ð��� ���� �Ŀ� ������ �� �ı�
            float petLifetime = Random.Range(6, 10);
            Destroy(spawnedPet, petLifetime);
        }
    }
}
