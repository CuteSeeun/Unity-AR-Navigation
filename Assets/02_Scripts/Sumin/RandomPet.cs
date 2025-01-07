using UnityEngine;

namespace ARNavi.Sumin
{
    public class RandomPet : MonoBehaviour
    {
        // 애니메이션 컴포넌트
        private Animation _animation;

        // 이동 속도
        public float _moveSpeed = 1f;
        // 애니메이션 클립 배열
        public AnimationClip[] animationClips;
        //파티클 오브젝트
        public GameObject particle;

        void Start()
        {
            // Animation 컴포넌트를 가져옴
            _animation = GetComponent<Animation>();

            // 애니메이션 클립 배열이 비어있지 않은 경우에만 설정
            if (animationClips != null && animationClips.Length > 0)
            {
                // 랜덤으로 애니메이션 클립을 선택
                AnimationClip randomClip = animationClips[Random.Range(0, animationClips.Length)];

                // 선택한 애니메이션 클립을 설정하고 재생
                _animation.AddClip(randomClip, "RandomAnimation");
                _animation.Play("RandomAnimation");
            }
            // 파티클 생성
            GameObject instantiatedParticle = Instantiate(particle, transform.position, Quaternion.identity);
            DestroyImmediate(instantiatedParticle, true);
        }


        void Update()
        {
            _animation.Play();
            // 현재 위치에서 앞으로 한 칸씩 이동
            transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
        }
    }
}

