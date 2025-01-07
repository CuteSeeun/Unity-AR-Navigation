using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ARNavi.C
{
    public class AppManager : MonoBehaviour
    {
        RasterMap myWebView;
        TMap tmap;

        public RawImage DeniedPopup;
        public RawImage CameraDeniedPopup;
        public Button ARButton;
        
        int ClickCount = 0;
        // 텍스트를 표시할 UI 텍스트
        public TextMeshProUGUI notificationText;
        //검색창
        public GameObject Options;
        //결과창
        public GameObject Options2;
        // 카메라 권한 거부 여부를 저장하는 변수
        bool cameraPermissionAccess = false;

        void Start()
        {
            // 위치 권한 확인 코루틴 시작
            StartCoroutine(AccessLocation());
            //ar버튼에 리스너 추가
            ARButton.onClick.AddListener(RequestCameraPermission); 
        }

        void RequestCameraPermission()
        {
            if (PlayerPrefs.GetInt("CameraPermissionDenied", 0) == 1)
            {
                // 권한 거부 시에만 팝업 활성화
                CameraDeniedPopup.gameObject.SetActive(true);
            }
            // 카메라 권한이 없으면 권한 요청
            else if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                PermissionCallbacks cameraPermissionCallbacks = new PermissionCallbacks();

                Permission.RequestUserPermission(Permission.Camera, cameraPermissionCallbacks);

                cameraPermissionCallbacks.PermissionGranted += CameraPermissionGranted;
                cameraPermissionCallbacks.PermissionDenied += CameraPermissionDenied;
            }
            else 
            {
                cameraPermissionAccess = true;
            }
           
        }

        internal void CameraPermissionGranted(string permissionName)
        {
            // 카메라 권한이 허용되면 DeniedPopup을 비활성화
            CameraDeniedPopup.gameObject.SetActive(false);
            cameraPermissionAccess = true;
        }

        internal void CameraPermissionDenied(string permissionName)
        {
            // 권한이 거부되면 팝업 활성화 및 상태 저장
            PlayerPrefs.SetInt("CameraPermissionDenied", 1);
            PlayerPrefs.Save();
            CameraDeniedPopup.gameObject.SetActive(true);
        }


        public void DirectionsPage()
        {
            //원래 길 안내 페이지가 떠야 하는데 아직 그 페이지 없으니 패스
        }

        void OnEnable()
        {
            // 스크립트가 활성화될 때마다 권한 거부 여부를 확인하고 거부됐으면 팝업을 활성화합니다.
            if (cameraPermissionAccess)
            {
                CameraDeniedPopup.gameObject.SetActive(true);
            }
        }

        IEnumerator AccessLocation()
        {
            yield return new WaitForSeconds(2f); // 2초 대기

            // 이전에 위치 접근 권한이 거부되었는지 확인
            if (PlayerPrefs.GetInt("LocationPermissionDenied", 0) == 1)
            {
                // 거부되었으면 DeniedPopup을 활성화하여 사용자에게 알림
                DeniedPopup.gameObject.SetActive(true);
            }
            else if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                // 위치 접근 권한이 없으면 권한을 요청
                PermissionCallbacks callbacks = new PermissionCallbacks();
                Permission.RequestUserPermission(Permission.FineLocation, callbacks);

                // 권한 허용 및 거부 콜백 연결
                callbacks.PermissionGranted += Permissioncallbacks_PermissionGranted;
                callbacks.PermissionDenied += Permissioncallbacks_PermissionDenied;
            }
            else
            {
                // 위치 접근 권한이 이미 있는 경우에 대한 처리 (예: 메인 씬 로드)
                //SceneManager.LoadScene("MainScene");
            }
        }

        internal void Permissioncallbacks_PermissionGranted(string permissionName)
        {
            // 권한 허용 시 아무 작업도 하지 않음
        }

        internal void Permissioncallbacks_PermissionDenied(string permissionName)
        {
            // 권한이 거부된 경우
            PlayerPrefs.SetInt("LocationPermissionDenied", 1); 
            // 상태를 저장
            PlayerPrefs.Save();
            // DeniedPopup을 활성화하여 사용자에게 알림
            DeniedPopup.gameObject.SetActive(true);
        }

        //위치 거부 -> 앱 종료 버튼 -> 앱종료 코드
        public void Close()
        {
            Application.Quit();
        }

        //안드로이드 설정으로 이동.
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
                // 현재 씬이 MainScene이 아닌 경우에만 메인 씬으로 전환
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
        /// 뒤로 가기 버튼 처리 메서드
        /// 앱에서 나가는 기능
        /// </summary>
        public void Escape()
        {
            //검색 기능 중 누를 경우
            if (Options.activeSelf)
            {
                // 검색창을 비활성화
                Options.SetActive(false);
                tmap.UnOverlaps();
            }
            //검색결과 나온 상태에서 누를 경우
            else if (Options2.activeSelf)
            {
                // 결과창을 비활성화
                Options2.SetActive(false);
                //myWebView.CloseWebview();
                tmap.ResetWebView();

                // 검색창을 활성화
                Options.SetActive(true);
            }
            else
            {
                ClickCount++;
                if (!IsInvoking("DoubleClick"))
                {
                    Invoke("DoubleClick", 1.0f);
                    ShowNotification("나가려면 한번 더 클릭해주세요.");
                }
                else if (ClickCount == 2)
                {
                    CancelInvoke("DoubleClick");
                    Application.Quit();
                }
            }
        }

        /// <summary>
        /// 두번 클릭
        /// </summary>
        void DoubleClick()
        {
            ClickCount = 0;
        }

        /// <summary>
        /// 한번 누르면 나가는 메세지 불러오기
        /// 알림 메시지 표시 메서드
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
        /// 안나가려면 가만히 있기
        /// 알림 메시지 숨기기 메서드
        /// </summary>
        void HideNotification()
        {
            if (notificationText != null)
            {
                notificationText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// ar 네비게이션 씬 로드 메서드
        /// </summary>
        public void ARNavigate()
        {
            //카메라 권한 허용될 경우에 또는 혐재 씬이 ARPetScene 일때 이동
            if (cameraPermissionAccess == true || SceneManager.GetActiveScene().name == "ARPetScene" )
            {
                SceneManager.LoadScene("GeospatialArf4_Sumin");
            }
        }

        /// <summary>
        /// ARPet 선택씬 이동
        /// </summary>
        public void PetScene()
        {
            SceneManager.LoadScene("ARPetScene");
        }

        /// <summary>
        /// 메인 씬 전환
        /// </summary>
        public void ReturnScene()
        {          
            SceneManager.LoadScene("UIToolkit");
        }
    }
}
