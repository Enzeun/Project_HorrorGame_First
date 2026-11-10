# After the Ring

UHFPS 프레임워크 위에서 만든 1인 공포 게임입니다. 프레임워크 원본은 한 곳만 고치고, 필요한 기믹은 기존 이벤트를 구독하는 독립 C# 컴포넌트로 확장했습니다.

**[상세 문서 (설계 판단 · 트러블슈팅 · 회고)](https://enzeun.github.io/projects/after-the-ring.html)** · [itch.io에서 플레이](https://enzeun.itch.io/after-the-ring) · [트레일러](https://youtu.be/2PpvqcETudA) · [포트폴리오](https://enzeun.github.io)

| 항목 | 내용 |
| --- | --- |
| 기간 | 약 3주 (2026.08.19 ~ 09.10) |
| 인원 | 1인 (기획 · 맵 · 연출 · 프로그래밍) |
| 엔진 | Unity 6000.3.6f1 · URP |
| 기술 | C# · UHFPS · UniTask · DOTween · Odin Inspector |
| 규모 | 직접 작성 스크립트 17개, 약 1,500줄 · 7개 언어 현지화 |

## 직접 작성한 코드

서드파티 에셋은 저장소에 포함하지 않았으므로, 이 저장소에 있는 C# 코드는 모두 직접 작성한 것입니다. (네임스페이스 `ENZEUN.Runtime`)

| 경로 | 내용 |
| --- | --- |
| `Assets/Script/GameFlow/*EventBinder.cs` | 런타임 이벤트 바인더 5종: UHFPS 컴포넌트의 기존 이벤트에 런타임에 리스너를 붙였다 떼는 계층 (중복 방지 · 일회성 실행 · 파괴 시 해제) |
| `Assets/Script/Subtitle/` | 자막 시스템: SO 데이터 → 트리거 → 매니저. UniTask로 순차 재생하고, 입력 잠금은 try/finally로 반드시 해제 |
| `Assets/Script/GameFlow/EyeBlinkController.cs` | 포스트 프로세스 기반 암전 연출. 평소에는 꺼져 있다가 연출 중에만 `Update` 실행 |
| `Assets/Script/GameFlow/Teleport*.cs` | 요청과 처리를 나눈 텔레포트. 이동 중 물리 간섭 방지 |
| `Assets/Script/GameFlow/LockDoorAfterEvent.cs` | 이벤트 후 문을 닫고 애니메이션이 끝나면 잠그는 기믹 |
| `Assets/Script/Entity/LookAt.cs` | 시야각 기반 NPC 시선 제어 |
| `Assets/Editor/BuildSizeAnalyzer.cs` | 빌드 크기 분석 에디터 도구 |

### 프레임워크 원본을 수정한 곳 (1곳)

Objective 완료가 중복으로 발생하는 문제는 `ObjectiveManager` 내부 처리 순서 안에서 상태를 검증해야 해서, 이 프로젝트에서 유일하게 UHFPS 원본을 직접 수정했습니다. 수정 위치 4곳에 `// [ENZEUN 추가]` 주석을 남겼습니다. UHFPS는 유료 에셋이라 이 파일은 저장소에 포함되어 있지 않습니다. 자세한 내용은 [상세 문서의 트러블슈팅](https://enzeun.github.io/projects/after-the-ring.html)에 있습니다.

## 실행 방법

이 프로젝트는 아래 유료 에셋을 사용합니다. 저작권과 용량 문제로 **`.gitignore`에서 제외되어 저장소에 포함되어 있지 않으므로**, 실행하려면 직접 구매해 Import 해야 합니다.

1. **UHFPS (URP)** - ThunderWire Studio
2. **Odin Inspector** - Sirenix
3. **DOTween Pro** - Demigiant

Unity `6000.3.6f1`로 열고, Build Settings의 `MainMenu` 씬부터 실행합니다.
