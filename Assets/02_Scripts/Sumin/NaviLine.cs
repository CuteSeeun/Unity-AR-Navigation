using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARNavi.Sumin
{

    /// ������ �浵�� ��Ÿ���� ����ü ���� 
    [System.Serializable]
    public struct Vector2Double
    {
        // ����
        public double latitude;
        // �浵
        public double longitude;
        // ������, ���� �浵�� ����� ���;���
        public Vector2Double(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }

    /// <summary>
    /// �׺���̼ǿ� ȭ��ǥ ǥ���ϴ� ��ũ��Ʈ
    /// </summary>
    public class NavLine : MonoBehaviour
    {
        ARRaycastManager arManager;
        // ������ �Ҵ�
        public GameObject navigator;
        // �Ÿ�
        float interval = 5f;
        // ���� Ÿ�̸�(������ �ʹ� ���� ����)
        float timer = 0f;
        // ���� �ð�(������ �ʹ� ���� ����)
        float spawnCool = 1f;
        // ȭ��ǥ�� �����ϴ� �ִ�ð�
        float arrowLifetime = 5f;
        // ȭ��ǥ�� �ٴ����κ��� �󸶳� �������� �����ϴ� ���� ������
        float heightOffset = 0.03f;
        // ������ ȭ��ǥ���� �����ϴ� ����Ʈ
        List<GameObject> arrows = new List<GameObject>();
        // �������� �浵�� ���� ����Ʈ
        public List<Vector2Double> destination = new List<Vector2Double>();
            // ���� ������ �ε���
            int nextDestinationIndex = 0;
            // ���� ������������ �Ÿ�
            float distanceToNextDestination = float.MaxValue;
            // ������ �������� �ε���
            int reachedDestinationIndex = -1;

            /// <summary>
            /// Unity Inspector���� ����ڰ� ���� ������ �� �ֵ��� ��
            /// </summary>
            public List<Vector2Double> Destination
            {
                get { return destination; }
                set { destination = value; }
            }

            void Start()
            {
                arManager = GetComponent<ARRaycastManager>();
            }

            void Update()
            {
                timer += Time.deltaTime;
                if (timer >= spawnCool)
                {
                    Ground();
                    timer = 0f;
                }
            }

            /// <summary>
            /// ������ ȭ��ǥ�� ��Ÿ���� �ϱ�
            /// </summary>    
        void Ground()
        {
            Vector2 screenSize = new Vector2(Screen.width / 2, Screen.height / 2);
            List<ARRaycastHit> hitinfo = new List<ARRaycastHit>();

            if (arManager.Raycast(screenSize, hitinfo, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
            {
                // ������ ����Ʈ�� �׸��� �ִ��� Ȯ��
                if (destination.Count > 0)
                {
                   // ������ ������ ȭ��ǥ���� ��� ����
                    foreach (var arrow in arrows)
                    {
                        Destroy(arrow);
                    }
                    // ����Ʈ ����
                    arrows.Clear();
                    // �ٴڿ� ������ ��ġ ���
                    Vector3 spawnPosition = hitinfo[0].pose.position;
                    // interval ��ŭ ���� �̵�
                    Vector3 intervalVector = Camera.main.transform.forward * interval;

                    // ȭ��ǥ�� �����ϰ� ����Ʈ�� �߰�
                    for (int i = 0; i <= 0; i++)
                    {
                        Vector3 nextPos = spawnPosition + intervalVector * i;
                        // ���� ������ ����
                        nextPos.y -= heightOffset;
                        // ȭ��ǥ�� ������ �����ϱ� ���� ȸ���� ���
                        Quaternion rotation =
                            Quaternion.LookRotation(CalculateDestinationDirection
                            (hitinfo[0].pose.position, destination[nextDestinationIndex]))
                                             * Quaternion.Euler(90, 0, 0);
                        // ȭ��ǥ�� �����Ͽ� ����Ʈ�� �߰�
                        GameObject arrow = Instantiate(navigator, nextPos, rotation);
                        arrows.Add(arrow);
                    }

                    // ���� ��ġ�� ���� ������������ �Ÿ� ���
                    distanceToNextDestination = Vector3.Distance(hitinfo[0].pose.position,
                        new Vector3((float)destination[nextDestinationIndex].longitude,                            
                        0f, (float)destination[nextDestinationIndex].latitude));
                    // ���� �������� �������� ��
                if (distanceToNextDestination < 0.5f)
                {
                        reachedDestinationIndex = nextDestinationIndex;
                        // ���� ������ �ε��� ����
                        nextDestinationIndex = (nextDestinationIndex + 1) % destination.Count;
                        // ������ �������� �˸��� �޽��� ���
                        Debug.Log("������ ������: " + reachedDestinationIndex);
                    }
                    // ������ ȭ��ǥ���� arrowLifetime ���Ŀ� ����
                    foreach (var arrow in arrows)
                    {
                        Destroy(arrow, arrowLifetime);
                    }
                }
                else
                {
                    // ������ ����Ʈ�� �׸��� ���� ��쿡 ���� ó��                  
                    foreach (var arrow in arrows)
                    {
                        // ������ ȭ��ǥ���� ��� ����
                        Destroy(arrow);
                    }
                    // ����Ʈ ����
                    arrows.Clear();
                    // ���� �������� �����߽��ϴ� �ϴ� UI �����
                    Debug.LogWarning("�������� �������� �ʾҽ��ϴ�.");
                }
            }
        }

        // ���� ��ġ�� ������ ������ ���� ���͸� ����ϴ� �޼��� 
        Vector3 CalculateDestinationDirection(Vector3 currentPosition, Vector2Double destination)
        {
            // ������ ��ġ�� Vector3�� ��ȯ
            Vector3 destinationPosition =
                new Vector3((float)destination.longitude, 0f, (float)destination.latitude);
            // ���� ��ġ�� �������� ���̸� ����Ͽ� ���� ���͸� ����
            Vector3 difference = destinationPosition - currentPosition;
            // ���� ���͸� ����ȭ�Ͽ� ��ȯ
            return difference.normalized;
        }
    }
}