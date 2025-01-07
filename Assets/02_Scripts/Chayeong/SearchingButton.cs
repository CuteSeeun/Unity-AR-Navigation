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

    // �˻���
    private TMP_InputField searchingLoc;

    // �˻��� �޴� ��ư
    private GameObject menuButton;

    // �˻��� back ��ư
    private GameObject backButton;


    public RasterMap rasterMap; // Unity �����Ϳ��� ����


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
    // �ʱ� �˻�â ��ư ������ �� (= ���� ���߱�)
    public void OnSearchingBarButton()
    {
        canvasSearchingBar.enabled = true;
        canvasSearchingList.enabled = true;
        canvasMapScene.enabled = false;
        
        //���� �����ϴ� �ڵ�
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

    // �ڷΰ��� ��ư
    public void OnSearchingBarBackButton()
    {
        searchingLoc.text = ""; // �˻�â �ؽ�Ʈ �ʱ�ȭ
        canvasAutoComplete.enabled = false; // �ڵ��ϼ� ĵ���� ��Ȱ��
        canvasSearchingList.enabled = false; // �ֱٰ˻���� ĵ���� ��Ȱ��

        menuButton.SetActive(true); // �˻���-�޴� ��ư Ȱ��ȭ
        backButton.SetActive(false); // �˻���-back ��ư ��Ȱ��ȭ

        //���� �Ѿ��ϴ� �ڵ�
        try
        {
            rasterMap.webViewObject.SetVisibility(true);

            // �˻� ����ȭ������ ���ư���(= �� ���� ��ġ ��� ���� ����)
            // ���� ��Ŀ �ʱ�ȭ
            rasterMap.webViewObject.EvaluateJS($"initialize();");
            //Destroy(webViewObject);
        }
        catch (System.Exception e)
        {
            print($"WebView Error : {e}");
        }
    }
}
