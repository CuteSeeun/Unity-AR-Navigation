using ARNavi.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ARNavi.Controller
{
    public class AutoComplete : MonoBehaviour
    {
        private TMP_InputField SearchingInputField;

        public GameObject ButtonAutoComplete;
        List<GameObject> AutoBarList = new List<GameObject>();

        public List<string> LocationName = new List<string>();
        public List<string> LocaionAddress = new List<string>();

        public SearchingBar searchingBar;

        GameObject auto;

        Canvas canvasAutoComplete;
        Canvas canvasSearchingList;

        void Start()
        {
            searchingBar = GameObject.Find("InputField (TMP)-Searching").GetComponent<SearchingBar>();
            SearchingInputField = searchingBar.searchingLoc;

            StartCoroutine(InputLocation());

            ButtonAutoComplete = Resources.Load<GameObject>("Button-AutoComplete");

            canvasAutoComplete = GameObject.Find("Canvas_autoComplete").GetComponent<Canvas>();
            canvasSearchingList = GameObject.Find("Canvas_searchingList").GetComponent<Canvas>();

        }

        IEnumerator InputLocation()
        {
            SearchingInputField.onValueChanged.AddListener(SearchingList);

            yield return new WaitForSeconds(0.1f);
        }

        void SearchingList(string str)
        {
            canvasAutoComplete.enabled = true;
            canvasSearchingList.enabled = false;

            if (SearchingInputField.text.Length >= 1)
            {
                searchingBar.StartCoroutine(searchingBar.GetCoordinates(SearchingInputField.text, () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (AutoBarList.Count < 11)
                        {
                            Debug.Log("안들어와?");
                            auto = Instantiate(ButtonAutoComplete);
                            auto.transform.SetParent(gameObject.transform, false);
                            AutoBarList.Add(auto.gameObject);

                        }
                    }
                    FillSearchingList();

                    Debug.Log("AutoBarList" + AutoBarList.Count);
                    Debug.Log("AutoBarList" + searchingBar.address.Count);
                }));

            }
        }


        void Update()
        {

        }

        void FillSearchingList()
        {

            Debug.Log("list :" + AutoBarList.Count + " : " + LocationName.Count);
            for (int j = 0; j < 10; j++)
            {
                // AutoBarList.Clear();
                //AutoBarList[j].GetComponentInChildren<TextMeshProUGUI>().text = LocationName[j];

                TextMeshProUGUI[] autoCompleteText = AutoBarList[j].GetComponentsInChildren<TextMeshProUGUI>();
                autoCompleteText[0].text = LocationName[j];
                autoCompleteText[1].text = LocaionAddress[j];
            }
        }
    }
}

