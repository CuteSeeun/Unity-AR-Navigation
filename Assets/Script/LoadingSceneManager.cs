using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 앱 시작시 로딩 관련 스크립트
/// </summary>
public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;

    [SerializeField]
    Image progressBar;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    /// <summary>
    /// 로딩 씬으로 이동
    /// </summary>
    /// <param name="sceneName"></param>
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    /// <summary>
    /// 로딩 끝나면 다음 씬으로 이동
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync("MainScene");

        // 다음 씬으로 바로 넘어가지 않도록 설정
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                // 5초 이상 경과하면 넘어감
                if (progressBar.fillAmount == 1.0f && timer >= 5.0f)
                {
                    // 다음 씬으로 넘어감
                    op.allowSceneActivation = true; 
                    yield break;
                }
            }
        }
    }
}