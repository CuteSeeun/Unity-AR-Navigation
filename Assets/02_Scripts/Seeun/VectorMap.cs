using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class VectorMap : MonoBehaviour
{
    private WebViewObject webViewObject;

    private float lastTouchTime;
    private const float doubleTapTime = 0.3f; // ���� ���� �����ϱ� ���� �ִ� �ð� ���� (0.3��)


    private int tapCount;
    private float lastTapTime;
    private const float doubleTapDelay = 0.3f; // ���� �� ������ ���� �ð� ����


    void Start()
    {
        StartCoroutine(VectorMapHtml()); //�� ���� �� ������ �ε��ϴ� �ڷ�ƾ �ٷ� ����
    }

    //���� �ε��ϴ� �ڷ�ƾ
    IEnumerator VectorMapHtml()
    {
        string htmlFileName = "VectorMap.html"; //�ε��� HTML ���� �̸��� ����
        string filePath = Path.Combine(Application.streamingAssetsPath, htmlFileName); // Application.streamingAssetsPath�� ���� �̸��� �����Ͽ� ��ü ���� ��� ����

        if (filePath.Contains("://"))  // ���� ��ο� "://" ���ڿ��� ���ԵǾ� �ִ��� Ȯ�� (��, ��ΰ� APK �������� = ��������� ����� ȯ�濡���� ��� Ȯ��)
        {
            UnityWebRequest request = UnityWebRequest.Get(filePath); //UnityWebRequest�� ����Ͽ� ���� ��ο��� HTML ������ ��û
            yield return request.SendWebRequest(); // ��Ʈ��ũ ��û�� ������ �Ϸ�� ������ ���

            if (request.isNetworkError || request.isHttpError) //��Ʈ��ũ ���� �Ǵ� HTTP������ �߻��ߴ��� Ȯ��.
            {
                Debug.LogError(request.error); //���� �޽����� �α׿� ���
                yield break; //���� �߻� �� �ڷ�ƾ ����
            }

            string htmlContent = request.downloadHandler.text; //��û�� ���������� �Ϸ�Ǹ� �ٿ�ε�� HTML ������ ������.
            string tempPath = Path.Combine(Application.persistentDataPath, htmlFileName); //�ӽ� ��θ� persistentDataPath�� ����
            File.WriteAllText(tempPath, htmlContent); // �ӽ� ��ο� HTML ������ ���Ϸ� �ۼ�
            filePath = "file://" + tempPath; //�ӽ� ���� ��θ� ���信�� �ε��� �� �ִ� ���·� ������Ʈ
                                             //tempPath�� ������Ʈ�� ��ο� file:// ���������� �߰��Ͽ� ���䰡 �� ����� ������ �ε�
                                             //file:// ���������� ���� ���� �ý��ۿ� �ִ� ���ҽ��� ����Ű�� �� ���

            /*
             * �ӽ� ��� ����(tempPath)
             * File.WriteAllText : �ٿ�ε���� html������ tempPath�� ���Ϸ� �����Ѵ�.
             */
        }
        else
        {
            filePath = "file://" + filePath; //���� ��ΰ� �ƴ� ���, filePath�� "file://" �������� �����Ѵ�.
        }

        //���並 �����ϱ� ���� �ڵ�
        if (webViewObject == null) //webViewObject�� �ʱ�ȭ���� �ʾҴٸ� ���� ����
        {
            webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>(); // �� GameObject�� �����ϰ� WebViewObject ������Ʈ�� �߰�
                                                                                             //�� ������Ʈ�� �� �������� ����Ƽ ���ο��� ǥ���ϰ� �����ϴ� ���������

            //�ݹ� �Լ� ����
            //Init() : ���������Ʈ�� �ʱ�ȭ�ϴ� �Լ�. �� �Լ��� �ݹ��� ����Ͽ� �ڹٽ�ũ��Ʈ���� ����Ƽ�� �޽����� ���۵� ������ �̸� ó���� �� �ִ�.
            webViewObject.Init((msg) =>
            {
                if (msg == "adjustMargins")
                {
                    AdjustWebViewMargins();
                }
                Debug.Log("CallFromJS[" + msg + "]"); // JavaScript���� �޽����� �޾��� �� �α׿� ����ϴ� �ݹ� �Լ��� �ʱ�ȭ
            });
        }
        webViewObject.LoadURL(filePath.Replace(" ", "%20")); // URL�� �ε��ϴµ�, ������ "%20"���� ġȯ
        webViewObject.SetVisibility(true); //������ ���ü��� true�� ����

        //������ ��ġ�� SetMargins �޼ҵ带 ���� �����. ĵ������ ������ ��ġ�� ������ ���� �ʰ� ��ü ���÷��� â�� ���� ������ ��ġ�� �������� ������.
        webViewObject.SetMargins(200, 1500, 200, 0); // ������ ������ �����մϴ�. (ȭ���� ������ ����)
                                                     //����, ��, ������, �Ʒ�

    }

    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastTouchTime < doubleTapTime)
            {
                // �� ��° ���� �����Ǿ��� ��
                AdjustWebViewMargins();
                lastTouchTime = 0; // Ÿ�̸� ����
            }
            else
            {
                // ù ��° ���� �����Ǿ��� �� Ÿ�̸� ����
                StartCoroutine(DelayedSingleTap());
                lastTouchTime = Time.time;
            }
        }
        */

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                tapCount++;
                if (tapCount == 1)
                {
                    lastTapTime = Time.time;
                }
            }
        }

        if (tapCount == 1 && (Time.time - lastTapTime > doubleTapDelay))
        {
            // �̱� ���� �����Ǿ��� �� ����
            OneTapWebViewMargins();
            tapCount = 0; // �� ī��Ʈ �ʱ�ȭ
        }
        else if (tapCount == 2)
        {
            // ���� ���� �����Ǿ��� �� ����
            AdjustWebViewMargins();
            tapCount = 0; // �� ī��Ʈ �ʱ�ȭ
        }


    }

    void OneTapWebViewMargins()
    {
        // �̱� �� �� ������ ���� ����
        webViewObject.SetMargins(200, 1500, 200, 0);
        Debug.Log("One tap margins set.");
    }

    void AdjustWebViewMargins()
    {
        // ���� ���, ������ 50, 100, 50, 100���� ����
        webViewObject.SetMargins(50, 100, 50, 100);
    }

    /*
    IEnumerator DelayedSingleTap()
    {
        yield return new WaitForSeconds(doubleTapTime);
        if (Time.time - lastTouchTime >= doubleTapTime)
        {
            // �ð� ���̰� ���� �� �ð����� ��� �̱� ������ ����
            webViewObject.SetMargins(200, 1500, 200, 0);
        }
    }
    */
    

    public void SendAddress(string data)
    {
        Debug.Log(data);
    }

}
