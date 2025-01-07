using UnityEngine;

namespace ARNavi.Sumin
{
    public class TrackingObject : MonoBehaviour
    {
        private Camera arCamera;
        // ARPet 스크립트에 대한 참조
        private ARPet arPet;

        private void Awake()
        {
            // AR 카메라 설정
            arCamera = Camera.main;
            // ARPet 스크립트에 대한 참조 가져오기
            arPet = GetComponent<ARPet>();
        }

        private void Update()
        {
            // ARPet 스크립트를 지닌 오브젝트의 위치에 따라 이동
            MoveToARPetPosition();
            // AR 카메라의 회전에 따라 방향을 조절
            RotateWithARCamera();
        }

        /// <summary>
        ///  ARPet 스크립트를 지닌 오브젝트의 위치에 따라 이동하는 메서드
        /// </summary>
        private void MoveToARPetPosition()
        {
            if (arPet != null)
            {
                // ARPet 스크립트를 지닌 오브젝트의 위치로 이동
                Vector3 targetPosition = arPet.transform.position + Vector3.up * 2f;
                transform.position = targetPosition;
            }
        }

        /// <summary>
        /// 오브젝트를 방향고정 매서드
        /// </summary>
        private void RotateWithARCamera()
        {
            if (arCamera != null)
            {
                // AR 카메라의 회전을 적용
                transform.rotation = Quaternion.Euler(arCamera.transform.eulerAngles.x, arCamera.transform.eulerAngles.y, 0f);
            }
        }
    }
}