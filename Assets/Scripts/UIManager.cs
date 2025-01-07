using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
   /*
    public VisualElement firstScreen;
    public VisualElement _secondScreen;

    public VisualElement rootVisualElement;

    void Start()
    {
        LoadAndShowFirstScreen();
        LoadAndHideSecondScreen();

        Button button = firstScreen.Q<Button>("ButtonMonsterChange");
        if(button != null )
        {
            button.clickable.clicked += SwitchToSecondScreen;
            Debug.Log("눌리고는 있음");
        }
    }

    void LoadAndShowFirstScreen()
    {
        VisualTreeAsset firstScreenAsset = Resources.Load<VisualTreeAsset>("UI/UI_JH_240422.uxml");

        if(firstScreenAsset != null )
        {
            firstScreen = firstScreenAsset.CloneTree().contentContainer;
            rootVisualElement.Add(firstScreen); 
        }
    }

    void LoadAndHideSecondScreen()
    {
        VisualTreeAsset secondScreenAsset = Resources.Load<VisualTreeAsset>("UI/UI_JH_MonsterList");
        if(secondScreenAsset != null )
        {
            _secondScreen = secondScreenAsset.CloneTree();
            _secondScreen.style.display = DisplayStyle.None;
            rootVisualElement.Add(_secondScreen);
        }
    }

    public void SwitchToSecondScreen()
    {
        firstScreen.style.display = DisplayStyle.None;
        _secondScreen.style.display = DisplayStyle.Flex;
    }
    */
    public VisualElement firstScreen;
    public VisualElement secondScreen;

    public GameObject _secondScreen;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        firstScreen = root.Q<VisualElement>("MainScreen");
        firstScreen.style.display = DisplayStyle.Flex;

        if(_secondScreen != null)
        {
            _secondScreen.SetActive(false);
        }

        //메인메뉴의 몬스터체인지 버튼
        Button button = firstScreen.Q<Button>("ButtonMonsterChange");
        if(button != null)
        {
            button.clickable.clicked += OnFirstButtonClicked;
        }
        
        //사이드메뉴의 메인캐릭터변경하기 버튼
        Button button2 = firstScreen.Q<Button>("MainCharacterChange");
        if(button2 != null)
        {
            button2.clickable.clicked += OnFirstButtonClicked;
        }

        /*
        Button button2 = secondScreen.Q<Button>("SelectMonsterButton");
        if(button2 != null)
        {
            button2.clickable.clicked += OnSecondButtonClicked;
        }

        var root2 = GetComponent<UIDocument>().rootVisualElement;
        secondScreen = root2.Q<VisualElement>("MainScreen");

        */
    }

    void OnFirstButtonClicked()
    {
        firstScreen.style.display = DisplayStyle.None;

        if(_secondScreen != null )
        {
            _secondScreen.SetActive(true);
        }
    }

    /*
     *
    void OnSecondButtonClicked()
    {
       if( _secondScreen != null)
        {
         _secondScreen.SetActive(false);
        }
        
        firstScreen.style.display = DisplayStyle.Flex;

    }
    */
}
