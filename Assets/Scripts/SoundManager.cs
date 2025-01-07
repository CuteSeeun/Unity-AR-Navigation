using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{

    public AudioSource bgm;
    private bool isMuted = false;

    //소리 껐다 켜기
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

        //Slider요소 가져오기
        volumeSlider = root.Q<Slider>("volumeSlider");

        if(volumeSlider != null)
        {
            volumeSlider.RegisterValueChangedCallback(OnVolumeSliderChanged);
        }
        else
        {
            Debug.LogError("Slider호출 안됨");
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
        PlayerPrefs.SetInt(MuteKey, isMuted ? 1 : 0); // 음소거 상태 저장


        if (isMuted)
        {
            bgm.Pause();
            Debug.Log("멈췄음");
        }
        else
        {
            bgm.Play();
            Debug.Log("플레이되고 있음");
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
