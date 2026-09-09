# Project_HorrorGame_First

## ⚠️ 유료 에셋 안내 (Paid Assets Notice)

본 프로젝트는 아래의 유료 에셋을 사용합니다.  
저작권 보호 및 용량 관리 목적으로 **에셋 파일은 `.gitignore` 처리되어 저장소에 포함되어 있지 않습니다.**

프로젝트를 정상적으로 실행하려면 해당 에셋을 직접 구매하여 프로젝트에 `Import` 해야 합니다.

1. **[UHFPS(URP)]** - ThunderWire Studio
2. **[Odin Inspector]** - Sirenix
3. **[DOTween Pro]** - Demigiant

---

# 1. 프로젝트 개요

### Unity 기반 Horror Game 개발 및 서드파티 프레임워크 확장

> 
> 
> 
> UHFPS · UniTask · DOTween · Odin Inspector
> 

| 항목 | 내용 |
| --- | --- |
| **개발 기간** | 5일 (26.08.28~26.09.04) *주말 제외 |
| **개발 인원** | 1인 개인 프로젝트 |
| **Engine** | Unity 6 |
| **Language** | C# |
| **Version Control** | Git |
| **링크** | GitHub Repository |

### 프로젝트 목표

- 서드파티 프레임워크 기반 개발 시, 기존 시스템의 구조와 이벤트 흐름을 분석하고 필요한 기능을 독립적인 C# 컴포넌트로 확장하는 검증 진행.
- 프레임워크 내부 코드를 무분별하게 수정하기보다, 기존 구조를 유지하면서 확장 가능한 영역을 우선 분리하여 실제 게임플레이에 적용 및 동작 검증.

---

# 2. 핵심 구현 요약

| 영역 | 구현 내용 |
| --- | --- |
| Framework Analysis | 기존 서드파티 프레임워크 구조 및 이벤트 흐름 분석 |
| Troubleshooting | Objective Lifecycle / Input State 충돌 해결 / Dynamic Lock 구현 |
| Event System | Runtime Event Binding (Listener 등록·제거·One-shot 처리) |
| Event Lifecycle | Listener 등록 / 제거 / One-shot 처리 |
| Async | UniTask 기반 Sequence 및 Delay 제어 |
| UI | ScriptableObject 기반 Subtitle System |
| Animation | DOTween 기반 연출 |
| Player Control | Input Freeze / Teleport System |
| NPC | 시야각 기반 LookAt 제어 |
| Architecture | Trigger / Manager / Binder / Asset 역할 분리 |
| Inspector | Odin Inspector 기반 설정 및 디버깅 |

---

# 3. 설계 원칙

- 기존 프레임워크 구조 최대한 유지
- 프레임워크 기능과 신규 기능의 결합도 최소화
- 반복적으로 사용할 기능은 독립적인 컴포넌트로 분리
- 이벤트 Listener의 등록 및 해제 생명주기 명확화
- 상태 기반으로 중복 실행 방지

---

# 4. Framework Troubleshooting

## 4-1. Objective 중복 완료 문제

### Problem

동일한 Objective 완료 조건이 다시 발생할 경우 **이미 완료된 Objective의 완료 처리가 재실행되는 문제 발생.**

```
Objective 생성
      ↓
조건 충족
      ↓
Objective 완료
      ↓
동일 조건 재발생
      ↓
완료 처리 및 완료 이벤트 재실행
```

### Root Cause

Objective 완료 처리 과정에서 **이미 완료된 Objective 및 SubObjective에 대한 상태 검증 로직 부족.**

### Solution

1. 완료 상태를 먼저 검증하도록 Objective 처리 로직 수정.

```csharp
if (data.IsCompleted.Value)
    return;
```

1. SubObjective 역시 완료 상태를 확인하여 중복 처리를 방지.

```csharp
if (subData.IsCompleted.Value)
    continue;
```

1. 전체 SubObjective가 완료되었을 때만 Objective 완료 이벤트를 발생시키도록 처리.

```markdown
CompleteObjective()
      ↓
Objective 완료 여부 확인
      ↓
이미 완료 → 종료
      ↓
SubObjective 상태 확인
      ↓
전체 완료
      ↓
OnObjectiveComplete
      ↓
Objective 정리
```

### Result

- Objective 완료 이벤트 중복 호출 방지
- Objective 상태 생명주기 명확화

### Key Point

> 단순 이벤트 제거가 아닌 **완료 상태를 기준으로 처리 유효성을 검증하도록 Lifecycle 개선.**
> 

---

## 4-2. Input 상태 충돌 분석 및 비동기 실행 순서 제어

### Problem

