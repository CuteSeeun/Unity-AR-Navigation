using UnityEngine;

namespace ARNavi.Sumin
{
    public class RandomPet : MonoBehaviour
    {
        // �ִϸ��̼� ������Ʈ
        private Animation _animation;

        // �̵� �ӵ�
        public float _moveSpeed = 1f;
        // �ִϸ��̼� Ŭ�� �迭
        public AnimationClip[] animationClips;
        //��ƼŬ ������Ʈ
        public GameObject particle;

        void Start()
        {
            // Animation ������Ʈ�� ������
            _animation = GetComponent<Animation>();

            // �ִϸ��̼� Ŭ�� �迭�� ������� ���� ��쿡�� ����
            if (animationClips != null && animationClips.Length > 0)
            {
                // �������� �ִϸ��̼� Ŭ���� ����
                AnimationClip randomClip = animationClips[Random.Range(0, animationClips.Length)];

                // ������ �ִϸ��̼� Ŭ���� �����ϰ� ���
                _animation.AddClip(randomClip, "RandomAnimation");
                _animation.Play("RandomAnimation");
            }
            // ��ƼŬ ����
            GameObject instantiatedParticle = Instantiate(particle, transform.position, Quaternion.identity);
            DestroyImmediate(instantiatedParticle, true);
        }


        void Update()
        {
            _animation.Play();
            // ���� ��ġ���� ������ �� ĭ�� �̵�
            transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
        }
    }
}

