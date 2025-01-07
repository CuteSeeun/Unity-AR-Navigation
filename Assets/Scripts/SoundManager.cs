using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{

    public AudioSource bgm;
    private bool isMuted = false;

    //�Ҹ� ���� �ѱ�
    private Button soundOff;
    private Button soundOn;

    private Slider volumeSlider;

    private const string MuteKey = "IsMuted";


    void Start()
    {
       

        var root = GetComponent<UIDocument>().rootVisualElement;    

        //mainScreen = root.Q<VisualElement>("MainScreen");

        soundOff = root.Q<Button>("Button_SoundOff");
        soundOff.style.display = DisplayStyle.Flex;

        soundOn = root.Q<Button>("Button_SoundOn");
        soundOn.style.display = DisplayStyle.None;

        isMuted = PlayerPrefs.GetInt(MuteKey, 0) == 1;


        UpdateSoundButton();

        soundOff.clickable.clicked += ToggleSound;
        soundOn.clickable.clicked += ToggleSound;

        //soundOff.RegisterCallback<ClickEvent>(PlayOffBackgroundMusic);
        //soundOn.RegisterCallback<ClickEvent>(PlayOnBackgroundMusic);

        //Slider��� ��������
        volumeSlider = root.Q<Slider>("volumeSlider");

        if(volumeSlider != null)
        {
            volumeSlider.RegisterValueChangedCallback(OnVolumeSliderChanged);
        }
        else
        {
            Debug.LogError("Sliderȣ�� �ȵ�");
        }
        
    }

    private void OnVolumeSliderChanged(ChangeEvent<float> evt)
    {
        UpdateSound();
    }

    private void UpdateSound()
    {
        if(bgm != null)
        {
            bgm.volume = isMuted ? 0f : volumeSlider.value;
        }
        else
        {
            Debug.LogError("Background music (AudioSource) is not assigned!");
        }
    }

    private void ToggleSound()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt(MuteKey, isMuted ? 1 : 0); // ���Ұ� ���� ����


        if (isMuted)
        {
            bgm.Pause();
            Debug.Log("������");
        }
        else
        {
            bgm.Play();
            Debug.Log("�÷��̵ǰ� ����");
        }

        UpdateSoundButton();
    }

    private void UpdateSoundButton()
    {
        soundOff.style.display = isMuted ? DisplayStyle.None : DisplayStyle.Flex;
        soundOn.style.display = isMuted ? DisplayStyle.Flex : DisplayStyle.None;

        //if (isMuted)
        //{
        //    soundOff.style.display = DisplayStyle.None;
        //    soundOn.style.display = DisplayStyle.Flex;
        //}
        //else
        //{
        //    soundOff.style.display = DisplayStyle.Flex;
        //    soundOn.style.display = DisplayStyle.None;
        //}
    }

    /*
    private void PlayOffBackgroundMusic(ClickEvent evt)
    {
        soundOff.style.display = DisplayStyle.None;
        soundOn.style.display = DisplayStyle.Flex;
    }

    private void PlayOnBackgroundMusic(ClickEvent evt)
    {
        soundOff.style.display = DisplayStyle.Flex;
        soundOn.style.display = DisplayStyle.None;

    }
    */
}
