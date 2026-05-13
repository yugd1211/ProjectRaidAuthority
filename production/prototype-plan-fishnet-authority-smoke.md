# FishNet Authority Smoke Prototype 계획

## 상태

**계획됨. 아직 구현되지 않음.**

이 문서는 프로토타입 계획이며 `/prototype 완료` 증거가 아니다. 실제 구현과 검증이 끝나기 전에는 `prototypes/fishnet-authority-smoke/README.md`를 만들지 않는다.

## 목표

Unity/FishNet dedicated server가 게임플레이 진실 상태를 소유할 수 있음을 가장 작은 범위에서 증명한다.

## 최소 시나리오

1. 헤드리스 또는 server mode 실행
2. 클라이언트 1–2개 접속
3. 서버 소유 더미 entity 생성
4. 클라이언트 input 전송
5. 서버가 위치 또는 상태를 확정
6. 클라이언트는 snapshot/event를 표시
7. 서버 로그에 검증 가능한 event 기록

## 선택 확장

- 서버 판정 damage
- RequestId 기반 loot transaction
- extraction hold와 commit event

## 완료 기준

- [ ] 서버와 클라이언트가 분리 실행된다.
- [ ] 클라이언트 직접 state write가 불가능하다.
- [ ] 서버 확정 snapshot/event가 클라이언트에 표시된다.
- [ ] 로그 또는 테스트 결과가 남는다.
- [ ] 결과를 `prototypes/fishnet-authority-smoke/README.md`에 기록한다.
