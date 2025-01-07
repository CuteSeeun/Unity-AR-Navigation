using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecentSearchingList : MonoBehaviour
{
    public TMP_InputField SearchingBar;
    private GameObject recentSearchingButtonPrefab;
    public List<string> recentSearching = new List<string>();
    private GameObject recentSearchingButton;
    private List<GameObject> searchingListButton = new List<GameObject>();

    void Start()
    {
        SearchingBar = GameObject.Find("InputField (TMP)-Searching").GetComponent<TMP_InputField>();
        recentSearchingButtonPrefab = Resources.Load<GameObject>("Button-SearhcingList");
        StartCoroutine(EndSearch());
    }

    IEnumerator EndSearch()
    {
        yield return new WaitForSeconds(0.5f);
        SearchingBar.onEndEdit.AddListener(RecentList);
    }


    
    void RecentList(string str)
    {
        // 검색항목 리스트에 추가
        recentSearching.Add(SearchingBar.text);

        // 버튼 프리팹 생성
        recentSearchingButton = Instantiate(recentSearchingButtonPrefab);
        recentSearchingButton.transform.SetParent(gameObject.transform, false);
        
        recentSearchingButton.GetComponentInChildren<TextMeshProUGUI>().text = SearchingBar.text;

        searchingListButton.Add(recentSearchingButton);
    }

    public void OnSearchingHistoryClear()
    {
        recentSearching.Clear();

        foreach(GameObject button in searchingListButton)
        {
            Destroy(button);
        }
        searchingListButton.Clear();
    }
}