Subtitle 연출 중 플레이어의 이동 및 시점 조작을 제한하기 위해 Input Freeze를 적용했으나, 존 프레임워크의 Examine, Inventory 시스템 등도 Player Input 상태를 독립적으로 제어하여 상태 변경이 충돌하는 문제 발생.

### 발생 구조

```
의도한 것

Examine Event 종료
      ↓
UHFPS Input 해제
      ↓
Subtitle Event 시작
      ↓
Subtitle Input Freeze
```

```markdown
실제 결과

Examine Event 종료
      ↓
Subtitle Event 시작
      ↓
Subtitle Input Freeze
      ↓
UHFPS Input 해제 
```

### Root Cause

Subtitle 시스템의 Freeze 로직 자체 결함이 아닌, 여러 시스템이 동일한 Player Input 상태를 독립적으로 제어하면서 이벤트 실행 순서에 따라 상태가 덮어써지는 구조적 충돌.

### Solution

- 기존 프레임워크의 Input 제어 로직을 수정하는 대신, **Subtitle 실행 시점을 분리하여 이벤트 충돌 방지.**
- `UniTask.Delay`를 이용하여 기존 이벤트 처리와 Subtitle Input 제어 사이에 시간적 간격 확보.

```csharp
await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeSubtitles),
								    cancellationToken: token);
```

### Result

- UHFPS와 Subtitle 시스템 간 Input 상태 충돌 해결 및 연속 이벤트의 상태 덮어쓰기 방지
- 비동기 연출의 실행 순서 제어

### Key Point

> 단순한 Delay 추가가 아닌 **공유 상태를 제어하는 시스템 간 실행 순서 충돌을 분석하고, 기존 프레임워크의 동작을 유지한 상태에서 실행 시점을 분리하여 해결.**
> 

**[추후 개선사항]**

임시 시점 분리 방식에서 향후 Priority 기반 상태 머신(State Machine) 구조로 개선 예정.

---

## 4-3. LockDoorAfterEvent

### Problem

UHFPS 기본 Door 기능만으로는 "특정 Gameplay Event 발생 후 문을 닫고 → 애니메이션 종료 후 문을 잠그고 → 후속 Gameplay Event 실행" 과 같은 연속 기믹 구현이 어려웠음.

### Root Cause

UHFPS는 기본적으로 인스펙터 기반 이벤트 연결만 지원하여, 런타임에서 원하는 시점에 Listener를 동적으로 등록/해제하기 어려운 구조였음.

### Solution

`DynamicObject`의 기존 Interaction 및 Animation Event를 활용하여 `LockDoorAfterEvent` 독립 컴포넌트 제작.

### 핵심 흐름

```
Gameplay Event
      ↓
LockDoor()
      ↓
Door Animation 종료 시 Event 등록
      ↓
문 상태 확인           
 ┌────┴──────────┐
 │               │
닫힘             열림
 │               │
 │             Animation 종료
 │               │
 └────┬──────────┘            
      ↓
SetLockedStatus(true)
      ↓
OnFinished → 다음 Gameplay Event 연결  
```

### 코드 스니펫

```csharp
public void LockDoor()
{
		if (doorToLock == null || Started)
				return;

    Started = true;

    if (doorToLock.IsOpened)
    {
        // 애니메이션 완료 후 실행될 리스너 동적 등록
				doorToLock.useEvent2.AddListener(WaitForAnimation);
				doorToLock.InteractStartPlayer(gameObject);
    }
    else
    {
				FinishMethod();
    }
}
private void WaitForAnimation()
{
    // 리스너 중복 방지를 위한 해제 처리 후 완료 메서드 호출
    doorToLock.useEvent2.RemoveListener(WaitForAnimation);
    FinishMethod();
}
```

### Result

- Door 상태(열림/닫힘)에 따른 후속 처리 분기 및 Animation 종료 전 잠금 처리 방지
- Runtime Listener 중복 등록 방지 및 안정적인 후속 Logic 실행 (`UnityEvent OnFinished`)

### Key Point

> UHFPS의 기존 Door 구조를 직접 수정하지 않고, 기존 이벤트를 활용하는 독립 C# 컴포넌트로 확장.
> 

---

# 5. Runtime Event Binding System

기존 시스템의 이벤트를 게임 진행 중 특정 조건에 따라 동적으로 등록하고 제거할 수 있도록 `Runtime Event Binder` 구조 구현.

### 설계 구

```
게임플레이 조건 발생
      ↓
BindEvent()
      ↓
기존 시스템 Event에 Listener 등록
      ↓
Event 발생
      ↓
Handler 실행
      ↓
UnityEvent 호출
      ↓
필요 시 Listener 제거
```

