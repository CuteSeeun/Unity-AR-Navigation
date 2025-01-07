using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    //각 페이지 상위노드
    private VisualElement _Mission1Container;
    private VisualElement _MyPageContainer;
    private VisualElement _ExitAppContainer;

    //미션1
    private Button _Event1OpenButton;
    private Button _Event1CloseButton;
    //마이페이지
    private Button _MyPageopenButton;
    private Button _MyPagecloseButton;
    //어플 종료
    private Button _ExitAppOpenButton;
    private Button _ExitAppCloseButton;

    private VisualElement _Mission1; // 미션1
    private VisualElement _MyPage; // 마이페이지
    private VisualElement _Background; // 어둡게 뒤배경 깔아줌
    private VisualElement _ExitApp; // 어플종료

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        //각 페이지에 해당하는 상위노드를 불러오는 기능 
        _MyPageContainer = root.Q<VisualElement>("Container_Right");
        _ExitAppContainer = root.Q<VisualElement>("Container_Bottom");
        _Mission1Container = root.Q<VisualElement>("Container_Left");

        //미션1 버튼
        _Event1OpenButton = root.Q<Button>("Button_Event1");
        _Event1CloseButton = root.Q<Button>("Button_LeftClose");

        _Mission1 = root.Q<VisualElement>("LeftSheet");
        _Background = root.Q<VisualElement>("Scrim3");

        //마이 페이지
        _MyPageopenButton = root.Q<Button>("Button_Open");
        _MyPagecloseButton = root.Q<Button>("Exit");

        _MyPage = root.Q<VisualElement>("RightSheet");
        _Background = root.Q<VisualElement>("Scrim");

        //어플 종료
        _ExitAppOpenButton = root.Q<Button>("Button_BottomOpen");
        _ExitAppCloseButton = root.Q<Button>("Button_BottomClose");

        _ExitApp = root.Q<VisualElement>("BottomSheet");
        _Background = root.Q<VisualElement>("Scrim2");


        _MyPageContainer.style.display = DisplayStyle.None;
        _ExitAppContainer.style.display = DisplayStyle.None;
         _Mission1Container.style.display = DisplayStyle.None;

        _Event1OpenButton.RegisterCallback<ClickEvent>(OnOpenButtonLeftSheet);
        _Event1CloseButton.RegisterCallback<ClickEvent>(OnCloseButtonLeftSheet);

        //마이 페이지
        _MyPageopenButton.RegisterCallback<ClickEvent>(OnOpenButtonRightSheet);
        _MyPagecloseButton.RegisterCallback<ClickEvent>(OnCloseButtonRightSheet);
        //어플 종료
        _ExitAppOpenButton.RegisterCallback<ClickEvent>(OnOpenButtonBottomSheet);
        _ExitAppCloseButton.RegisterCallback<ClickEvent>(OnCloseButtonBottomSheet);
    }

    // 페이지 여는 함수

    private void OnOpenButtonLeftSheet(ClickEvent evt)
    {
        _Mission1Container.style.display = DisplayStyle.Flex;

        _Mission1.AddToClassList("BottomSheet--Up");
        _Background.AddToClassList("Scrim--fadein");
    }

    private void OnOpenButtonRightSheet(ClickEvent evt)
    {
        _MyPageContainer.style.display = DisplayStyle.Flex;

        _MyPage.AddToClassList("RightSheet--Left");
        _Background.AddToClassList("Scrim--fadein");
    }

    private void OnOpenButtonBottomSheet(ClickEvent evt)
    {
        _ExitAppContainer.style.display = DisplayStyle.Flex;

        _ExitApp.AddToClassList("BottomSheet--Up");
        _Background.AddToClassList("Scrim--fadein");
    }


    // 페이지 닫는 함수

    // 미션1
    private void OnCloseButtonLeftSheet(ClickEvent evt)
    {
        _Mission1Container.style.display = DisplayStyle.None;

        _Mission1.RemoveFromClassList("BottomSheet--Up");
        _Background.RemoveFromClassList("Scrim--fadein");
    }

    private void OnCloseButtonRightSheet(ClickEvent evt)
    {
        _MyPageContainer.style.display = DisplayStyle.None;

        _MyPage.RemoveFromClassList("RightSheet--Left");
        _Background.RemoveFromClassList("Scrim--fadein");
    }

    private void OnCloseButtonBottomSheet(ClickEvent evt)
    {
        _ExitAppContainer.style.display = DisplayStyle.None;

        _ExitApp.RemoveFromClassList("BottomSheet--Up");
        _Background.RemoveFromClassList("Scrim--fadein");
    }
}
