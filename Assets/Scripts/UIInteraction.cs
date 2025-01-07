using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIInteraction : MonoBehaviour
{
    public UIDocument uiDoc;

    private Button SelectMonsterButton;
    void Start()
    {
        SelectMonsterButton = uiDoc.rootVisualElement.Q<Button>("SelectMonsterButton");
        SelectMonsterButton.clicked += UiButton_Clicked;
    }

    private void UiButton_Clicked()
    {
        Debug.Log("the clicked event happened");
        SceneManager.LoadScene("UIToolkit");
    }

    private void TestClickEvent(ClickEvent clickEv)
    {
        Debug.Log("registered callback click");
    }

   
}
