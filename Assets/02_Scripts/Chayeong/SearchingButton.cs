using ARNavi.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SearchingButton : MonoBehaviour
{
    Canvas canvasSearchingBar;
    Canvas canvasSearchingList;
    Canvas canvasAutoComplete;

    // 검색바
    private TMP_InputField searchingLoc;

    // 검색바 메뉴 버튼
    private GameObject menuButton;

    // 검색바 back 버튼
    private GameObject backButton;


    public RasterMap rasterMap; // Unity 에디터에서 설정


    void Start()
    {
        canvasSearchingBar = GameObject.Find("Canvas_searchingBar").GetComponent<Canvas>();
        canvasSearchingList = GameObject.Find("Canvas_searchingList").GetComponent<Canvas>();
        canvasAutoComplete = GameObject.Find("Canvas_autoComplete").GetComponent<Canvas>();

        searchingLoc = GameObject.Find("InputField (TMP)-Searching").GetComponent<TMP_InputField>();
        menuButton = searchingLoc.transform.GetChild(1).gameObject;
        backButton = searchingLoc.transform.GetChild(2).gameObject;

        rasterMap = GameObject.Find("RasterMap").GetComponent<RasterMap>();
    }

    void Update()
    {
    }

    /*
    // 초기 검색창 버튼 눌렀을 때 (= 지도 감추기)
    public void OnSearchingBarButton()
    {
        canvasSearchingBar.enabled = true;
        canvasSearchingList.enabled = true;
        canvasMapScene.enabled = false;
        
        //웹뷰 꺼야하는 코드
        try
        {
            rasterMap.webViewObject.SetVisibility(false);

            //Destroy(webViewObject);
        }
        catch (System.Exception e)
        {
            print($"WebView Error : {e}");
        }
    }
    */

    // 뒤로가기 버튼
    public void OnSearchingBarBackButton()
    {
        searchingLoc.text = ""; // 검색창 텍스트 초기화
        canvasAutoComplete.enabled = false; // 자동완성 캔버스 비활성
        canvasSearchingList.enabled = false; // 최근검색기록 캔버스 비활성

        menuButton.SetActive(true); // 검색바-메뉴 버튼 활성화
        backButton.SetActive(false); // 검색바-back 버튼 비활성화

        //웹뷰 켜야하는 코드
        try
        {
            rasterMap.webViewObject.SetVisibility(true);

            // 검색 메인화면으로 돌아가기(= 내 현재 위치 기반 지도 띄우기)
            // 현재 마커 초기화
            rasterMap.webViewObject.EvaluateJS($"initialize();");
            //Destroy(webViewObject);
        }
        catch (System.Exception e)
        {
            print($"WebView Error : {e}");
        }
    }
}