### 공통 설계

- Runtime Listener 등록 및 제거
- `triggerOnce`를 통한 일회성 이벤트 지원
- `isBound`를 이용한 중복 등록 방지
- UnityEvent를 통한 Inspector 기반 후속 로직 연결
- 이벤트 종류별 Binder 분리

### 구현 대상

| Component | Runtime Binding |
| --- | --- |
| `DynamicObjectEventBinder` | Open / Close |
| `InteractableRuntimeEventBinder` | Take / Examine Start / Examine End |
| `InteractableLightRuntimeEventBinder` | Light On / Off |
| `ObjectiveCompleteRuntimeBinder` | Objective Complete |
| `EyeBlinkEventBinder` | Eyes Close / Open |

### 코드 스니펫 (`DynamicObjectEventBinder`)

```csharp
public class DynamicObjectEventBinder : MonoBehaviour
    {           
        public UnityEvent OnOpen;        

        public void BindOpen()
        {
            if (target == null || isBound)
                return;

            isBound = true;

            target.useEvent1.AddListener(HandleOpen);
        }
        
        ...
        
        private void HandleOpen()
        {
            OnOpen?.Invoke();

            if (triggerOnce)
            {
                target.useEvent1.RemoveListener(HandleOpen);
                isBound = false;
            }
        }        
    }
```

### Key Point

> 특정 이벤트에 직접 종속된 게임플레이 로직을 작성하는 대신, **Runtime Binding 계층을 별도 컴포넌트로 분리하여 재사용성 및 Inspector 기반 이벤트 연결 구조 확보.**
> 

---

# 6. Subtitle System

게임 내 대사와 연출을 위한 `ScriptableObject` + `Manager` + `Trigger` 기반 Subtitle System 구현.

### Architecture

```
SubtitleAsset
      ↓
SubtitleTrigger
      ↓
SubtitleManager
      ↓
TextMeshPro
      ↓
DOTween / UniTask
```

### SubtitleAsset

자막 데이터와 게임플레이 로직을 분리하기 위해 ScriptableObject 기반으로 구성.

### SubtitleTrigger

여러 자막을 순차적으로 재생하고 전체 연출 흐름을 관리.

- 자막 순차 재생
- 자막별 Delay (길이)
- 시작 전 Delay / 종료 후 Delay
- Player Movement Freeze
- Player Look Freeze
- 종료 Event
- CancellationToken 기반 비동기 작업 취소

### 비동기 흐름

```
TriggerSubtitles()
      ↓
Start Delay
      ↓
Player Input 제어
      ↓
Subtitle 재생
      ↓
Next Subtitle Delay
      ↓
다음 자막
      ↓
Subtitle 종료
      ↓
Player Input 복구
      ↓
OnSubtitlesFinished → 후속 UnityEvent
```

### Key Point

> 단순한 텍스트 출력 기능이 아닌 **여러 자막과 Player 상태 제어가 결합된 비동기 연출 Sequence를 하나의 시스템으로 분리.**
> 

---

# 7. Player Teleport System

플레이어의 위치와 바라보는 방향을 동시에 변경하는 Teleport 시스템 구현.

### Architecture

```
[TeleportTrigger (요청/대상 전달)]
      ↓
[TeleportManager (실제 처리)]
      ↓
[Player]
```

### 처리 흐름

```
Player Freeze
      ↓
DragRigidbody Disable
      ↓
Position 변경
      ↓
LookController Rotation
      ↓
DragRigidbody Enable
      ↓
Player Freeze 해제
```

### **Key Point**

텔레포트 중 `DragRigidbody`를 비활성화하여 물리적 상호작용에 의한 위치 간섭을 방지하고, Trigger와 실제 동작을 분리하여 재사용 가능한 구조로 설계.

---

# 8. NPC LookAt System

NPC가 플레이어의 위치와 시야각을 기준으로 자연스럽게 시선을 전환하도록 구현.

### 처리 방식

```
NPC / Player 위치 계산
      ↓
수평 방향 벡터 계산
      ↓
NPC Forward와 각도 계산
      ↓
Max Angle 검사
      ↓
Target Weight 결정
      ↓
MoveTowards로 Weight 보간
```

### 구현 기능

- 수평 방향 기반 시야각 계산
- `maxAngle`을 이용한 시야 범위 제한
- `LookAtConstraint.weight` 보간
- 자동 / 강제 Weight 제어

### 활용

플레이어 접근에 따라 NPC가 자연스럽게 시선을 전환하는 **기괴한 공포 연출**에 활용.

---
