using ARNavi.Controller;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ARNavi.Model
{
    #region PoisitionBack

    public class EvChargers
    {
        public List<object> evCharger { get; set; }
    }

    public class NewAddress
    {
        public string centerLat { get; set; }
        public string centerLon { get; set; }
        public string frontLat { get; set; }
        public string frontLon { get; set; }
        public string roadName { get; set; }
        public string bldNo1 { get; set; }
        public string bldNo2 { get; set; }
        public string roadId { get; set; }
        public string fullAddressRoad { get; set; }
    }

    public class NewAddressList
    {
        public List<NewAddress> newAddress { get; set; }
    }

    public class Poi
    {
        public string id { get; set; }
        public string pkey { get; set; }
        public string navSeq { get; set; }
        public string collectionType { get; set; }
        public string name { get; set; }
        public string telNo { get; set; }
        public string frontLat { get; set; }
        public string frontLon { get; set; }
        public string noorLat { get; set; }
        public string noorLon { get; set; }
        public string upperAddrName { get; set; }
        public string middleAddrName { get; set; }
        public string lowerAddrName { get; set; }
        public string detailAddrName { get; set; }
        public string mlClass { get; set; }
        public string firstNo { get; set; }
        public string secondNo { get; set; }
        public string roadName { get; set; }
        public string firstBuildNo { get; set; }
        public string secondBuildNo { get; set; }
        public string radius { get; set; }
        public string bizName { get; set; }
        public string upperBizName { get; set; }
        public string middleBizName { get; set; }
        public string lowerBizName { get; set; }
        public string detailBizName { get; set; }
        public string rpFlag { get; set; }
        public string parkFlag { get; set; }
        public string detailInfoFlag { get; set; }
        public string desc { get; set; }
        public string dataKind { get; set; }
        public string zipCode { get; set; }
        public string adminDongCode { get; set; }
        public string legalDongCode { get; set; }
        public NewAddressList newAddressList { get; set; }
        public EvChargers evChargers { get; set; }
    }

    public class Pois
    {
        public List<Poi> poi { get; set; }
    }

    public class Root2
    {
        public SearchPoiInfo searchPoiInfo { get; set; }
    }

    public class SearchPoiInfo
    {
        public string totalCount { get; set; }
        public string count { get; set; }
        public string page { get; set; }
        public Pois pois { get; set; }
    }

    #endregion

    public class SearchingBar : MonoBehaviour
    {

        // �˻���
        public TMP_InputField searchingLoc;

        // �˻��� �޴� ��ư
        private GameObject menuButton;

        // �˻��� back ��ư
        private GameObject backButton;

        // �ֱٰ˻��� ĵ����
        Canvas searhcingListCanvas;

        public List<string> longitude = new List<string>();
        public List<string> latitude = new List<string>();
        public List<string> address = new List<string>();
        public List<string> name = new List<string>();

        private int version = 1;
        private int page = 1; // ��Ұ˻��� ������ ��
        private int count = 11; // ���������� ������ ��� ����
        private string callback = "fuction";
        private string searchKeyword;
        string appKey = "9UweKMZ14A4N0CQD22GRb7fhP1IscFjA7ITMABz0"; // TMap AppKey

        public RasterMap rasterMap; // Unity �����Ϳ��� ����


        private void Awake()
        {
            searchingLoc = gameObject.GetComponent<TMP_InputField>();
            if (searchingLoc == null)
                Debug.Log("searchingLoc == null");
        }

        void Start()
        {
            menuButton = gameObject.transform.GetChild(1).gameObject;
            backButton = gameObject.transform.GetChild(2).gameObject;

            searhcingListCanvas = GameObject.Find("Canvas_searchingList").GetComponent<Canvas>();
            rasterMap = GameObject.Find("RasterMap").GetComponent<RasterMap>();

            StartCoroutine(finalLocation());
        }

        IEnumerator finalLocation()
        {
            yield return null;

            searchingLoc.onSelect.AddListener(DisableWebView); // ���� ��Ȱ��ȭ �Լ��� onSelect �����ʷ� �߰� : �Ķ��� ui ��ġ ��
            searchingLoc.onSubmit.AddListener(OnInputEnter); // �˻� �� ����
        }

        /// <summary>
        /// �˻��� ��ġ��
        /// </summary>
        /// <param name="id"></param>
        void DisableWebView(string id) // �Ķ��� ui ��ġ �� ���並 ��Ȱ��ȭ�ϴ� �޼ҵ�
        {
            menuButton.SetActive(false); // �˻���-�޴� ��ư ��Ȱ��ȭ
            backButton.SetActive(true); // �˻���-back ��ư Ȱ��ȭ
            searhcingListCanvas.enabled = true; // �ֱٰ˻��� ��� ĵ���� Ȱ��ȭ

            if (rasterMap.webViewObject != null)
            {
                rasterMap.webViewObject.SetVisibility(false); // raster map ��Ȱ��ȭ
            }
        }

        /// <summary>
        /// �˻��� ����
        /// </summary>
        /// <param name="address"></param>
        void OnInputEnter(string address)
        {
            string finalAddress = searchingLoc.text;

            Debug.Log(finalAddress);

            // ���� �ѱ� - �˻��� ���� ��Ŀ 10�� ǥ��
            if (rasterMap.webViewObject != null)
            {
                rasterMap.webViewObject.SetVisibility(true);
                if (!string.IsNullOrEmpty(finalAddress))
                {
                    rasterMap.webViewObject.EvaluateJS($"searchPOI('{finalAddress}');");
                }
            }

            // �ڵ��ϼ� ĵ���� ����
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

        /// <summary>
        /// �˻��� - ���� �浵 ������ �ޱ�
        /// </summary>
        /// <param name="a"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetCoordinates(string a, Action callback = null)
        {
            yield return new WaitForSeconds(0.5f);

            string url = "https://apis.openapi.sk.com/tmap/pois?";
            string customUrl = "";

            customUrl += "&version=" + version;
            customUrl += "&page=" + page;
            customUrl += "&count=" + count;
            customUrl += "&callback=" + callback;
            customUrl += "&searchKeyword=" + WWW.EscapeURL(a);
            customUrl += "&appKey=" + appKey;

            url += customUrl;

            UnityWebRequest webRequest = UnityWebRequest.Get(url); // �� ��û ����
            yield return webRequest.SendWebRequest(); // �� ��û ������

            Debug.Log(url);

            // �� ��û ���� �߻��� �α� ���
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error : " + webRequest.error);
            }
            // �� ��û ����
            else
            {
                Debug.Log(webRequest.downloadHandler.text);

                // JSON ������ ����ȭ
                Root2 root = JsonConvert.DeserializeObject<Root2>(webRequest.downloadHandler.text);

                AutoComplete autoComplete = GameObject.Find("Content-AutoComplete").GetComponent<AutoComplete>();
                autoComplete.LocationName.Clear();
                autoComplete.LocaionAddress.Clear();
                name.Clear();
                address.Clear();

                if (callback != null)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        longitude.Add(root.searchPoiInfo.pois.poi[i].newAddressList.newAddress[0].centerLon);
                        latitude.Add(root.searchPoiInfo.pois.poi[i].newAddressList.newAddress[0].centerLat);
                        address.Add(root.searchPoiInfo.pois.poi[i].newAddressList.newAddress[0].fullAddressRoad);
                        name.Add(root.searchPoiInfo.pois.poi[i].name);

                        autoComplete.LocationName.Add(name[i]);
                        autoComplete.LocaionAddress.Add(address[i]);
                    }

                    callback();

                }
            }

            StopCoroutine(GetCoordinates("emty"));

        }
    }

}
