<!-- markdownlint-disable MD013 -->

# 서버 권한 핵심 아키텍처/검증 리뷰 — 2026-05-26

## 문서 상태

- **작성자**: OMX team worker-3
- **범위**: FishNet authoritative server-core의 수용 기준/검증 기준 점검
- **비범위**: 실제 DB, Backend service, 영구 저장소 구현
- **결론**: FishNet smoke prototype은 서버 권한의 최소 증거를 남겼지만, 첫 production server-core story로 넘어가기 전에는 아래의 추적성/검증 누락을 story 수용 기준에 명시해야 한다.

## 검토한 근거

| 근거 | 확인한 내용 |
| --- | --- |
| `docs/architecture/architecture.md` | 클라이언트 입력-only, 서버 truth, 검증 순서가 정의되어 있다. |
| `docs/architecture/control-manifest.md` | Client/Server Core/Loot/QA/FishNet Server Guard 필수 규칙과 금지 규칙이 정의되어 있다. |
| `docs/architecture/adr-0004-networkbehaviour-adapter-composition.md` | FishNet `NetworkBehaviour` 계약은 adapter가 소유하고 순수 규칙은 collaborator로 분리해야 한다. |
| `design/gdd/authoritative-server-core.md` | server tick, snapshot, `EntityId`, AOI 후보와 기본 수용 기준이 있다. |
| `production/prototype-plan-fishnet-authority-smoke.md` | FishNet authority smoke prototype이 완료 상태로 갱신되어 있다. |
| `prototypes/fishnet-authority-smoke/README.md` | 수동 검증 완료 증거와 남은 한계가 기록되어 있다. |
| `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow` 정적 확인 | 서버 이동, loot 요청, `SyncVar`, `[ServerRpc]`, `[Server]` 사용 흔적이 존재한다. |
| `Assets/00_ProjectRaidAuthority/04_Tests/00_EditMode/Editor` 정적 확인 | 이동 입력 sanitize/rate-limit 순수 규칙 테스트가 존재한다. |
| `design/gdd/inventory-looting-system.md` | RequestId 멱등 transaction 수용 기준이 아직 자동화/완료 체크로 닫히지 않았다. |
| `design/gdd/raid-flow-system.md` | 추출 성공/실패 commit 경계 수용 기준이 아직 smoke 범위 밖이다. |

## 첫 server-core story에 반드시 들어가야 할 수용 기준

아래 항목은 worker-2가 작성할 첫 server-core story의 acceptance checklist에 그대로 들어갈 수 있는 최소 문장이다.

1. **서버 권한 입력 경계**
   - 클라이언트는 이동/상호작용 요청 payload만 보낸다.
   - 서버는 요청값을 sanitize/clamp/reject한 뒤 위치, 회전, loot 상태를 확정한다.
   - 클라이언트 코드가 HP, 위치, loot ownership, extraction success를 직접 확정하지 않는다.
2. **FishNet 계약 가시성**
   - `[ServerRpc]`, `[Server]`, `SyncVar`, `NetworkBehaviour` lifecycle이 어느 adapter 파일에 있는지 리뷰어가 한 번에 찾을 수 있다.
   - production story에서 `partial`을 유지한다면 ADR-0004 예외 사유와 계약 소유 파일을 story에 기록한다.
3. **순수 collaborator 테스트성**
   - Unity/FishNet 타입 없이 검증 가능한 입력 sanitize, rate-limit, distance/ownership/idempotency 정책은 EditMode 테스트 대상으로 분리한다.
   - collaborator가 `UnityEngine`, FishNet namespace, `Transform`, `NetworkObject`, `Time`에 직접 의존하지 않는다.
4. **멱등 transaction 증거**
   - 같은 `RequestId`의 loot 요청은 두 번째부터 상태 변경 없이 무시된다.
   - 이미 획득된 item에 대한 새 요청은 거절 로그를 남기고 소유권을 바꾸지 않는다.
5. **재현 가능한 검증 증거**
   - story 완료 보고에는 Unity EditMode 테스트 결과, 정적 grep/리뷰 결과, 수동 FishNet smoke 체크리스트 결과 또는 미실행 사유를 함께 남긴다.
   - smoke 로그에는 최소한 `서버 이동 적용`, `LootCommitted`, `LootRejected` 또는 동등한 서버 확정 이벤트가 남는다.

## 검증 기준 매트릭스

| 검증 대상 | 권장 검증 방법 | PASS 기준 | 현재 상태 |
| --- | --- | --- | --- |
| 서버 이동 권한 | Unity EditMode + 수동 FishNet smoke | 입력 sanitize/rate-limit 테스트 PASS, 클라이언트 직접 위치 확정 없음 | 부분 충족: 순수 규칙 테스트 존재, smoke 수동 증거 존재 |
| FishNet Server Guard | 정적 grep + 코드리뷰 | gameplay truth 변경 메서드에 `[Server]` 또는 `[ServerRpc]` 진입 후 서버 검증이 있다 | 부분 충족: loot state 변경에는 `[Server]` 확인, 이동 apply는 story에서 추가 리뷰 필요 |
| Adapter/composition 경계 | 정적 파일 구조 리뷰 | FishNet 계약 소유 파일과 순수 collaborator 경계가 명확하다 | 주의: `GamePlayer`가 partial/domain 폴더로 나뉘어 있어 ADR-0004와 tension 있음 |
| Loot idempotency | Unity PlayMode 또는 수동 2회 요청 | duplicate `RequestId`는 무시, already-looted item은 거절 | 부분 충족: smoke README에 로그 패턴 존재, 자동화는 미완료 |
| Tick/snapshot/AOI 초기값 | ADR/story checklist | 후보값과 측정 로그 위치가 story에 연결된다 | 누락: prototype README가 별도 ADR 필요성을 명시 |
| 신규 참가자 snapshot 복원 | story acceptance | 이미 looted item이 신규 client에도 unavailable로 복원된다 | 누락: prototype README의 남은 한계 |

