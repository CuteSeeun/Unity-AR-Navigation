using UnityEngine;

namespace ARNavi.Sumin
{
    public class TrackingAddress : MonoBehaviour
    {
        private Camera arCamera;

        private void Awake()
        {
            // AR 카메라 설정
            arCamera = Camera.main;
        }

        private void Update()
        {
            // AR 카메라의 회전에 따라 방향을 조절
            RotateWithARCamera();
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