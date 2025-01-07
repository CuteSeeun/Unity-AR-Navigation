using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIInteraction_MainMenu : MonoBehaviour
{
    public UIDocument uiDoc;

    private Button Button_MainMenu;
    private Button Button_GoAR;


    void Start()
    {
        Button_MainMenu = uiDoc.rootVisualElement.Q<Button>("Button_MainMenu");
        Button_GoAR = uiDoc.rootVisualElement.Q<Button>("Button_GoAR");
        Button_MainMenu.clicked += GoMainMenu;
        Button_GoAR.clicked += GoARWindow;

    }

    private void GoMainMenu()
    {
        SceneManager.LoadScene("UIToolkit");
    }

    private void GoARWindow()
    {
        SceneManager.LoadScene("ARPetScene");
    }



}
