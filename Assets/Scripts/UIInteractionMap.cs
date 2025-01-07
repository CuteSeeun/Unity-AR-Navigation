using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIInteractionMap : MonoBehaviour
{
    public UIDocument uiDoc;


    //메인메뉴에서 각각 버튼이 하는 역할들
    private Button Button_RasterMap;
    private Button DogMonsterA_Button;

    private Button GoHome;
    private Button GoMap;
    private Button GoAR;

    private Button GoMimic;

    void Start()
    {
        Button_RasterMap = uiDoc.rootVisualElement.Q<Button>("Button_RasterMap");
        GoMap = uiDoc.rootVisualElement.Q<Button>("GoMap");

        Button_RasterMap.clicked += RasterMap_Clicked;
        GoMap.clicked += RasterMap_Clicked;

        //캐릭터모아보기의 첫번째 몬스터A를 눌렀을때 씬을  넘겨 몬스터A를 소개하는 영상이 나오게끔 
        DogMonsterA_Button = uiDoc.rootVisualElement.Q<Button>("DogMonsterA_Button");
        DogMonsterA_Button.clicked += GoDogMonsterA;

        GoHome = uiDoc.rootVisualElement.Q<Button>("GoHome");
        GoHome.clicked += GoMainMenu;

        //GoMap = uiDoc.rootVisualElement.Q<Button>("GoMap");
        GoAR = uiDoc.rootVisualElement.Q<Button>("GoAR");
        GoAR.clicked += GoARWindow;

        GoMimic = uiDoc.rootVisualElement.Q<Button>("Button_GoMimic");
        GoMimic.clicked += GoMimicFight;


    }

    private void GoMimicFight()
    {
        SceneManager.LoadScene("AR_Welcome");

    }

    private void GoMainMenu()
    {
        SceneManager.LoadScene("UIToolkit");
    }

    private void GoARWindow()
    {
        SceneManager.LoadScene("GeospatialArf4_Sumin");

    }

    private void RasterMap_Clicked()
    {
        Debug.Log("the clicked event happened");
        SceneManager.LoadScene("Chayeong_02");
    }

    private void GoDogMonsterA()
    {
        SceneManager.LoadScene("Introduce_DogMonsterA");
    }
}
