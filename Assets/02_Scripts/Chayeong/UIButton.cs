using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AR.Navi
{
    public class UIButton : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(AccessLocation());
        }

        /// <summary>
        /// 위치접근 허용
        /// </summary>
        /// <returns></returns>
        IEnumerator AccessLocation()
        {
            // 로고 등장 2초 후 위치접근허용 팝업
            yield return new WaitForSeconds(2f);

            // 위치접근 권한이 없으면
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                // 콜백 인스턴스 생성
                PermissionCallbacks callbacks = new PermissionCallbacks();

                // 권한 요청
                Permission.RequestUserPermission(Permission.FineLocation, callbacks);

                // 허용했을 때, 거부했을 때
                callbacks.PermissionGranted += Permissioncallbacks_PermissionGranted;
                callbacks.PermissionDenied += Permissioncallbacks_PermissionDenied;
            }
            // 이미 위치접근 권한 있으면
            else
            {
                StaticMap();
            }
            StopCoroutine(AccessLocation());
        }

        internal void Permissioncallbacks_PermissionGranted(string permissionName)
        {
            // 위치권한 허용했을 때
            StaticMap();
        }

        internal void Permissioncallbacks_PermissionDenied(string permissionName)
        {
            // 위치권한 거부했을 때, 앱 종료
            Application.Quit();
        }

        public void StaticMap()
        {
            // 앱 홈화면 출력
            Canvas canvas2 = GameObject.Find("Canvas_2").gameObject.GetComponent<Canvas>();
            canvas2.enabled = true;            
        }

        // 검색바 입력시작
        public void OnTouchSearchingBar()
        {
            Canvas canvas2 = GameObject.Find("Canvas_2").gameObject.GetComponent<Canvas>();
            canvas2.enabled = false;

            Canvas canvas3 = GameObject.Find("Canvas_3").gameObject.GetComponent<Canvas>();
            canvas3.enabled = true;
        }

        
        TouchScreenKeyboard keyboard;
        public static string keyboardText = "";

        public void OnEnterOnSearchingBar()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Canvas canvas3 = GameObject.Find("Canvas_3").gameObject.GetComponent<Canvas>();
                canvas3.enabled = false;

                Canvas canvas4 = GameObject.Find("Canvas_4").gameObject.GetComponent<Canvas>();
                canvas4.enabled = true;
            }
        }


        // 검색바 입력 시작
        private void OnTouchSearchBarButton()
        {
            // keyboard = TouchScreenKeyboard.Open("Search", TouchScreenKeyboardType.Default);
            // keyboardText = keyboard.text;

            // TouchScreenKeyboard.Status.Done;
        }

        // 검색바 입력 종료
        public void OnEndSearchingBar()
        {
            Canvas canvas3 = GameObject.Find("Canvas_3").gameObject.GetComponent<Canvas>();
            canvas3.enabled = false;

            Canvas canvas4 = GameObject.Find("Canvas_4").gameObject.GetComponent<Canvas>();
            canvas4.enabled = true;
        }
        
        private void GotoAppSetting()
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
    }
}
