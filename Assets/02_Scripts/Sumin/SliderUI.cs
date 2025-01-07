using System.Collections;
using UnityEngine;

namespace ARNavi.Sumin
{
    public class SliderUI : MonoBehaviour
    {
        private Vector2 startTouchPosition;
        private Vector2 endTouchPosition;
        public bool isSliding = false;

        //�����͸� ���۽� �����ϱ�
        //public RasterMap rasterMap;

        public Animator animator;

        //�ּ� �Ÿ�
        public float minSwipeDistance = 50f;

        private void Start()
        {
            //�����͸� ���۽� �����ϱ�
            //rasterMap = GetComponent<RasterMap>();
        }

        void Update()
        {
            // ��ġ �̺�Ʈ ó��
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                //�ѹ��� ��ġ
                if (touch.phase == TouchPhase.Began)
                {
                    startTouchPosition = touch.position;
                }
                //��������
                else if (touch.phase == TouchPhase.Moved)
                {
                    endTouchPosition = touch.position;
                    //�������� �� �Ÿ�
                    Vector2 swipeDirection = endTouchPosition - startTouchPosition;
                    float swipeDistance = swipeDirection.magnitude;

                    if (swipeDistance > minSwipeDistance)
                    {
                        // �Ʒ��� ���������ϴ� �������� Ȯ��
                        if (!isSliding && swipeDirection.y < 0)
                        {
                            StartCoroutine(SwipeDown());
                        }
                        // ���� ���������ϴ� �������� Ȯ��
                        else if (!isSliding && swipeDirection.y > 0)
                        {
                            StartCoroutine(SwipeUp());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �����̵� ������
        /// </summary>
        /// <returns></returns>
        IEnumerator SwipeDown()
        {
            animator.SetBool("extend", true);
            
            //�����͸� ũ�� ������ �Ұ����ϰų�, �ٸ� ����� ������� ����
            //rasterMap.ExtendWebView();
            isSliding = true;

            yield return 1.5f;           
            isSliding = false;
        }

        /// <summary>
        /// �����̵� �ø���
        /// </summary>
        /// <returns></returns>
        IEnumerator SwipeUp()
        {
            animator.SetBool("extend", false);
            //�����͸� ũ�� ������ �Ұ����ϰų�, �ٸ� ����� ������� ����
            //rasterMap.ResetWebView();           

            isSliding = true;

            yield return 1.5f;
            isSliding = false;
        }
    }
}