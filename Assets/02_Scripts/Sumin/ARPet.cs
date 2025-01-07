using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace ARNavi.Sumin
{
    /// <summary>
    /// ARPet ��ȣ�ۿ� ��ũ��Ʈ
    /// </summary>
    public class ARPet : MonoBehaviour
    {
        private Camera arCamera;
        public Animator animator;
        // ARPet�� �ʱ� �Ÿ�
        public float initialDistance = 10f;
        public float initialRight = 1f;
        public float initialUp = 3f;

        // ARPet�� �̵� �ӵ�
        public float moveSpeedMultiplier = 5f;

        // �������� �����ߴ��� ���θ� ��Ÿ���� ����
        private bool arrived = false;

        private void Awake()
        {
            // AR ī�޶� ����
            arCamera = Camera.main;
            //�ִϸ��̼� ���
            Animator animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!arrived || !HasDestination())
            {
                FocusOn();
                Interact();
            }
        }

        /// <summary>
        /// ARPet�� ȭ�� �տ� ����
        /// </summary>
        void FocusOn()
        {
            // �������� ���� ��쿡�� ī�޶��� ��ġ�� ���󰡵��� ��
            // ARPet�� ��ġ�� �׻� ī�޶��� ��ġ�� �����Ͽ� ī�޶��� �������� ���󰡵��� ��
            transform.position = arCamera.transform.position + arCamera.transform.forward * initialDistance;
            transform.position += arCamera.transform.right * initialRight;
            transform.position += arCamera.transform.up * initialUp;

            // ARPet�� �׻� ī�޶� ȸ���ϴ� ������ �ٶ󺸵��� ��
            transform.forward = -arCamera.transform.forward;
        }

        /// <summary>
        /// �������� �̵��ϴ� �޼���
        /// </summary>
        public void MoveToDestination(Vector3 destination)
        {
            // ������������ �Ÿ��� ���� �Ÿ� ������ �� �������� ó��
            if (Vector3.Distance(transform.position, destination) < 0.1f)
            {
                arrived = true;
                return;
            }

            // ������ �������� �̵�
            Vector3 direction = (destination - transform.position).normalized;
            transform.position += direction * moveSpeedMultiplier * Time.deltaTime;
            // ��ǥ�� ���� �ٶ󺸵��� ȸ��
            transform.forward = direction;
        }

        /// <summary>
        /// ������ �������� �缳���ϴ� �޼���
        /// </summary>
        public void ResetDestination()
        {
            arrived = false;
        }

        /// <summary>
        /// �������� �ִ��� ���θ� ��ȯ�ϴ� �޼���
        /// </summary>
        private bool HasDestination()
        {
            // ���⿡ �������� �ִ����� Ȯ���ϴ� �ڵ带 �����ؾ� �մϴ�.
            // ���� ���, Destination ����Ʈ�� ��� �ִ����� Ȯ���Ͽ� �������� ������ �Ǵ��� �� �ֽ��ϴ�.

            // ����� �������� �ʾ����Ƿ� �׻� false�� ��ȯ�մϴ�.
            return false;
        }

        /// <summary>
        /// ĳ���� ��ġ�� ��ȣ�ۿ� �ִϸ��̼� �۵�
        /// </summary>
        void Interact()
        {
            // ����ڰ� ȭ���� ��ġ���� ��
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                // Ray�� ĳ���Ϳ� �浹�ϴ��� Ȯ��
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // �浹�� ��ü�� ĳ�������� Ȯ��
                    if (hit.collider.gameObject == gameObject && animator != null)
                    {
                        animator.SetTrigger("jump");
                    }
                }
            }
        }

        /// <summary>
        ///  AR ������̼� �۵��� ���� �̵��ϵ��� ����
        /// </summary>
        public void PetMove()
        {
            // ���� ���� ������Ʈ�� ����� PlayableDirector ã��
            PlayableDirector director = GetComponent<PlayableDirector>();
            if (director != null)
            {
                // �÷��̾�� ���ͷ� �� �̵��ϴ� Ʈ�� ã��
                TrackAsset petMoveTrack = null;

                // �÷��̾�� ������ Ʈ�� �� ��ŭ �ݺ�
                foreach (PlayableBinding binding in director.playableAsset.outputs)
                {
                    // Ʈ���� �� �̵� Ʈ������ Ȯ��
                    if (binding.sourceObject is TrackAsset track && track.name == "PetMoveTrack")
                    {
                        petMoveTrack = track;
                        break;
                    }
                }

                // �� �̵� Ʈ���� �����ϴ� ��쿡�� ó��
                if (petMoveTrack != null)
                {
                    // Ʈ���� ����� Ŭ�� �� ��ŭ �ݺ�
                    foreach (TimelineClip clip in petMoveTrack.GetClips())
                    {
                        // �ش� Ŭ���� ���� �ð� �� ���� �ð� ��������
                        double startTime = clip.start;
                        double endTime = clip.end;
                        // �÷��̾�� ���Ϳ��� �ش� �ð���� �÷���
                        director.time = startTime;
                        director.Evaluate();
                    }
                }
            }
        }
    }
}
