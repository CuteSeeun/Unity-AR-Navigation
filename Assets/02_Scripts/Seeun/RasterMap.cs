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
public class RasterMap : MonoBehaviour //지도를 로드만 하기. 다른 클래스에서 이 클래스 부르기.
{
    public WebViewObject webViewObject;

    //지도 로드, 검색 버튼 클릭 리스너
    void Start()
    {
        StartCoroutine(LoadLocalHtml()); //앱 시작 시 지도를 로드하는 코루틴 바로 실행

        Permission.RequestUserPermission(Permission.FineLocation);// 위치 권한 요청
    }

    IEnumerator LoadLocalHtml()
    {
        string htmlFileName = "Tmap.html"; //로드할 HTML 파일 이름을 지정
        string filePath = Path.Combine(Application.streamingAssetsPath, htmlFileName); // Application.streamingAssetsPath와 파일 이름을 결합하여 전체 파일 경로 생성

        if (filePath.Contains("://"))  // 파일 경로에 "://" 문자열이 포함되어 있는지 확인 (즉, 경로가 APK 내부인지 = 통상적으로 모바일 환경에서의 경로 확인)
        {
            UnityWebRequest request = UnityWebRequest.Get(filePath); //UnityWebRequest를 사용하여 파일 경로에서 HTML 파일을 요청
            yield return request.SendWebRequest(); // 네트워크 요청을 보내고 완료될 때까지 대기

            if (request.isNetworkError || request.isHttpError) //네트워크 에러 또는 HTTP에러가 발생했는지 확인.
            {
                Debug.LogError(request.error); //에러 메시지를 로그에 출력
                yield break; //에러 발생 시 코루틴 중지
            }

            string htmlContent = request.downloadHandler.text; //요청이 성공적으로 완료되면 다운로드된 HTML 내용을 가져옴.
            string tempPath = Path.Combine(Application.persistentDataPath, htmlFileName); //임시 경로를 persistentDataPath에 생성
            File.WriteAllText(tempPath, htmlContent); // 임시 경로에 HTML 내용을 파일로 작성
            filePath = "file://" + tempPath; //임시 파일 경로를 웹뷰에서 로드할 수 있는 형태로 업데이트
                                             //tempPath로 업데이트된 경로에 file:// 프로토콜을 추가하여 웹뷰가 이 경로의 파일을 로드
                                             //file:// 프로토콜은 로컬 파일 시스템에 있는 리소스를 가리키는 데 사용

            /*
             * 임시 경로 생성(tempPath)
             * File.WriteAllText : 다운로드받은 html내용을 tempPath에 파일로 저장한다.
             */
        }
        else
        {
            filePath = "file://" + filePath; //로컬 경로가 아닌 경우, filePath를 "file://" 형식으로 포맷한다.
        }

        //웹뷰를 관리하기 위한 코드
        if (webViewObject == null) //webViewObject가 초기화되지 않았다면 새로 생성
        {
            webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>(); // 새 GameObject를 생성하고 WebViewObject 컴포넌트를 추가
                                                                                             //이 컴포넌트는 웹 페이지를 유니티 내부에서 표시하고 관리하는 기능을제공

            //콜백 함수 설정
            //Init() : 웹뷰오브젝트를 초기화하는 함수. 이 함수에 콜백을 등록하여 자바스크립트에서 유니티로 메시지가 전송될 때마다 이를 처리할 수 있다.
            webViewObject.Init((msg) =>
            {
                Debug.Log("CallFromJS[" + msg + "]"); // JavaScript에서 메시지를 받았을 때 로그에 출력하는 콜백 함수를 초기화


                // 메시지에 따라 다른 함수를 호출
                if (msg == "loadARScene")
                {
                    //LoadARGuideScene();
                }

            });
        }
        webViewObject.LoadURL(filePath.Replace(" ", "%20")); // URL을 로드하는데, 공백은 "%20"으로 치환
        webViewObject.SetVisibility(true); //웹뷰의 가시성을 true로 설정

        //웹뷰의 위치는 SetMargins 메소드를 통해 제어됨. 캔버스의 구조나 위치에 영향을 받지 않고 전체 디스플레이 창에 대한 절대적 위치를 기준으로 설정됨.
        webViewObject.SetMargins(0, 400, 0, 200); // 웹뷰의 마진을 설정합니다. (화면의 여백을 조정)

    }

    /*
    // 자바스크립트 출발 버튼 클릭 시 호출_응 안됨
    
    public void StartNavigation()
    {
        initUI.SetActive(false);     // InitUI를 비활성화
        directionUI.SetActive(true);  // DirectionUI를 활성화

        // 출발 경로를 탐색하는 추가 코드 작성
        // Debug.Log("출발 경로를 탐색합니다.");
    }
    

    // 자바스크립트 도착 버튼 클릭 시 호출_응 안됨
    public void EndNavigation()
    {
        initUI.SetActive(false);     // InitUI를 비활성화
        directionUI.SetActive(true);  // DirectionUI를 활성화
    }

    */

    /*
    //자바스크립트에서 유니티 호출 테스트 코드
    public void ReceiveMessage(string message)
    {
        // initUI가 활성화되어 있을 때만 아래 코드 실행
        if (initUI.activeSelf)
        {
            initUI.SetActive(false);     // InitUI를 비활성화
            directionUI.SetActive(true);  // DirectionUI를 활성화

            startUI.text = message; // TMP_InputField의 텍스트를 업데이트
        }

    }
    */


    //전체 코드 실행 흐름
    /*
     * -TMP_InputField 'searchUI'와 Button'searchButton' : 이 두 ui컴포넌트는 사용자로부터 검색어를 입력받고 검색 버튼이 클릭될 때 해당 검색어로 poi 검색을 시작한다.
     * -Start 메서드 : 애플리케이션 시작 시 필요한 초기화를 수행. 웹뷰를 로드하고 검색 버튼의 클릭 이벤트에 리스너를 추가
     * -OnSearchButtonClicked 메서드 : 검색 버튼이 클릭되면 실행됨. 검색어가 유용한 경우 html파일의 searchPOI 함수를 호출
     * -LoadLocalHtml 코루틴 : 지정된 HTML파일을 로컬에서 로드. 로컬 파일 시스템에서 HTML파일을 로드하기 위해 필요한 경로 처리를 수행하고 웹뷰 객체를 생성 및 초기화.
     * -webViewObject.EvaluateJS : 검색어를 인수로 사용하여 HTML내부의 자바스크립트 함수를 호출
     */
}