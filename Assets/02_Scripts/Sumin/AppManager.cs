using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ARNavi.Sumin
{
    public class AppManager : MonoBehaviour
    {
        /// <summary>
        /// �ҷ��� ���̶� �����ؼ� ����ϱ�
        /// </summary>
        //RasterMap myWebView;

        public RawImage DeniedPopup;
        public RawImage CameraDeniedPopup;
        public Button ARButton;

        int ClickCount = 0;
        // �ؽ�Ʈ�� ǥ���� UI �ؽ�Ʈ
        public TextMeshProUGUI notificationText;
        //�ɼ� UI
        public GameObject Options;
        //���â
        public GameObject Options2;
        // ī�޶� ���� �ź� ���θ� �����ϴ� ����
        bool cameraPermissionDenied = false;

        void Start()
        {
            // ��ġ ���� Ȯ�� �ڷ�ƾ ����
            StartCoroutine(AccessLocation());
            //ar��ư�� ������ �߰�
            ARButton.onClick.AddListener(RequestCameraPermission); 
        }

        void RequestCameraPermission()
        {
            if (PlayerPrefs.GetInt("CameraPermissionDenied", 0) == 1)
            {
                // ���� �ź� �ÿ��� �˾� Ȱ��ȭ
                CameraDeniedPopup.gameObject.SetActive(true);
            }
            // ī�޶� ������ ������ ���� ��û
            else if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                PermissionCallbacks cameraPermissionCallbacks = new PermissionCallbacks();

                Permission.RequestUserPermission(Permission.Camera, cameraPermissionCallbacks);

                cameraPermissionCallbacks.PermissionGranted += CameraPermissionGranted;
                cameraPermissionCallbacks.PermissionDenied += CameraPermissionDenied;
            }
            else
            {
                cameraPermissionDenied = true;
            }

        }

        internal void CameraPermissionGranted(string permissionName)
        {
            // ī�޶� ������ ���Ǹ� DeniedPopup�� ��Ȱ��ȭ
            CameraDeniedPopup.gameObject.SetActive(false);
            cameraPermissionDenied = true;
        }

        internal void CameraPermissionDenied(string permissionName)
        {
            // ������ �źεǸ� �˾� Ȱ��ȭ �� ���� ����
            PlayerPrefs.SetInt("CameraPermissionDenied", 1);
            PlayerPrefs.Save();
            CameraDeniedPopup.gameObject.SetActive(true);
        }


        public void DirectionsPage()
        {
            //���� �� �ȳ� �������� ���� �ϴµ� ���� �� ������ ������ �н�
        }

        void OnEnable()
        {
            // ��ũ��Ʈ�� Ȱ��ȭ�� ������ ���� �ź� ���θ� Ȯ���ϰ� �źε����� �˾��� Ȱ��ȭ�մϴ�.
            if (cameraPermissionDenied)
            {
                CameraDeniedPopup.gameObject.SetActive(true);
            }
        }

        IEnumerator AccessLocation()
        {
            yield return new WaitForSeconds(2f); // 2�� ���

            // ������ ��ġ ���� ������ �źεǾ����� Ȯ��
            if (PlayerPrefs.GetInt("LocationPermissionDenied", 0) == 1)
            {
                // �źεǾ����� DeniedPopup�� Ȱ��ȭ�Ͽ� ����ڿ��� �˸�
                DeniedPopup.gameObject.SetActive(true);
            }
            else if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                // ��ġ ���� ������ ������ ������ ��û
                PermissionCallbacks callbacks = new PermissionCallbacks();
                Permission.RequestUserPermission(Permission.FineLocation, callbacks);

                // ���� ��� �� �ź� �ݹ� ����
                callbacks.PermissionGranted += Permissioncallbacks_PermissionGranted;
                callbacks.PermissionDenied += Permissioncallbacks_PermissionDenied;
            }
            else
            {
                // ��ġ ���� ������ �̹� �ִ� ��쿡 ���� ó��
                //(���� �� �ε� �ϸ� ��� ���ξ��� ���� ��ħ�Ǽ� �ٸ� ��� �ʿ�)
                //SceneManager.LoadScene("MainScene");
            }
        }

        internal void Permissioncallbacks_PermissionGranted(string permissionName)
        {
            // ���� ��� �� �ƹ� �۾��� ���� ����
        }

        internal void Permissioncallbacks_PermissionDenied(string permissionName)
        {
            // ������ �źε� ���
            PlayerPrefs.SetInt("LocationPermissionDenied", 1);
            // ���¸� ����
            PlayerPrefs.Save();
            // DeniedPopup�� Ȱ��ȭ�Ͽ� ����ڿ��� �˸�
            DeniedPopup.gameObject.SetActive(true);
        }

        //��ġ �ź� -> �� ���� ��ư -> ������ �ڵ�
        public void Close()
        {
            Application.Quit();
        }

        //�ȵ���̵� �������� �̵�.
        public void Settings()
        {
            try
            {
#if UNITY_ANDROID
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    string packageName = currentActivityObject.Call<string>("getPackageName");

                    using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                    using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                    using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                    {
                        intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                        intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                        currentActivityObject.Call("startActivity", intentObject);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // ���� ���� MainScene�� �ƴ� ��쿡�� ���� ������ ��ȯ
                if (SceneManager.GetActiveScene().name == "MainScene")
                {
                    Escape();
                }
                else
                {
                    ReturnScene();
                }
            }
        }

        /// <summary>
        /// �ڷ� ���� ��ư ó�� �޼���
        /// �ۿ��� ������ ���
        /// </summary>
        public void Escape()
        {
            //�˻� ��� �� ���� ���
            if (Options.activeSelf)
            {
                // Options�� ��Ȱ��ȭ
                Options.SetActive(false);
            }
            //�˻���� ���� ���¿��� ���� ���
            else if (Options2.activeSelf)
            {
                // Options2�� ��Ȱ��ȭ
                Options2.SetActive(false);

                //�˻��Ѵ� ���� ���� ���� �ʿ� ������ ����� 
                //myWebView.CloseWebview();

                // Options�� Ȱ��ȭ
                Options.SetActive(true);
            }
            else
            {
                ClickCount++;
                if (!IsInvoking("DoubleClick"))
                {
                    Invoke("DoubleClick", 1.0f);
                    ShowNotification("�������� �ѹ� �� Ŭ�����ּ���.");
                }
                else if (ClickCount == 2)
                {
                    CancelInvoke("DoubleClick");
                    Application.Quit();
                }
            }
        }

        /// <summary>
        /// �ι� Ŭ��
        /// </summary>
        void DoubleClick()
        {
            ClickCount = 0;
        }

        /// <summary>
        /// �ѹ� ������ ������ �޼��� �ҷ�����
        /// �˸� �޽��� ǥ�� �޼���
        /// </summary>
        /// <param name="message"></param>
        void ShowNotification(string message)
        {
            if (notificationText != null)
            {
                notificationText.text = message;
                notificationText.gameObject.SetActive(true);
                Invoke("HideNotification", 1.0f);
            }
        }

        /// <summary>
        /// �ȳ������� ������ �ֱ�
        /// �˸� �޽��� ����� �޼���
        /// </summary>
        void HideNotification()
        {
            if (notificationText != null)
            {
                notificationText.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// ���� �˻� �� �̵�
        /// </summary>
        public void MapScene()
        {
            SceneManager.LoadScene("Chayeong_02");
        }

        /// <summary>
        /// ar �׺���̼� �� �ε� �޼���
        /// </summary>
        public void ARNavigate()
        {
            //ī�޶� ���� ���� ��쿡
            if (cameraPermissionDenied == true)
            {
                SceneManager.LoadScene("GeospatialArf4");
            }
        }

        /// <summary>
        /// ARPet ���þ� �̵�
        /// </summary>
        public void PetScene()
        {
            SceneManager.LoadScene("ARPetScene");
        }

        /// <summary>
        /// ���� �� ��ȯ
        /// </summary>
        public void ReturnScene()
        {
            SceneManager.LoadScene("UIToolkit");
        }
    }
}
