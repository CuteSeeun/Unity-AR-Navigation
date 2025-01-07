using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectLocation : MonoBehaviour
{
    // public RasterMap rasterMap; // Unity �����Ϳ��� ����
    public RasterMap rasterMap; // Unity �����Ϳ��� ����

    void Start()
    {
        rasterMap = GameObject.Find("RasterMap").GetComponent<RasterMap>();
    }

    public void OnSelectLocationButton()
    {
        TextMeshProUGUI[] address = gameObject.GetComponentsInChildren<TextMeshProUGUI>();

        string locationName = address[1].text;

        Debug.Log(locationName);

        //���� Ű�� �ڵ��ϼ� ��ġ�� �ּ� �ڹٽ�ũ��Ʈ���� ������
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
        //locationName�� �Ű������� �ڹٽ�ũ��Ʈ ȣ���ϴ� �ڵ�
        if (rasterMap != null && !string.IsNullOrEmpty(locationName))
        {
            string searchKeyword = locationName;  // �Է� �ʵ忡�� �˻�� ������
            Debug.Log(1);  // Unity���� �α� ���
            rasterMap.webViewObject.EvaluateJS($"searchPOI('{searchKeyword}');"); //�ڹٽ�ũ��Ʈ �Լ��� ȣ���Ͽ� �˻���� POI�˻�
        }
        */


        //�ڵ��ϼ� ĵ������ �����Ⱥ��̰� ĵ������ ���� �ڵ�
        // �ڵ��ϼ� ĵ������ ��Ȱ��ȭ
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
