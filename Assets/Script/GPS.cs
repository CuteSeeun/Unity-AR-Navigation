using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class GPS : MonoBehaviour
{

    //텍스트 ui변수
    public static GPS instance;
    //최대 대기시간
    public float maxWaitTime = 10.0f;
    //마지막 받은 시간
    public float resendTime = 1.0f;

    //위도 경도 변경
    public float latitude = 0;
    public float longitude = 0;
    float waitTime = 0;
    //GPS 받기
    public bool receiveGPS = false;

    //인스턴트 설정 (싱글톤)
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        StartCoroutine(GPS_On());
    }

    /// <summary>
    /// GPS처리 함수
    /// </summary>
    /// <returns></returns>
    public IEnumerator GPS_On()
    {
        //만일,GPS사용 허가를 받지 못했다면, 권한 허가 팝업을 띄움
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }
        }

        //만일 GPS 장치가 켜져 있지 않으면 위치 정보를 수신할 수 없다고 표시
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("GPS Off");
            yield break;
        }

        //위치 데이터를 요청 -> 수신 대기
        Input.location.Start();

        //GPS 수신 상태가 초기 상태에서 일정 시간 동안 대기함
        while (Input.location.status == LocationServiceStatus.Initializing && waitTime < maxWaitTime)
        {
            yield return new WaitForSeconds(1.0f);
            waitTime++;
        }

        //수신 실패 시 수신이 실패됐다는 것을 출력
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("위치 정보 수신 실패");
        }

        //응답 대기 시간을 넘어가도록 수신이 없었다면 시간 초과됐음을 출력
        if (waitTime >= maxWaitTime)
        {
            Debug.LogError("응답 대기 시간 초과");
        }

        //수신된 GPS 데이터를 화면에 출력
        LocationInfo li = Input.location.lastData;
     
        //위치 정보 수신 시작 체크
        receiveGPS = true;

        //위치 데이터 수신 시작 이후 resendTime 경과마다 위치 정보를 갱신하고 출력
        while (receiveGPS)
        {
            li = Input.location.lastData;
            latitude = li.latitude;
            longitude = li.longitude;

            Debug.Log("위도 : " + latitude.ToString() + "경도 : " + longitude.ToString());

            yield return new WaitForSeconds(resendTime);
        }
    }
}