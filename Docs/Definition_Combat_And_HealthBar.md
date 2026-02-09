# 전투·체력바 정의

> 캐릭터와 몬스터가 **서로의 Model을 비교해 데미지를 주고받는** 구조. 체력바 UI로 현재/최대 HP 표시.

---

## 1. 인터페이스

| 인터페이스 | 용도 |
|-----------|------|
| **IDamageable** | 데미지 수신·체력 표시. `CurrentHp`, `MaxHp`, `TakeDamage(int)`, `OnHpChanged` |
| **IAttackPowerSource** | 공격력 제공. `AttackPower` |

**PlayerModel**, **MonsterModel**이 둘 다 구현. 전투 시 히트 감지기가 `IAttackPowerSource.AttackPower`로 데미지량을 정하고, 맞는 쪽 `IDamageable.TakeDamage()`로 적용(내부에서 방어력 감산).

---

## 2. 몬스터 쪽

- **MonsterData** (SO): `maxHp`, `attackPower`, `defense`. `CreateAssetMenu → Enemy/Monster Data`.
- **MonsterModel** (MonoBehaviour): Data 기반 런타임 HP, `TakeDamage`/`Heal`, `OnHpChanged`, `IsDead`.
- **Monster**: Model 보유. `Start`에서 `Model.Initialize()` 호출.

몬스터 프리팹 예시: Monster + MonsterModel + (선택) Collider. MonsterData 에셋 할당 후, 체력바용으로 **WorldHealthBarView**에 `_damageableHolder = MonsterModel` 연결.  
**플레이어가 통과하지 못하게 하려면** 몬스터 오브젝트에 **Nav Mesh Obstacle** 추가 (플레이어가 NavMeshAgent이므로, Npc처럼 Obstacle이 있어야 우회/막힘).

---

## 3. 역할 분리 (Weapon이 Detector 보유, Player는 Weapon만 구독)

| 컴포넌트 | 역할 |
|----------|------|
| **Weapon** | **히트박스(Collider) 소유** + **DamageableDetector 보유**. Collider를 `Initialize(hitbox)`로 Detector에 주입. `EnableHit()`/`DisableHit()`으로 켜/끔. 감지 결과는 **Weapon.OnDamageableDetected**로 전달. |
| **DamageableDetector** | Weapon이 컴포넌트로 보유. 주입받은 Collider로 Trigger만 해석, IDamageable 감지 시 이벤트 발행. (Weapon이 그 이벤트를 받아 자신의 OnDamageableDetected로 다시 알림) |
| **HitDamageApplier** | **Weapon**만 구독(`_weapon.OnDamageableDetected`). 플레이어/몬스터 쪽에 두고, `_weapon`에 자기 쪽 Weapon 연결. |

- **Player**: **Weapon**만 보유(PlayerAttacker._weapon). Damageable 찾은 건 **Weapon을 통해 이벤트**로 받음 → HitDamageApplier를 플레이어 GO에 두고 `_weapon`으로 같은 무기 연결하면 됨.

무기 오브젝트 구성: 같은 GO에 **Weapon** + **DamageableDetector** + **Collider**(Is Trigger). Weapon의 `_hitbox`에 Collider, `_detector`에 같은 GO의 DamageableDetector. Awake에서 Weapon이 `_detector.Initialize(_hitbox)` 호출. PlayerAttacker에는 `_weapon`만 할당. HitDamageApplier는 **플레이어(또는 몬스터) GO**에 두고 `_weapon` = 해당 무기, Owner/Source 연결.

---

## 4. 플레이어 → 몬스터 / 몬스터 → 플레이어

- **플레이어**: 무기 GO에 Weapon + DamageableDetector + Collider. Player GO에 PlayerAttacker(_weapon) + HitDamageApplier(_weapon = 같은 무기, Owner/Source = PlayerModel).
- **몬스터**: 몬스터 히트박스 GO에 Weapon + DamageableDetector + Collider. 몬스터 GO에 HitDamageApplier(_weapon = 그 무기, Owner/Source = MonsterModel). 애니에서 Weapon.EnableHit() / DisableHit() 호출.

---

## 5. 체력바 UI

| UI | 용도 | 연결 |
|----|------|------|
| **PlayerHealthBarView** | 스크린 공간 (예: 좌상단). | `_player` = Player, `_fillImage` = Image(Filled, Horizontal). |
| **WorldHealthBarView** | 머리 위 월드 바 (몬스터/NPC). | `_damageableHolder` = MonsterModel(또는 PlayerModel), `_fillImage` = Fill 이미지. `_hideWhenDead`로 사망 시 숨김. |

Fill Image는 **Image Type = Filled**, **Fill Method = Horizontal** 권장. 월드 바는 캔버스를 **World Space**로 두고, 몬스터 자식으로 위치(예: localPosition (0, 2, 0)) 후 빌보드는 스크립트가 LateUpdate에서 카메라 방향으로 처리.

---

## 6. 요약

| 하고 싶은 일 | 사용처 |
|-------------|--------|
| 플레이어가 몬스터에게 데미지 | 무기에 Weapon + DamageableDetector + (선택) HitDamageApplier, PlayerAttacker._weapon 연결, 애니 이벤트 EnableAttackHit / EndAttack |
| 몬스터가 플레이어에게 데미지 | 몬스터 히트박스에 Weapon + DamageableDetector + HitDamageApplier (Owner/Source=MonsterModel), 애니에서 Weapon.EnableHit/DisableHit |
| 플레이어 체력 표시 | PlayerHealthBarView (Player + Fill Image) |
| 몬스터 체력 표시 | WorldHealthBarView (MonsterModel + Fill Image, 월드 캔버스) |

데미지 식은 각 Model의 `TakeDamage(int amount)` 내부에서 `amount - Defense`로 처리됨.
