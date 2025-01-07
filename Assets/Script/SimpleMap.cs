using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking; // UnityEngine.Networking 네임스페이스 추가

public class WebViewLoader : MonoBehaviour
{
    // 웹뷰를 표시할 RawImage
    public RawImage webViewRawImage;

    void Start()
    {
        LoadWebView();
    }

    void LoadWebView()
    {
        // 웹뷰를 표시할 URL
        string url = "https://apis.openapi.sk.com/tmap/jsv2?version=1&appKey=" +
            "pU4GcaKOvx9LLvvu25eQg8AS2Izhv6V62crHL4E0";

        // 웹뷰 로드
        StartCoroutine(LoadWebViewCoroutine(url));
    }

    IEnumerator LoadWebViewCoroutine(string url)
    {
        // Unity의 UnityWebRequest를 사용하여 웹페이지를 가져오기
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);

        // 웹페이지가 로드될 때까지 대기
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // 웹페이지 로드가 완료되면 RawImage에 웹뷰 텍스처 설정
            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
            webViewRawImage.texture = texture;

            // 웹뷰 텍스처의 가로, 세로 크기를 RawImage의 크기에 맞게 설정
            webViewRawImage.SetNativeSize();
        }
        else
        {
            Debug.LogError("Failed to load webpage: " + webRequest.error);
        }
    }
}