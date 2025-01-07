using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    //�� ������ �������
    private VisualElement _Mission1Container;
    private VisualElement _MyPageContainer;
    private VisualElement _ExitAppContainer;

    //�̼�1
    private Button _Event1OpenButton;
    private Button _Event1CloseButton;
    //����������
    private Button _MyPageopenButton;
    private Button _MyPagecloseButton;
    //���� ����
    private Button _ExitAppOpenButton;
    private Button _ExitAppCloseButton;

    private VisualElement _Mission1; // �̼�1
    private VisualElement _MyPage; // ����������
    private VisualElement _Background; // ��Ӱ� �ڹ�� �����
    private VisualElement _ExitApp; // ��������

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        //�� �������� �ش��ϴ� ������带 �ҷ����� ��� 
        _MyPageContainer = root.Q<VisualElement>("Container_Right");
        _ExitAppContainer = root.Q<VisualElement>("Container_Bottom");
        _Mission1Container = root.Q<VisualElement>("Container_Left");

        //�̼�1 ��ư
        _Event1OpenButton = root.Q<Button>("Button_Event1");
        _Event1CloseButton = root.Q<Button>("Button_LeftClose");

        _Mission1 = root.Q<VisualElement>("LeftSheet");
        _Background = root.Q<VisualElement>("Scrim3");

        //���� ������
        _MyPageopenButton = root.Q<Button>("Button_Open");
        _MyPagecloseButton = root.Q<Button>("Exit");

        _MyPage = root.Q<VisualElement>("RightSheet");
        _Background = root.Q<VisualElement>("Scrim");

        //���� ����
        _ExitAppOpenButton = root.Q<Button>("Button_BottomOpen");
        _ExitAppCloseButton = root.Q<Button>("Button_BottomClose");

        _ExitApp = root.Q<VisualElement>("BottomSheet");
        _Background = root.Q<VisualElement>("Scrim2");


        _MyPageContainer.style.display = DisplayStyle.None;
        _ExitAppContainer.style.display = DisplayStyle.None;
         _Mission1Container.style.display = DisplayStyle.None;

        _Event1OpenButton.RegisterCallback<ClickEvent>(OnOpenButtonLeftSheet);
        _Event1CloseButton.RegisterCallback<ClickEvent>(OnCloseButtonLeftSheet);

        //���� ������
        _MyPageopenButton.RegisterCallback<ClickEvent>(OnOpenButtonRightSheet);
        _MyPagecloseButton.RegisterCallback<ClickEvent>(OnCloseButtonRightSheet);
        //���� ����
        _ExitAppOpenButton.RegisterCallback<ClickEvent>(OnOpenButtonBottomSheet);
        _ExitAppCloseButton.RegisterCallback<ClickEvent>(OnCloseButtonBottomSheet);
    }

    // ������ ���� �Լ�

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


    // ������ �ݴ� �Լ�

    // �̼�1
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
