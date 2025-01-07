using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
public class RasterMap : MonoBehaviour //������ �ε常 �ϱ�. �ٸ� Ŭ�������� �� Ŭ���� �θ���.
{
    public WebViewObject webViewObject;

    //���� �ε�, �˻� ��ư Ŭ�� ������
    void Start()
    {
        StartCoroutine(LoadLocalHtml()); //�� ���� �� ������ �ε��ϴ� �ڷ�ƾ �ٷ� ����

        Permission.RequestUserPermission(Permission.FineLocation);// ��ġ ���� ��û
    }

    IEnumerator LoadLocalHtml()
    {
        string htmlFileName = "Tmap.html"; //�ε��� HTML ���� �̸��� ����
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
                Debug.Log("CallFromJS[" + msg + "]"); // JavaScript���� �޽����� �޾��� �� �α׿� ����ϴ� �ݹ� �Լ��� �ʱ�ȭ


                // �޽����� ���� �ٸ� �Լ��� ȣ��
                if (msg == "loadARScene")
                {
                    //LoadARGuideScene();
                }

            });
        }
        webViewObject.LoadURL(filePath.Replace(" ", "%20")); // URL�� �ε��ϴµ�, ������ "%20"���� ġȯ
        webViewObject.SetVisibility(true); //������ ���ü��� true�� ����

        //������ ��ġ�� SetMargins �޼ҵ带 ���� �����. ĵ������ ������ ��ġ�� ������ ���� �ʰ� ��ü ���÷��� â�� ���� ������ ��ġ�� �������� ������.
        webViewObject.SetMargins(0, 400, 0, 200); // ������ ������ �����մϴ�. (ȭ���� ������ ����)

    }

    /*
    // �ڹٽ�ũ��Ʈ ��� ��ư Ŭ�� �� ȣ��_�� �ȵ�
    
    public void StartNavigation()
    {
        initUI.SetActive(false);     // InitUI�� ��Ȱ��ȭ
        directionUI.SetActive(true);  // DirectionUI�� Ȱ��ȭ

        // ��� ��θ� Ž���ϴ� �߰� �ڵ� �ۼ�
        // Debug.Log("��� ��θ� Ž���մϴ�.");
    }
    

    // �ڹٽ�ũ��Ʈ ���� ��ư Ŭ�� �� ȣ��_�� �ȵ�
    public void EndNavigation()
    {
        initUI.SetActive(false);     // InitUI�� ��Ȱ��ȭ
        directionUI.SetActive(true);  // DirectionUI�� Ȱ��ȭ
    }

    */

    /*
    //�ڹٽ�ũ��Ʈ���� ����Ƽ ȣ�� �׽�Ʈ �ڵ�
    public void ReceiveMessage(string message)
    {
        // initUI�� Ȱ��ȭ�Ǿ� ���� ���� �Ʒ� �ڵ� ����
        if (initUI.activeSelf)
        {
            initUI.SetActive(false);     // InitUI�� ��Ȱ��ȭ
            directionUI.SetActive(true);  // DirectionUI�� Ȱ��ȭ

            startUI.text = message; // TMP_InputField�� �ؽ�Ʈ�� ������Ʈ
        }

    }
    */


    //��ü �ڵ� ���� �帧
    /*
     * -TMP_InputField 'searchUI'�� Button'searchButton' : �� �� ui������Ʈ�� ����ڷκ��� �˻�� �Է¹ް� �˻� ��ư�� Ŭ���� �� �ش� �˻���� poi �˻��� �����Ѵ�.
     * -Start �޼��� : ���ø����̼� ���� �� �ʿ��� �ʱ�ȭ�� ����. ���並 �ε��ϰ� �˻� ��ư�� Ŭ�� �̺�Ʈ�� �����ʸ� �߰�
     * -OnSearchButtonClicked �޼��� : �˻� ��ư�� Ŭ���Ǹ� �����. �˻�� ������ ��� html������ searchPOI �Լ��� ȣ��
     * -LoadLocalHtml �ڷ�ƾ : ������ HTML������ ���ÿ��� �ε�. ���� ���� �ý��ۿ��� HTML������ �ε��ϱ� ���� �ʿ��� ��� ó���� �����ϰ� ���� ��ü�� ���� �� �ʱ�ȭ.
     * -webViewObject.EvaluateJS : �˻�� �μ��� ����Ͽ� HTML������ �ڹٽ�ũ��Ʈ �Լ��� ȣ��
     */
}