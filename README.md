## 프로젝트 소개
<img src="https://github.com/CuteSeeun/Unity-AR-Navigation/blob/main/2_AR.pptx.png" alt="커버 이미지" width="950" />  

<details>
  <summary>
    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
  <rect x="3" y="7" width="13" height="10" rx="2" ry="2"></rect>
  <polygon points="16 7 22 11 22 13 16 17 16 7"></polygon>
</svg>
    AR Navigation.mp4  :  이미지 클릭 -> 영상 시청 
  </summary>
  
   [![유튜브 미리보기 이미지](https://img.youtube.com/vi/9ptV7AUB8UE/0.jpg)](https://www.youtube.com/watch?v=9ptV7AUB8UE&t=489s)
</details>  

Unity로 개발한 안드로이드용 AR 내비게이션입니다.  
이 어플은 3D PET과 함께 실시간 경로 안내와 AR 내비게이션을 제공하는 서비스로, 사용자에게 직관적인 길찾기와 재미있는 상호작용 경험을 제공합니다.  
지도에서 위치 검색, AR 화살표 경로 안내, 이미지 인식을 통한 정보 팝업 기능을 지원합니다. 또한, 다양한 PET 캐릭터와 상호작용하며 즐거운 내비게이션을 경험할 수 있습니다.  

## 개발 기간
- 2024.04.08 ~ 2024.05.16


## 팀원 소개
- **[김재흠](https://github.com/nasri016)** : UI/UX, AR 몬스터 제작, 3D Pet 모델링 및 애니메이션 제작, 인트로 영상 제작
- **[김수민](https://github.com/eneru7i)** : AR 이미지 트래킹, Pet 애니메이션 선택 화면
- **[최세은](https://github.com/CuteSeeun)** : 티맵 연동 및 검색과 도보 경로 기능 
- **[한차영](https://github.com/chayeong)** : 검색어 자동완성, 최근 검색어 목록, AR 오브젝트 및 Pet 배치


## 개발 환경
- **Front-end** : HTML, CSS, JavaScript, WebView Plugin
- **Back-end** : Tmap API (REST API)
- **AR 개발** : Unity (AR Foundation, ARCore), C#
- **3D 콘텐츠** : Cinema 4D(모델링), Unity 3D
- **통신** : UnityWebRequest, JavaScript <-> Unity 통신
- **빌드** : Unity Android Build (SDK/NDK/JDK)

## 어플 주요 기능
- 메인 메뉴  
  - 사이드 메뉴에서 메인 메뉴, 지도, AR로 이동할 수 있고 3D PET 목록을 확인할 수 있습니다. 또한 사운드를 조절할 수 있습니다.
  - 3D PET 클릭 시 캐릭터 소개 영상을 시청할 수 있습니다.
  - 길찾기 버튼을 눌러 지도 화면으로 이동할 수 있습니다.

<br>

- 지도 화면
  - 실시간으로 현재 위치 마킹 (Geolocation API)
  - 지도 타입 설정(ROAD, SATELLITE, HYBRID)
  - 검색 자동완성 기능
  - 검색어 검색 시 관련 장소 최대 10개 마킹 / 전화번호, 명칭, 주소로 검색 가능 / 검색 시 모든 마커가 화면에 들어오도록 지도의 보기 범위를 조정, 이동, 확대
  - 마커 선택 시 명칭, 주소, 전화번호 인포윈도우 생성
  - 출발, 도착지 설정 시 도보 경로 라인과 경로 상세 페이지 팝업창 생성

<br>

- AR Pet 선택창
  - 스와이프로 원하는 펫 설정
  - 터치 시 Pet 상호작용 애니메이션 작동

<br>

- AR 내비게이션 화면
  - 권한 설정 팝업창 (사진, 앨범, 동영)
  - 미니맵 / 클릭 시 지도 확대
  - 도보 경로에 따른 AR 화살표, 3D PET 배치
  - 도착지 주변 팝업창, AR 마커 활성화
  - 특정 장소(이미지) 인식 시 해당 정보 팝업창 활성화 
