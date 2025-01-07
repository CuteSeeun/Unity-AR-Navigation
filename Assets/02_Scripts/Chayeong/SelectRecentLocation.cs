using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectRecentLocation : MonoBehaviour
{
    public RasterMap rasterMap; // Unity �����Ϳ��� ����

    void Start()
    {
        rasterMap = GameObject.Find("RasterMap").GetComponent<RasterMap>();
    }

    public void OnSelectRecentLocationButton()
    {

        TextMeshProUGUI recentLocation = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        string locationName = recentLocation.text;

        Debug.Log(locationName);


        // ���� �Ѱ� �ֱٰ˻���� ��ġ �� �ڹٽ�ũ��Ʈ���� �˻��� ������
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
        //���� locationName �Ű����� �ڹٽ�ũ��Ʈ���� �����ϱ�
        if (rasterMap != null && !string.IsNullOrEmpty(locationName))
        {
            string searchKeyword = locationName;  // �Է� �ʵ忡�� �˻�� ������
            Debug.Log(1);  // Unity���� �α� ���
            rasterMap.webViewObject.EvaluateJS($"searchPOI('{searchKeyword}');"); //�ڹٽ�ũ��Ʈ �Լ��� ȣ���Ͽ� �˻���� POI�˻�
        }
        */

        // �ֱٰ˻���� ĵ���� ����
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
