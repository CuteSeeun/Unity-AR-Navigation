using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 뒤로 가기 누르면 앱 나가는 기능, 차후 AppManager과 합칠 예정
/// </summary>
public class EscapeManager : MonoBehaviour
{
    int ClickCount = 0;
    // 텍스트를 표시할 UI 텍스트
    public Text notificationText; 

    /// <summary>
    /// 클릭 시 앱에서 나가기 
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClickCount++;
            if (!IsInvoking("DoubleClick"))
                Invoke("DoubleClick", 1.0f);
                ShowNotification("나가려면 한번 더 클릭해주세요.");

        }
        else if (ClickCount == 2)
        {
            CancelInvoke("DoubleClick");
            Application.Quit();
        }
    }

    /// <summary>
    /// 두번 클릭
    /// </summary>
    void DoubleClick()
    {
        ClickCount = 0;
    }

    /// <summary>
    /// 한번 누르면 나가는 메세지 불러오기
    /// </summary>
    /// <param name="message"></param>
    void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
            Invoke("HideNotification", 1.0f);
        }
    }

    /// <summary>
    /// 안나가려면 가만히 있기
    /// </summary>
    void HideNotification()
    {
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }
}
