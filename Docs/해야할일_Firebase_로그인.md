# Firebase 로그인·세이브 연동 (해야 할 일)

## 순서

| 순서 | 작업 | 설명 |
|------|------|------|
| 1 | Firebase SDK 설치 및 초기화 | FirebaseAuth, Firestore(또는 Realtime DB) Unity 패키지 추가. `FirebaseApp.CheckAndFixDependenciesAsync()` |
| 2 | Firebase 콘솔 설정 | Auth 이메일/비밀번호 활성화. Firestore/Realtime DB 생성. 규칙 설정 |
| 3 | 회원가입·로그인 | `CreateUserWithEmailAndPasswordAsync`, `SignInWithEmailAndPasswordAsync`. IntroSceneView에 로그인/회원가입 UI |
| 4 | uid 기반 세이브 저장 | Firestore `users/{uid}/savedata` 구조. SaveManager와 연동 |
| 5 | (추후) Google Sign-In | 계정 연동(`linkWithCredential`) 또는 별도 로그인 옵션 |

## 참고

- 이메일/비밀번호: Unity Editor에서도 테스트 가능
- 세이브: `Auth.CurrentUser.UserId`를 키로 사용
- Google 연동: 나중에 추가해도 구조 변경 없이 가능
