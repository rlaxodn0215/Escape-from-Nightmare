# Step 9: danger-system

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/ARCHITECTURE.md`
- `/design/04_MONSTER_HIDING_AI.txt`
- `/design/08_REMAINING_TASKS.txt`
- `/src/ai/monster.lua`
- `/data/events.lua`

## 작업

위험도 계산과 몬스터 압박 신호를 구현한다.

1. `src/ai/danger.lua`를 만든다.
2. `danger_level`, `noise_level`, `stay_timer`, `capture_gauge`, `hide_noise` 값을 관리하는 인터페이스를 둔다.
3. 퍼즐 오답, 오래 머무름, 큰 소리 이벤트, 최종 추격이 danger system에 영향을 주도록 hook을 연결한다.
4. 접근 경고는 발소리, 심장박동, 조명 깜빡임, 숨소리/낮은 울음소리 hook으로 분리한다.
5. 정확한 수치는 placeholder balance로 두되, `design/08_REMAINING_TASKS.txt`에 남은 밸런싱 항목임을 코드/데이터에서 추적 가능하게 한다.

## Acceptance Criteria

```bash
luac -p main.lua
```

환경에서 가능한 경우:

```bash
love .
```

## 검증 절차

1. danger 값들이 증가/감소하는 최소 경로가 있는지 확인한다.
2. capture_gauge가 가득 차면 gameover hook으로 이어지는지 확인한다.
3. 밸런싱 수치가 코드 곳곳에 흩어지지 않았는지 확인한다.
4. 성공 시 step 9를 `completed`로 바꾸고 danger system 결과를 `summary`에 적는다.

## 금지사항

- 난이도를 몬스터 속도 증가 중심으로만 만들지 마라. 이유: 설계는 위험도, 수색 빈도, 등장 타이밍 중심이다.
- 밸런싱 placeholder를 최종값처럼 문서화하지 마라. 이유: 플레이 테스트 후 조정 대상이다.
