using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARNavi.Sumin
{
    public class NaviMonAppear : MonoBehaviour
    {

        //펫의 데이터베이스 참조
        public PetDataBase petDatabase;
        // 레이캐스트 관리
        public ARRaycastManager arRaycastManager;
        // 다음 펫 생성 시간
        float spawnInterval = 3f;
        // 팻 생성 타이머
        float spawnTimer = 0f;

        // 생성된 팻을 관리하기 위한 리스트
        List<GameObject> spawnedPets = new List<GameObject>();


        void Update()
        {
            // 일정 간격으로 팻 생성
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                // 카메라의 위치에서 아래쪽으로 레이캐스트 수행하여 땅의 위치를 찾음
                // 카메라의 위치를 가져옴
                Vector3 cameraPosition = Camera.main.transform.position;

                // x축과 z축에 대해 -40에서 -10 사이 또는 10에서 40 사이의 랜덤 값 생성
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

                // 팻 생성 위치 설정
                Vector3 spawnPosition = new Vector3(cameraPosition.x + randomX, cameraPosition.y, cameraPosition.z + randomZ);

                // 팻 생성
                SpawnPet(spawnPosition);

                // 타이머 초기화
                spawnTimer = 0f;
            }
        }

        /// <summary>
        /// 랜덤 펫 생성하는 매서드
        /// </summary>
        /// <param name="spawnPosition"></param>
        void SpawnPet(Vector3 spawnPosition)
        {
            // PetDatabase에서 랜덤한 펫 데이터를 선택
            int randomIndex = Random.Range(0, petDatabase.PetCount);
            Pet randomPet = petDatabase.GetPet(randomIndex);

            // 팻 생성
            GameObject spawnedPet = Instantiate(randomPet.PetImage, spawnPosition, Quaternion.identity);
            spawnedPets.Add(spawnedPet);


            // 랜덤한 방향으로 이동
            Vector3 moveDirection = Random.onUnitSphere;
            // y 축은 움직이지 않도록 설정
            moveDirection.y = 0f;
            // 일정 시간이 지난 후에 생성된 팻 파괴
            float petLifetime = Random.Range(6, 10);
            Destroy(spawnedPet, petLifetime);
        }
    }
}
