using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectLocation : MonoBehaviour
{
    // public RasterMap rasterMap; // Unity 에디터에서 설정
    public RasterMap rasterMap; // Unity 에디터에서 설정

    void Start()
    {
        rasterMap = GameObject.Find("RasterMap").GetComponent<RasterMap>();
    }

    public void OnSelectLocationButton()
    {
        TextMeshProUGUI[] address = gameObject.GetComponentsInChildren<TextMeshProUGUI>();

        string locationName = address[1].text;

        Debug.Log(locationName);

        //웹뷰 키고 자동완성 터치한 주소 자바스크립트에게 보내기
        if (rasterMap.webViewObject != null)
        {
            rasterMap.webViewObject.SetVisibility(true);
            if (!string.IsNullOrEmpty(locationName))
            {
                string searchKeyword = locationName;
                rasterMap.webViewObject.EvaluateJS($"searchPOI('{searchKeyword}');");
            }
        }

        /*

        try
        {
            if (rasterMap.webViewObject != null)
            {
                rasterMap.webViewObject.SetVisibility(true);
                if (!string.IsNullOrEmpty(locationName))
                {
                    rasterMap.webViewObject.EvaluateJS($"searchPOI('{locationName}');");
                }
            }
            else
            {
                Debug.LogError("WebViewObject is not initialized in RasterMap.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebView Error: {e}");
        }

        */
        /*
        //locationName을 매개변수로 자바스크립트 호출하는 코드
        if (rasterMap != null && !string.IsNullOrEmpty(locationName))
        {
            string searchKeyword = locationName;  // 입력 필드에서 검색어를 가져옴
            Debug.Log(1);  // Unity에서 로그 출력
            rasterMap.webViewObject.EvaluateJS($"searchPOI('{searchKeyword}');"); //자바스크립트 함수를 호출하여 검색어로 POI검색
        }
        */


        //자동완성 캔버스가 눈에안보이게 캔버스만 끄는 코드
        // 자동완성 캔버스를 비활성화
        Canvas autoCompleteCanvas = GameObject.Find("Canvas_autoComplete").GetComponent<Canvas>();
        if (autoCompleteCanvas != null)
        {
            autoCompleteCanvas.enabled = false;
        }
        else
        {
            Debug.LogError("AutoComplete Canvas not found");
        }
    }
}
