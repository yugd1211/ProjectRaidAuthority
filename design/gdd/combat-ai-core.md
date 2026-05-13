# 전투와 AI 핵심

> **상태**: 아키텍처 계획 기준 승인  
> **작성 기준일**: 2026-05-13  
> **구현 축**: 짧고 명확한 서버 판정 전투  
> **주의**: 실제 수치는 Unity/FishNet smoke prototype 측정 전까지 후보값이다.

## 개요

서버가 발사, 명중, 피해, 상태이상, 적 AI 의사결정을 판정하는 전투 기반 시스템이다.

## 플레이어 판타지

플레이어는 빠르게 읽히는 전투 속에서 맞힐 때 확실하고, 맞을 때 납득 가능하다고 느껴야 한다.

## 상세 규칙

1. 클라이언트는 조준/발사 입력만 보내고 명중 결과를 확정하지 않는다.
2. 서버는 weapon spec, entity state, server tick 기준으로 hit/damage를 계산한다.
3. AI는 서버에서 목표 선택, 이동 의도, 공격 타이밍을 결정한다.
4. 피해와 상태 변경은 `DamageApplied` 같은 서버 이벤트로 기록한다.
5. VFX와 UI는 확정 이벤트를 표시만 한다.

## 수식

```text
final_damage = base_damage * trait_multiplier * vulnerability_multiplier - armor_reduction
hit_allowed = server_distance <= weapon_range and cooldown_ready
```

수식은 구현 전 검증용 기준이다. 최종 타입과 직렬화 방식은 Unity/FishNet API 확인 후 결정한다.

## 예외 상황

- 동일 발사 sequence가 중복 도착하면 한 번만 처리한다.
- 클라이언트 예측 VFX가 서버에서 거절되면 서버 이벤트가 표시를 덮어쓴다.
- AI가 장애물에 막히면 재경로 탐색 또는 공격 취소 로그를 남긴다.

## 의존성

- 서버 권한 핵심
- Roguelike run build
- Raid 흐름
- HUD

## 조정값

- base_damage
- cooldown_seconds
- enemy_notice_radius
- elite_affix_count
- status_duration

## 수용 기준

- [ ] 클라이언트 hit report 없이 서버가 피해를 계산한다.
- [ ] 중복 발사/피해 요청이 중복 피해를 만들지 않는다.
- [ ] AI가 서버에서만 공격 결정을 내린다.
- [ ] 피해 이벤트가 raid log에 남는다.
