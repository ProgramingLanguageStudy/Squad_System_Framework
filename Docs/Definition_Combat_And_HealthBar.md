# 전투·체력바 정의

> 캐릭터와 Enemy가 **서로의 Model을 비교해 데미지를 주고받는** 구조. 체력바 UI로 현재/최대 HP 표시.

---

## 1. 인터페이스

| 인터페이스 | 용도 |
|-----------|------|
| **IDamageable** | 데미지 수신·체력 표시. `CurrentHp`, `MaxHp`, `TakeDamage(int)`, `OnHpChanged` |
| **IAttackPowerSource** | 공격력 제공. `AttackPower` |

**PlayerModel**, **EnemyModel**이 둘 다 구현. 전투 시 히트 감지기가 `IAttackPowerSource.AttackPower`로 데미지량을 정하고, 맞는 쪽 `IDamageable.TakeDamage()`로 적용(내부에서 방어력 감산).

---

## 2. Enemy 쪽

- **EnemyData** (SO): `maxHp`, `attackPower`, `defense`. `CreateAssetMenu → Enemy/Enemy Data`.
- **EnemyModel** (MonoBehaviour): Data 기반 런타임 HP, `TakeDamage`/`Heal`, `OnHpChanged`, `IsDead`.
- **Enemy**: Model 보유. `Start`에서 `Model.Initialize()` 호출.

Enemy 프리팹 예시: Enemy + EnemyModel + (선택) Collider. EnemyData 에셋 할당 후, 체력바용으로 **WorldHealthBarView**를 자식 등에서 참조, Start 시 `Initialize(Model)` 주입.  
**플레이어가 통과하지 못하게 하려면** Enemy에 **일반 Collider**(Capsule/Box, Is Trigger 해제)를 두면 됨. 플레이어는 CharacterController 사용으로 Collider와 충돌함.

---

## 3. 역할 분리 (HitboxController가 Detector 보유, Attacker가 감지 시 데미지 적용)

| 컴포넌트 | 역할 |
|----------|------|
| **HitboxController** | **히트박스(Collider) 소유** + **DamageableDetector 보유**. 같은 GO에 Collider·Detector 두면 Trigger 자동 수신. `EnableHit()`/`DisableHit()`으로 켜/끔. 감지 결과는 **HitboxController.OnDamageableDetected**로 전달. |
| **DamageableDetector** | HitboxController가 같은 GO에 두고 보유. Trigger만 해석, IDamageable 감지 시 이벤트 발행. (HitboxController가 그 이벤트를 받아 자신의 OnDamageableDetected로 다시 알림) |
| **PlayerAttacker** (플레이어) | **HitboxController** 보유 + `OnDamageableDetected` 구독해 데미지 적용. `_ownerModel`은 Player.Initialize()에서 주입. |

- **Player**: PlayerAttacker에 `_hitboxController`만 인스펙터 연결. `_ownerModel`은 Initialize(_stateMachine, _model)로 주입. Damageable 감지 시 Attacker가 직접 `TakeDamage` 호출.

무기/히트박스 오브젝트 구성: 같은 GO에 **HitboxController** + **DamageableDetector** + **Collider**(Is Trigger). HitboxController의 `_hitbox`에 Collider, `_detector`에 같은 GO의 DamageableDetector. **PlayerAttacker**에는 인스펙터에서 `_hitboxController`만 할당. Model은 Player 초기화 시 주입.

---

## 4. 플레이어 → Enemy / Enemy → 플레이어

- **플레이어**: 무기 GO에 HitboxController + DamageableDetector + Collider. Player GO에 PlayerAttacker(_hitboxController만 인스펙터, Model은 Initialize 시 주입).
- **Enemy**: Enemy 히트박스 GO에 HitboxController + DamageableDetector + Collider. Enemy GO에 Enemy용 Attacker. 공격 시작/끝: 상태(Enter·타이머)에서 호출. 히트 구간만 애니 이벤트: Animation_BeginHitWindow / Animation_EndHitWindow.

---

## 5. 체력바 UI

| UI | 용도 | 연결 |
|----|------|------|
| **PlayerHealthBarView** | 스크린 공간 (예: 좌상단). | Player가 참조 보유. `Initialize(PlayerModel)`로 Model 주입. `_fillImage`만 인스펙터. |
| **WorldHealthBarView** | 머리 위 월드 바 (Enemy/NPC). | Enemy가 참조 보유(자식 등). `Initialize(IDamageable)`로 Model 주입. `_fillImage`, `_hideWhenDead`만 인스펙터. |

Fill Image는 **Image Type = Filled**, **Fill Method = Horizontal** 권장. 월드 바는 캔버스를 **World Space**로 두고, Enemy 자식으로 위치(예: localPosition (0, 2, 0)) 후 빌보드는 스크립트가 LateUpdate에서 카메라 방향으로 처리.

---

## 6. 요약

| 하고 싶은 일 | 사용처 |
|-------------|--------|
| 플레이어가 Enemy에게 데미지 | 무기에 HitboxController + DamageableDetector, PlayerAttacker. 시작/끝: 애니 이벤트 OnAttackStarted/OnAttackEnded. 히트 구간: Animation_BeginHitWindow/Animation_EndHitWindow |
| Enemy가 플레이어에게 데미지 | Enemy 히트박스에 HitboxController + DamageableDetector, Enemy Attacker. 시작/끝: 상태 Enter·타이머. 히트 구간: 애니 이벤트 Animation_BeginHitWindow/Animation_EndHitWindow |
| 플레이어 체력 표시 | Player가 PlayerHealthBarView 참조, Initialize 시 Model 주입. View는 Fill Image만 인스펙터. |
| Enemy 체력 표시 | Enemy가 WorldHealthBarView 참조(자식 등), Start 시 Initialize(Model) 주입. 월드 캔버스. |

데미지 식은 각 Model의 `TakeDamage(int amount)` 내부에서 `amount - Defense`로 처리됨.
