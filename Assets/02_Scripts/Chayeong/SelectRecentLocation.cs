using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectRecentLocation : MonoBehaviour
{
    public RasterMap rasterMap; // Unity 에디터에서 설정

    void Start()
    {
        rasterMap = GameObject.Find("RasterMap").GetComponent<RasterMap>();
    }

    public void OnSelectRecentLocationButton()
    {

        TextMeshProUGUI recentLocation = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        string locationName = recentLocation.text;

        Debug.Log(locationName);


        // 웹뷰 켜고 최근검색기록 터치 시 자바스크립트에게 검색어 보내기
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
        //웹뷰 locationName 매개변수 자바스크립트에게 전달하기
        if (rasterMap != null && !string.IsNullOrEmpty(locationName))
        {
            string searchKeyword = locationName;  // 입력 필드에서 검색어를 가져옴
            Debug.Log(1);  // Unity에서 로그 출력
            rasterMap.webViewObject.EvaluateJS($"searchPOI('{searchKeyword}');"); //자바스크립트 함수를 호출하여 검색어로 POI검색
        }
        */

        // 최근검색목록 캔버스 끄기
        Canvas searchingListCanvas = GameObject.Find("Canvas_searchingList").GetComponent<Canvas>();
        if (searchingListCanvas != null)
        {
            searchingListCanvas.enabled = false;
        }
        else
        {
            Debug.LogError("AutoComplete Canvas not found");
        }
    }
}
