using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NaviMonsterStart : MonoBehaviour
{
        public void SceneChange()
        {
            SceneManager.LoadScene("UIToolkit");
        }
        /*public void acredev2SceneChange()
        {
        SceneManager.LoadScene("acredev2");
        }*/
    
}
