using System.Collections;
using UnityEngine;

namespace ARNavi.Sumin
{
    public class SliderUI : MonoBehaviour
    {
        private Vector2 startTouchPosition;
        private Vector2 endTouchPosition;
        public bool isSliding = false;

        //레스터맵 제작시 연동하기
        //public RasterMap rasterMap;

        public Animator animator;

        //최소 거리
        public float minSwipeDistance = 50f;

        private void Start()
        {
            //레스터맵 제작시 연동하기
            //rasterMap = GetComponent<RasterMap>();
        }

        void Update()
        {
            // 터치 이벤트 처리
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                //한번만 터치
                if (touch.phase == TouchPhase.Began)
                {
                    startTouchPosition = touch.position;
                }
                //스와이프
                else if (touch.phase == TouchPhase.Moved)
                {
                    endTouchPosition = touch.position;
                    //스와이프 할 거리
                    Vector2 swipeDirection = endTouchPosition - startTouchPosition;
                    float swipeDistance = swipeDirection.magnitude;

                    if (swipeDistance > minSwipeDistance)
                    {
                        // 아래로 스와이프하는 동작인지 확인
                        if (!isSliding && swipeDirection.y < 0)
                        {
                            StartCoroutine(SwipeDown());
                        }
                        // 위로 스와이프하는 동작인지 확인
                        else if (!isSliding && swipeDirection.y > 0)
                        {
                            StartCoroutine(SwipeUp());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 슬라이드 내리기
        /// </summary>
        /// <returns></returns>
        IEnumerator SwipeDown()
        {
            animator.SetBool("extend", true);
            
            //레스터맵 크기 조절이 불가능하거나, 다른 방법이 있을경우 삭제
            //rasterMap.ExtendWebView();
            isSliding = true;

            yield return 1.5f;           
            isSliding = false;
        }

        /// <summary>
        /// 슬라이드 올리기
        /// </summary>
        /// <returns></returns>
        IEnumerator SwipeUp()
        {
            animator.SetBool("extend", false);
            //레스터맵 크기 조절이 불가능하거나, 다른 방법이 있을경우 삭제
            //rasterMap.ResetWebView();           

            isSliding = true;

            yield return 1.5f;
            isSliding = false;
        }
    }
}