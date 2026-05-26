# FishNet Authority Smoke Prototype 완료 증거

상태: Complete  
완료일: 2026-05-26  
검증 기준: Unity/FishNet 서버 권한 이동 + RequestId 기반 loot transaction + loot 획득 후 클라이언트 표시 제거 동기화  
증거 출처: 사용자 수동 검증 완료 보고와 코드/정적 근거 확인

## 실행한 씬

```text
Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetNetworkFlow/OfflineBootstrap.unity
Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetNetworkFlow/MatchRoom.unity
Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetNetworkFlow/Gameplay.unity
```

## 실행 방법

1. `OfflineBootstrap`에서 FishNet 서버 또는 Host 역할을 시작한다.
2. 클라이언트 1개 이상을 접속한다.
3. `MatchRoom`에서 RoomPlayer Ready를 완료한다.
4. `Gameplay` 씬으로 전환되는지 확인한다.
5. GamePlayer 이동 입력을 전송하고 서버 적용 위치가 클라이언트에 동기화되는지 확인한다.
6. `LootItem` 근처에서 `E` 키로 loot 요청을 보낸다.
7. 다른 클라이언트에서도 획득된 loot item이 보이지 않거나 상호작용 후보에서 제외되는지 확인한다.

## 검증한 서버 권한 항목

- 클라이언트는 이동 입력만 전송하고, 서버가 위치/회전을 확정한다.
- `NetworkTransform`은 서버 권한 기준으로 동작한다.
- 클라이언트는 loot 요청에 `requestId`와 `itemId`만 보낸다.
- 서버는 loot 거리, 중복 requestId, 이미 획득된 상태를 검증한다.
- 성공한 loot는 서버에서 `LootItemState.Looted`로 확정된다.
- `LootItemState`는 FishNet `SyncVar`로 클라이언트에 동기화된다.
- 획득 후 클라이언트 presentation은 `Renderer`/`Collider` 비활성화로 사라져 보이며, 루트 `NetworkObject`는 유지된다.
- 같은 item에 대한 재획득 요청은 새 소유권 변경 없이 거절된다.

## 성공 로그 샘플 / 확인 패턴

```text
[NetworkFlow] 서버 전용 모드를 시작했습니다. 클라이언트는 함께 시작하지 않습니다.
[NetworkFlow] RoomPlayer를 스폰했습니다. clientId=<clientId>
[NetworkFlow] 모든 RoomPlayer가 준비되었습니다. Gameplay 씬을 로드합니다.
[FishNet Authority Smoke] LootItem 스폰: itemId=loot-smoke-item-001, position=<position>
[FishNet Authority Smoke] 서버 이동 적용: owner=<ownerId>, move=<direction>, position=<position>
[FishNet Authority Smoke] LootCommitted: owner=<ownerId>, requestId=<requestId>, itemId=loot-smoke-item-001, state=Looted
[FishNet Authority Smoke] LootRejected: owner=<ownerId>, requestId=<requestId>, itemId=loot-smoke-item-001, reason=Loot item already looted
[FishNet Authority Smoke] Duplicate LootRequest ignored: owner=<ownerId>, requestId=<requestId>, itemId=loot-smoke-item-001
```

## 남은 한계

- 실제 인벤토리 스택/수량 UI는 아직 없다.
- 추출 성공 후 `ExtractCommitted → StashOwned` 영구 확정은 아직 없다.
- raid 중 신규 gameplay 참가자의 looted snapshot 복원은 아직 별도 정책이 없다.
- 대량 loot에 대한 despawn/pooling 최적화는 아직 하지 않았다.
- Tick/Snapshot/AOI 초기값은 별도 ADR로 고정해야 한다.

## 다음 단계로 가져갈 결정

1. `NetworkObject.Despawn()`은 지금 쓰지 않고, 서버 검증 객체를 남긴 채 `SyncVar` 상태와 presentation 비활성화로 처리한다.
2. 대량 loot 최적화가 필요해지면 서버 loot registry와 despawn/pooling 정책을 함께 설계한다.
3. 다음 문서 작업은 Tick/Snapshot/AOI 초기값 ADR과 첫 server-core story/sprint 후보 작성이다.