## 누락/리스크 등록부

### GAP-01 — Tick/Snapshot/AOI 초기값 ADR 부재

- **근거**: `design/gdd/authoritative-server-core.md`는 tick 20–30Hz, snapshot 10–20Hz, AOI 25–45m 후보만 둔다. `prototypes/fishnet-authority-smoke/README.md`도 초기값 ADR 필요성을 남은 단계로 적고 있다.
- **영향**: 첫 production story가 측정 기준 없이 “작동함”만 확인할 위험이 있다.
- **권장 조치**: 첫 story acceptance에 “선택한 tick/snapshot/AOI 후보와 로그 위치를 기록한다”를 추가한다. 별도 ADR은 story 완료 전 또는 직후 작성한다.

### GAP-02 — ADR-0004와 현재 prototype partial 구조의 긴장

- **근거**: ADR-0004는 FishNet 계약을 단일 adapter/orchestrator 파일이 소유한다고 결정했다. 현재 prototype은 `GamePlayer.cs` 외에도 `30_Loot/Client/GamePlayer.LootClient.cs`, `30_Loot/Server/GamePlayer.LootServer.cs` 같은 partial 파일에 FishNet 요청 흐름을 분산한다.
- **영향**: production story에서 계약 소유자를 놓치면 `[ServerRpc]`, `SyncVar`, 서버 검증 위치 리뷰가 어려워진다.
- **권장 조치**: 실제 구현 변경은 이번 작업 범위 밖이다. 첫 story에는 “partial 유지 시 예외 사유와 계약 소유 표를 story에 남긴다”를 수용 기준으로 둔다.

### GAP-03 — Loot idempotency 자동 검증 부족

- **근거**: smoke README에는 duplicate/already-looted 로그 패턴이 있지만, 현재 확인된 자동 테스트는 movement sanitize/rate-limit 중심이다.
- **영향**: 수동 검증 회귀를 놓칠 수 있다.
- **권장 조치**: DB/Backend 구현 없이도 EditMode 순수 정책 테스트 또는 PlayMode smoke checklist로 duplicate request, already-looted request를 고정한다.

### GAP-04 — 신규 참가자/재접속 snapshot 복원 정책 미정

- **근거**: prototype README가 “raid 중 신규 gameplay 참가자의 looted snapshot 복원은 아직 별도 정책이 없다”고 명시한다.
- **영향**: late join 또는 reconnect에서 클라이언트 표시가 서버 truth와 어긋날 수 있다.
- **권장 조치**: 첫 server-core story의 비범위로 둘 수 있지만, story acceptance에는 “late join/reconnect는 비범위이며 후속 story로 추적한다”를 명시한다.

### GAP-05 — 완료 증거 포맷이 문서별로 흩어짐

- **근거**: GDD 수용 기준, architecture 검증 순서, prototype README 로그 패턴이 서로 연결되어 있지만 하나의 story 완료 템플릿은 없다.
- **영향**: worker별 산출물이 합쳐질 때 leader audit에서 PASS/FAIL 판단이 흐려질 수 있다.
- **권장 조치**: 첫 story 완료 보고는 아래 “완료 증거 템플릿”을 따른다.

## 완료 증거 템플릿

첫 server-core story 완료 시 아래 블록을 story 문서나 완료 보고에 붙인다.

```md
## Verification Evidence

- PASS/FAIL — Unity EditMode tests: `<command or Unity Test Runner path>` → `<결과 요약>`
- PASS/FAIL — Static server authority scan: `grep -RIn "[ServerRpc]\|[Server]\|SyncVar" Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow` → `<검토 결과>`
- PASS/FAIL — Client truth ownership scan: `<명령 또는 리뷰 범위>` → `<클라이언트 직접 확정 없음/발견 내용>`
- PASS/FAIL — FishNet manual smoke: `<scene/role/client count>` → `<서버 이동/LootCommitted/LootRejected 로그>`
- PASS/FAIL — Documentation traceability: `<GDD/ADR/manifest/story 링크>` → `<연결 확인>`
- NOT TESTED — `<미실행 항목>` → `<사유와 후속 작업>`
```

## Leader handoff 요약

- 이번 작업은 문서/계획만 작성했으며 DB/Backend 구현은 하지 않았다.
- 첫 server-core story는 `docs/architecture/control-manifest.md`, `docs/architecture/adr-0004-networkbehaviour-adapter-composition.md`, `design/gdd/authoritative-server-core.md`, `prototypes/fishnet-authority-smoke/README.md`를 acceptance source로 참조해야 한다.
- production story 착수 전 가장 큰 누락은 Tick/Snapshot/AOI 초기값 ADR, partial 구조 예외 처리, loot idempotency 자동화, 추출 commit 경계, late join snapshot 정책이다.
- `architecture-traceability.md`의 `다음 증거`는 이제 `prototypes/fishnet-authority-smoke/README.md`와 이 리뷰 문서로 연결될 수 있다. 다만 traceability 원본 갱신은 worker-1/worker-2 산출물 충돌 가능성을 피하기 위해 leader 통합 단계로 넘긴다.
