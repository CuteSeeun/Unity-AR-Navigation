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
        /// ��ġ���� ���
        /// </summary>
        /// <returns></returns>
        IEnumerator AccessLocation()
        {
            // �ΰ� ���� 2�� �� ��ġ������� �˾�
            yield return new WaitForSeconds(2f);

            // ��ġ���� ������ ������
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                // �ݹ� �ν��Ͻ� ����
                PermissionCallbacks callbacks = new PermissionCallbacks();

                // ���� ��û
                Permission.RequestUserPermission(Permission.FineLocation, callbacks);

                // ������� ��, �ź����� ��
                callbacks.PermissionGranted += Permissioncallbacks_PermissionGranted;
                callbacks.PermissionDenied += Permissioncallbacks_PermissionDenied;
            }
            // �̹� ��ġ���� ���� ������
            else
            {
                StaticMap();
            }
            StopCoroutine(AccessLocation());
        }

        internal void Permissioncallbacks_PermissionGranted(string permissionName)
        {
            // ��ġ���� ������� ��
            StaticMap();
        }

        internal void Permissioncallbacks_PermissionDenied(string permissionName)
        {
            // ��ġ���� �ź����� ��, �� ����
            Application.Quit();
        }

        public void StaticMap()
        {
            // �� Ȩȭ�� ���
            Canvas canvas2 = GameObject.Find("Canvas_2").gameObject.GetComponent<Canvas>();
            canvas2.enabled = true;            
        }

        // �˻��� �Է½���
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


        // �˻��� �Է� ����
        private void OnTouchSearchBarButton()
        {
            // keyboard = TouchScreenKeyboard.Open("Search", TouchScreenKeyboardType.Default);
            // keyboardText = keyboard.text;

            // TouchScreenKeyboard.Status.Done;
        }

        // �˻��� �Է� ����
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
