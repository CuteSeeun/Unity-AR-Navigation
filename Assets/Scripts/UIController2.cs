using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController2 : MonoBehaviour
{
    //이름변경하기
    private VisualElement SideMenu0;
    private TextField nameTextField;
    private UnityEngine.UIElements.Label nameLabel;


    private VisualElement SideMenu1;
    private VisualElement SideMenu2;
    private VisualElement SideMenu3;
    private VisualElement SideMenu4;

    private Button _Button;
    

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        SideMenu0 = root.Q<VisualElement>("SideMenu0");
        //이름 바꾸기 추후 수정 필요 
        nameTextField = SideMenu0.Q<TextField>("NameTextField");
        nameLabel = root.Q<UnityEngine.UIElements.Label>("Name");

        SideMenu0.style.display = DisplayStyle.None;
        _Button = root.Q<Button>("EditName");
        _Button.clicked += OnEditNameButtonClick;
        //_Button.RegisterCallback<ClickEvent>(EditName);
      

        SideMenu1 = root.Q<VisualElement>("SideMenu1");
        SideMenu1.style.display = DisplayStyle.None;
        _Button = root.Q<Button>("Home");
        _Button.RegisterCallback<ClickEvent>(GoHome);

        SideMenu2 = root.Q<VisualElement>("SideMenu2");
        SideMenu2.style.display = DisplayStyle.None;
        _Button = root.Q<Button>("Monsters");
        _Button.RegisterCallback<ClickEvent>(GoMonsters);

        SideMenu3 = root.Q<VisualElement>("SideMenu3");
        SideMenu3.style.display = DisplayStyle.None;
        _Button = root.Q<Button>("Volume");
        _Button.RegisterCallback<ClickEvent>(GoVolume);

        SideMenu4 = root.Q<VisualElement>("SideMenu4");
        SideMenu4.style.display = DisplayStyle.None;
        _Button = root.Q<Button>("Setting");
        _Button.RegisterCallback<ClickEvent>(GoSetting);


    }

    private void OnEditNameButtonClick()
    {
        SideMenu0.style.display = DisplayStyle.Flex;
    }

    private void GoHome(ClickEvent evt)
    {
        SideMenu1.style.display = DisplayStyle.Flex;
        SideMenu2.style.display = DisplayStyle.None;
        SideMenu3.style.display = DisplayStyle.None;
    }

    private void GoMonsters(ClickEvent evt)
    {
        SideMenu1.style.display = DisplayStyle.None;
        SideMenu2.style.display = DisplayStyle.Flex;
        SideMenu3.style.display = DisplayStyle.None;
    }

    private void GoVolume(ClickEvent evt)
    {
        SideMenu1.style.display = DisplayStyle.None;
        SideMenu2.style.display = DisplayStyle.None;
        SideMenu3.style.display = DisplayStyle.Flex;
    }

    private void GoSetting(ClickEvent evt)
    {
        SideMenu4.style.display = DisplayStyle.Flex;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
