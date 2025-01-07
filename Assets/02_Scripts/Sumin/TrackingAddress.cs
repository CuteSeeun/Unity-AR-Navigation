using UnityEngine;

namespace ARNavi.Sumin
{
    public class TrackingAddress : MonoBehaviour
    {
        private Camera arCamera;

        private void Awake()
        {
            // AR ī�޶� ����
            arCamera = Camera.main;
        }

        private void Update()
        {
            // AR ī�޶��� ȸ���� ���� ������ ����
            RotateWithARCamera();
        }

        /// <summary>
        /// ������Ʈ�� ������� �ż���
        /// </summary>
        private void RotateWithARCamera()
        {
            if (arCamera != null)
            {
                // AR ī�޶��� ȸ���� ����
                transform.rotation = Quaternion.Euler(arCamera.transform.eulerAngles.x, arCamera.transform.eulerAngles.y, 0f);
            }
        }
    }
}