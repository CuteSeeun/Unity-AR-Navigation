using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class VectorMap : MonoBehaviour
{
    private WebViewObject webViewObject;

    private float lastTouchTime;
    private const float doubleTapTime = 0.3f; // 더블 탭을 감지하기 위한 최대 시간 간격 (0.3초)


    private int tapCount;
    private float lastTapTime;
    private const float doubleTapDelay = 0.3f; // 더블 탭 감지를 위한 시간 간격


    void Start()
    {
        StartCoroutine(VectorMapHtml()); //앱 시작 시 지도를 로드하는 코루틴 바로 실행
    }

    //지도 로드하는 코루틴
    IEnumerator VectorMapHtml()
    {
        string htmlFileName = "VectorMap.html"; //로드할 HTML 파일 이름을 지정
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
                if (msg == "adjustMargins")
                {
                    AdjustWebViewMargins();
                }
                Debug.Log("CallFromJS[" + msg + "]"); // JavaScript에서 메시지를 받았을 때 로그에 출력하는 콜백 함수를 초기화
            });
        }
        webViewObject.LoadURL(filePath.Replace(" ", "%20")); // URL을 로드하는데, 공백은 "%20"으로 치환
        webViewObject.SetVisibility(true); //웹뷰의 가시성을 true로 설정

        //웹뷰의 위치는 SetMargins 메소드를 통해 제어됨. 캔버스의 구조나 위치에 영향을 받지 않고 전체 디스플레이 창에 대한 절대적 위치를 기준으로 설정됨.
        webViewObject.SetMargins(200, 1500, 200, 0); // 웹뷰의 마진을 설정합니다. (화면의 여백을 조정)
                                                     //왼쪽, 위, 오른쪽, 아래

    }

    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastTouchTime < doubleTapTime)
            {
                // 두 번째 탭이 감지되었을 때
                AdjustWebViewMargins();
                lastTouchTime = 0; // 타이머 리셋
            }
            else
            {
                // 첫 번째 탭이 감지되었을 때 타이머 시작
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
            // 싱글 탭이 감지되었을 때 실행
            OneTapWebViewMargins();
            tapCount = 0; // 탭 카운트 초기화
        }
        else if (tapCount == 2)
        {
            // 더블 탭이 감지되었을 때 실행
            AdjustWebViewMargins();
            tapCount = 0; // 탭 카운트 초기화
        }


    }

    void OneTapWebViewMargins()
    {
        // 싱글 탭 시 적용할 마진 설정
        webViewObject.SetMargins(200, 1500, 200, 0);
        Debug.Log("One tap margins set.");
    }

    void AdjustWebViewMargins()
    {
        // 예를 들어, 마진을 50, 100, 50, 100으로 변경
        webViewObject.SetMargins(50, 100, 50, 100);
    }

    /*
    IEnumerator DelayedSingleTap()
    {
        yield return new WaitForSeconds(doubleTapTime);
        if (Time.time - lastTouchTime >= doubleTapTime)
        {
            // 시간 차이가 더블 탭 시간보다 길면 싱글 탭으로 간주
            webViewObject.SetMargins(200, 1500, 200, 0);
        }
    }
    */
    

    public void SendAddress(string data)
    {
        Debug.Log(data);
    }

}
