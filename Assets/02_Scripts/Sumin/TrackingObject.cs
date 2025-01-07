using UnityEngine;

namespace ARNavi.Sumin
{
    public class TrackingObject : MonoBehaviour
    {
        private Camera arCamera;
        // ARPet ��ũ��Ʈ�� ���� ����
        private ARPet arPet;

        private void Awake()
        {
            // AR ī�޶� ����
            arCamera = Camera.main;
            // ARPet ��ũ��Ʈ�� ���� ���� ��������
            arPet = GetComponent<ARPet>();
        }

        private void Update()
        {
            // ARPet ��ũ��Ʈ�� ���� ������Ʈ�� ��ġ�� ���� �̵�
            MoveToARPetPosition();
            // AR ī�޶��� ȸ���� ���� ������ ����
            RotateWithARCamera();
        }

        /// <summary>
        ///  ARPet ��ũ��Ʈ�� ���� ������Ʈ�� ��ġ�� ���� �̵��ϴ� �޼���
        /// </summary>
        private void MoveToARPetPosition()
        {
            if (arPet != null)
            {
                // ARPet ��ũ��Ʈ�� ���� ������Ʈ�� ��ġ�� �̵�
                Vector3 targetPosition = arPet.transform.position + Vector3.up * 2f;
                transform.position = targetPosition;
            }
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