# -*- coding: utf-8 -*-
from pathlib import Path
from xml.sax.saxutils import escape

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (
    PageBreak,
    Paragraph,
    Preformatted,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)


ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "output" / "pdf" / "Escape_from_Nightmares_Project_Handoff.pdf"
FONT_DIR = Path(r"C:\Windows\Fonts")


def register_fonts():
    pdfmetrics.registerFont(TTFont("Malgun", str(FONT_DIR / "malgun.ttf")))
    pdfmetrics.registerFont(TTFont("MalgunBold", str(FONT_DIR / "malgunbd.ttf")))


def make_styles():
    styles = getSampleStyleSheet()
    styles.add(
        ParagraphStyle(
            "KoreanBody",
            parent=styles["BodyText"],
            fontName="Malgun",
            fontSize=9.2,
            leading=14,
            textColor=colors.HexColor("#20262c"),
            spaceAfter=5,
        )
    )
    styles.add(
        ParagraphStyle(
            "KoreanSmall",
            parent=styles["KoreanBody"],
            fontSize=8.0,
            leading=12,
            textColor=colors.HexColor("#48545f"),
        )
    )
    styles.add(
        ParagraphStyle(
            "KoreanH1",
            parent=styles["Heading1"],
            fontName="MalgunBold",
            fontSize=16,
            leading=21,
            textColor=colors.HexColor("#26323f"),
            spaceBefore=8,
            spaceAfter=7,
        )
    )
    styles.add(
        ParagraphStyle(
            "KoreanH2",
            parent=styles["Heading2"],
            fontName="MalgunBold",
            fontSize=11.2,
            leading=15,
            textColor=colors.HexColor("#2f5663"),
            spaceBefore=6,
            spaceAfter=4,
        )
    )
    styles.add(
        ParagraphStyle(
            "CoverTitle",
            parent=styles["Title"],
            fontName="MalgunBold",
            fontSize=28,
            leading=34,
            alignment=TA_CENTER,
            textColor=colors.HexColor("#26323f"),
            spaceAfter=6,
        )
    )
    styles.add(
        ParagraphStyle(
            "CoverSub",
            parent=styles["KoreanBody"],
            fontSize=13,
            leading=18,
            alignment=TA_CENTER,
            textColor=colors.HexColor("#56616b"),
            spaceAfter=14,
        )
    )
    styles.add(
        ParagraphStyle(
            "Eyebrow",
            parent=styles["KoreanSmall"],
            fontName="MalgunBold",
            alignment=TA_CENTER,
            textColor=colors.HexColor("#3d7c8c"),
            spaceAfter=8,
        )
    )
    styles.add(
        ParagraphStyle(
            "Callout",
            parent=styles["KoreanBody"],
            borderColor=colors.HexColor("#4b9bb0"),
            borderWidth=0.75,
            borderPadding=7,
            backColor=colors.HexColor("#f0f8fa"),
            leftIndent=0,
            rightIndent=0,
            spaceBefore=4,
            spaceAfter=8,
        )
    )
    styles.add(
        ParagraphStyle(
            "Warning",
            parent=styles["KoreanBody"],
            borderColor=colors.HexColor("#d4564b"),
            borderWidth=0.75,
            borderPadding=7,
            backColor=colors.HexColor("#fff5f3"),
            spaceBefore=4,
            spaceAfter=8,
        )
    )
    styles.add(
        ParagraphStyle(
            "KoreanCode",
            parent=styles["Code"],
            fontName="Malgun",
            fontSize=7.1,
            leading=10,
            textColor=colors.HexColor("#1f2b36"),
            backColor=colors.HexColor("#f8f8f6"),
            borderColor=colors.HexColor("#d7d2c8"),
            borderWidth=0.5,
            borderPadding=6,
            spaceBefore=3,
            spaceAfter=7,
        )
    )
    return styles


def p(text, style):
    return Paragraph(escape(text).replace("\n", "<br/>"), style)


def bullets(items, styles):
    flow = []
    for item in items:
        flow.append(p(f"- {item}", styles["KoreanBody"]))
    return flow


def code(text, styles):
    return Preformatted(text, styles["KoreanCode"])


def table(rows, widths, styles, header=True):
    body = []
    for row in rows:
        body.append([p(str(cell), styles["KoreanSmall"]) for cell in row])
    tbl = Table(body, colWidths=widths, repeatRows=1 if header else 0)
    commands = [
        ("FONTNAME", (0, 0), (-1, -1), "Malgun"),
        ("FONTSIZE", (0, 0), (-1, -1), 7.6),
        ("LEADING", (0, 0), (-1, -1), 10.5),
        ("GRID", (0, 0), (-1, -1), 0.35, colors.HexColor("#d7d2c8")),
        ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ("LEFTPADDING", (0, 0), (-1, -1), 5),
        ("RIGHTPADDING", (0, 0), (-1, -1), 5),
        ("TOPPADDING", (0, 0), (-1, -1), 4),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, colors.HexColor("#fbfaf7")]),
    ]
    if header:
        commands.extend(
            [
                ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#26323f")),
                ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
                ("FONTNAME", (0, 0), (-1, 0), "MalgunBold"),
            ]
        )
    tbl.setStyle(TableStyle(commands))
    return tbl


def draw_footer(canvas, doc):
    canvas.saveState()
    canvas.setFont("Malgun", 7)
    canvas.setFillColor(colors.HexColor("#6b747c"))
    canvas.drawString(15 * mm, 9 * mm, "Escape from Nightmares 프로젝트 인수인계 문서")
    canvas.drawRightString(A4[0] - 15 * mm, 9 * mm, f"{doc.page}")
    canvas.restoreState()


def build_story(styles):
    story = []

    story.extend(
        [
            Spacer(1, 34 * mm),
            p("PROJECT HANDOFF GUIDE", styles["Eyebrow"]),
            p("Escape from Nightmares", styles["CoverTitle"]),
            p("스크립트 구조, 데이터 흐름, 유지보수 가이드", styles["CoverSub"]),
            p("Unity 6000.3.9f1 · URP 2D · uGUI · Unity Input System · 작성일 2026-05-25", styles["CoverSub"]),
            Spacer(1, 15 * mm),
            p(
                "새 담당자가 Unity 프로젝트를 열고 Stage 1 데이터와 런타임 흐름을 이해한 뒤, 방/아이템/퍼즐/UI/몬스터 규칙을 어디서 수정해야 하는지 빠르게 찾도록 만든 인수인계용 PDF입니다.",
                styles["Callout"],
            ),
            table(
                [
                    ["항목", "내용"],
                    ["프로젝트", "Escape_from_Nightmares"],
                    ["주요 경로", "Assets/EscapeFromNightmares"],
                    ["현재 Stage 1 데이터", "ScriptableObject asset 기준. 코드 factory는 Editor builder seed로만 사용"],
                    ["저장 파일 정책", "settings.json, clear_records.json만 허용"],
                ],
                [42 * mm, 123 * mm],
                styles,
            ),
            PageBreak(),
        ]
    )

    story.append(p("1. 한눈에 보는 현재 구조", styles["KoreanH1"]))
    story.append(
        p(
            "게임 코드는 기능별로 Scripts 하위 폴더에 분리되어 있고, Stage 1의 최종 런타임 데이터는 ScriptableObject asset으로 관리합니다. GameDirector는 씬 lifecycle과 서비스 조립을 담당하고, 실제 규칙/표시/액션 실행은 작은 presenter와 service로 나뉩니다.",
            styles["KoreanBody"],
        )
    )
    story.append(
        code(
            """Assets/EscapeFromNightmares/
  Audio/                 - AudioMixer 등 Unity 오디오 asset
  Prefabs/UI/            - 인벤토리, 설정창 등 UI prefab
  Resources/EscapeFromNightmares/
    Audio, CloseUps, Data, Endings, HideViews, Items, Monster, Objects, Puzzles, Rooms, Title, UI
  Scenes/                - TitleScene.unity, MainScene.unity
  ScriptableObjects/     - ResourcePathCatalog, RoomSpriteCatalog, MonsterPlacementCatalog, Stage1/
  Scripts/
    Bootstrap/ Data/ Editor/ Runtime/ Services/ UI/
  Tests/EditMode/        - Unity Test Framework EditMode tests""",
            styles,
        )
    )
    story.append(p("유지보수 기준", styles["KoreanH2"]))
    story.extend(
        bullets(
            [
                "방, 문, 아이템 ID, 퍼즐 답, 플래그 값은 Stage 1 ScriptableObject asset에서 확인합니다.",
                "RuntimeStageFactory는 런타임 데이터 원천이 아니라 Editor builder seed입니다.",
                "플레이어가 보는 UI/scene object 변경은 script와 함께 prefab/scene/editor builder 갱신이 필요합니다.",
                "새 저장 파일은 만들지 않습니다. settings.json과 clear_records.json만 유지합니다.",
            ],
            styles,
        )
    )

    story.append(p("2. Stage 1 데이터와 로딩 흐름", styles["KoreanH1"]))
    story.append(
        p(
            "Stage 1은 다음 asset 묶음으로 구성됩니다. Stage1DataAssetBuilder는 기존 asset을 삭제하지 않고 필드 갱신 위주로 동작해 GUID와 scene reference를 보존하도록 설계되어 있습니다.",
            styles["KoreanBody"],
        )
    )
    story.append(
        table(
            [
                ["구분", "경로 / 수량", "비고"],
                ["StageCatalog", "Assets/EscapeFromNightmares/Resources/EscapeFromNightmares/Data/StageCatalog.asset", "Resources.Load 후 stage1 참조 사용."],
                ["Stage1 root", "Assets/EscapeFromNightmares/ScriptableObjects/Stage1/Stage1.asset", "StageDefinition, MonsterNodeGraph, SoundCatalog 연결."],
                ["Items", "Stage1/Items - 10개", "아이템 ID, 이름, 설명, 리소스 경로."],
                ["Puzzles", "Stage1/Puzzles - 8개", "퍼즐 ID, 타입, 정답, 보상, 플래그."],
                ["Rooms", "Stage1/Rooms - 21개", "방 face, 문 target, interactable 구성."],
                ["MainScene 참조", "Assets/EscapeFromNightmares/Scenes/MainScene.unity", "GameDirector.stageDefinition serialized 참조 우선 사용."],
            ],
            [35 * mm, 83 * mm, 47 * mm],
            styles,
        )
    )
    story.append(p("런타임 로딩 순서", styles["KoreanH2"]))
    story.append(
        code(
            """GameDirector.Awake()
  -> StageRepository.LoadStage1(stageDefinition)
       1. GameDirector serialized StageDefinition이 있으면 사용
       2. 없으면 Resources/EscapeFromNightmares/Data/StageCatalog.asset 로드
       3. catalog.stage1 사용
  -> StageLookup 구성
  -> GameSession / services / presenters / UI reference bind""",
            styles,
        )
    )
    story.append(
        p(
            "주의: Stage 1 데이터를 수정할 때 RuntimeStageFactory.CreateStage1()를 런타임에서 다시 쓰면 안 됩니다. 새 runtime/test 코드는 StageRepository.LoadStage1() 또는 StageTestData.LoadStage1()을 사용하세요.",
            styles["Warning"],
        )
    )

    story.append(p("3. 런타임 실행 흐름", styles["KoreanH1"]))
    story.append(
        p(
            "MainScene의 GameDirector가 Stage asset을 로드하고 서비스와 presenter를 조립합니다. 이후 플레이어 입력은 InteractionSystem과 EscapeActionExecutor를 거쳐 room, inventory, puzzle, UI, sound, monster 상태를 갱신합니다.",
            styles["KoreanBody"],
        )
    )
    story.append(
        code(
            """TitleSceneController / Start button
  -> SceneManager.LoadScene(MainScene)
  -> GameDirector.Awake()
       Stage load, lookup, services, UI references bind
  -> GameDirector.Start()
       session.Start(stage), ambient BGM, first room render

Player clicks interactable
  -> InteractionSystem.ResolveInteractable(interactableId)
  -> EscapeActionExecutor.Execute(action)
       MoveRoom / AcquireItem / OpenCloseUp / OpenPuzzle / EnterHideSpot / CompleteStage
  -> RenderRoom(), presenters, services update""",
            styles,
        )
    )
    story.append(
        table(
            [
                ["기능", "주 담당", "역할"],
                ["방 표시", "RoomPresenter", "face별 이미지, hitbox 버튼, 문/상호작용 렌더링 helper."],
                ["액션 실행", "EscapeActionExecutor", "resolver 결과를 방 이동, 아이템 획득, UI 열기, 퍼즐 진입 등으로 분기."],
                ["Close-up", "CloseUpPresenter", "확대 이미지 표시와 종료 처리."],
                ["Puzzle", "PuzzlePresenter", "퍼즐 입력 UI, 정답 검증 요청, 성공/실패 표시."],
                ["Hide view", "HideViewPresenter", "은신 화면 표시와 은신 상태 진입/해제."],
                ["Monster", "MonsterPresenter", "몬스터 QA/상태 표시, 위험 상황 표현."],
                ["Stage clear", "StageClearPresenter", "클리어 UI와 clear record 저장 연결."],
                ["Panel fade", "PanelFader", "전환/패널 fade 처리."],
                ["특수 진행", "ProgressionFlow", "서재 금고, 세탁실 몬스터 시작, 최종 추격, Stage 1 클리어 기록."],
            ],
            [31 * mm, 43 * mm, 91 * mm],
            styles,
        )
    )

    story.append(p("4. 스크립트 카탈로그", styles["KoreanH1"]))
    story.append(
        p(
            "새 담당자가 파일을 찾기 쉽도록 폴더별 주요 스크립트와 책임을 정리했습니다. 이름은 .cs 파일명 기준입니다.",
            styles["KoreanBody"],
        )
    )
    story.append(p("Scripts/Data", styles["KoreanH2"]))
    story.append(
        table(
            [
                ["스크립트", "책임"],
                ["StageDefinition", "stageId, rooms, items, puzzles, monster graph, sound catalog 등 한 스테이지 전체 데이터."],
                ["StageCatalog", "Resources 아래에서 Stage 1 참조를 찾는 catalog asset 타입."],
                ["RoomDefinition", "roomId, face, 문 target, interactable, BGM/ambient 정보."],
                ["ItemDefinition", "itemId, 표시명, 설명, close-up/resource 경로."],
                ["PuzzleDefinition", "puzzleId, type, 정답, 요구 item, 보상 item/flag."],
                ["MonsterNodeGraph", "몬스터 이동/위험 node graph."],
                ["SoundCatalog", "BGM/SFX key와 Resource path catalog."],
                ["ResourcePathCatalog, RoomSpriteCatalog, MonsterPlacementCatalog", "리소스 경로와 타이틀/방/몬스터 배치 정보."],
                ["GameDefinitions", "공용 enum/struct/data contract 모음."],
            ],
            [48 * mm, 117 * mm],
            styles,
        )
    )
    story.append(p("Scripts/Runtime", styles["KoreanH2"]))
    story.append(
        table(
            [
                ["스크립트", "책임"],
                ["GameDirector", "Unity lifecycle, scene reference wiring, 서비스 조립, 최상위 흐름 facade."],
                ["StageRepository", "serialized StageDefinition 또는 StageCatalog를 통해 Stage 1 로드."],
                ["StageLookup", "room/item/puzzle/interactable ID 기반 빠른 조회 helper."],
                ["RoomController", "현재 방/face 상태와 회전, 방 이동 흐름의 상태 controller."],
                ["RoomPresenter", "방 이미지와 interactable hitbox UI 렌더링."],
                ["EscapeActionExecutor", "Interaction action을 런타임 서비스와 presenter 호출로 변환."],
                ["CloseUpPresenter, CloseUpState", "확대 화면 상태와 표시."],
                ["PuzzlePresenter", "퍼즐 UI 표시와 입력 전달."],
                ["HideViewPresenter", "은신 화면 표시와 은신 진입/해제."],
                ["MonsterPresenter", "몬스터 표시와 QA/debug 성격의 상태 노출."],
                ["StageClearPresenter", "스테이지 클리어 화면과 기록 흐름."],
                ["PanelFader", "패널 fade helper."],
                ["ProgressionFlow", "Stage 1 특수 플래그/추격/클리어 진행 규칙."],
                ["StudySafePuzzleRules", "서재 금고 퍼즐처럼 별도 규칙이 필요한 puzzle helper."],
            ],
            [52 * mm, 113 * mm],
            styles,
        )
    )
    story.append(p("Scripts/Services", styles["KoreanH2"]))
    story.append(
        table(
            [
                ["스크립트", "책임"],
                ["GameSession", "현재 stage, room, flags, inventory 등 플레이 세션 상태."],
                ["InteractionSystem", "현재 방의 interactable resolve와 action 추출."],
                ["InventoryService", "아이템 획득/보유/사용 가능성 판단."],
                ["PuzzleService", "퍼즐 정답 검증, 보상 지급, flag 갱신."],
                ["DangerSystem", "위험도와 몬스터 조우 관련 상태 판단."],
                ["HidingSystem", "은신 가능/은신 중 상태 관리."],
                ["MonsterAIController", "MonsterNodeGraph 기반 몬스터 이동/추격 판단."],
                ["ResourceManager", "Resources path 기반 Sprite/Audio 등 asset 로드."],
                ["SoundManager", "SoundCatalog 기반 BGM/SFX 재생."],
                ["SettingsSaveService", "settings.json, clear_records.json 저장/로드 정책."],
            ],
            [52 * mm, 113 * mm],
            styles,
        )
    )
    story.append(p("Bootstrap / UI / Editor / Tests", styles["KoreanH2"]))
    story.append(
        table(
            [
                ["폴더", "스크립트", "책임"],
                ["Bootstrap", "EscapeFromNightmaresBootstrap", "씬 bootstrap 및 기본 런타임 진입 보조."],
                ["UI", "InventoryWindow, InventorySlotView", "인벤토리 창과 슬롯 표시."],
                ["UI", "SettingsAudioPanel", "설정/오디오 UI."],
                ["UI", "TitleSceneController", "타이틀 화면 버튼, 씬 전환, clear record 표시."],
                ["Editor", "Stage1DataAssetBuilder", "Stage 1 ScriptableObject 자산 생성/갱신, MainScene 참조 연결."],
                ["Editor", "TitleSceneAssetBuilder", "TitleScene infrastructure, placeholder resources, monster placement catalog 갱신."],
                ["Editor", "RuntimeStageFactory", "Stage 1 seed 데이터. 런타임 직접 호출 금지."],
                ["Tests/EditMode", "StageTestData", "테스트용 Stage 1 로더. RuntimeStageFactory 직접 호출 방지."],
                ["Tests/EditMode", "StageAssetCatalogTests", "StageCatalog, graph, duplicate ID, resource path, scene reference 검증."],
                ["Tests/EditMode", "GameSessionTests", "세션, 상호작용, 퍼즐, 몬스터, UI helper 관련 EditMode 테스트."],
            ],
            [28 * mm, 51 * mm, 86 * mm],
            styles,
        )
    )

    story.append(p("5. 기능 수정 시 작업 위치", styles["KoreanH1"]))
    story.append(
        table(
            [
                ["수정하려는 것", "우선 볼 곳", "함께 확인할 것"],
                ["새 방/문/상호작용", "Stage1/Rooms asset, RoomDefinition", "Stage1DataAssetBuilder seed, door target graph test, room sprite resource."],
                ["아이템 추가/수정", "Stage1/Items asset, ItemDefinition", "InventoryService, ResourcePathCatalog, item sprite path."],
                ["퍼즐 규칙", "Stage1/Puzzles asset, PuzzleService", "ProgressionFlow, StudySafePuzzleRules, reward/flag tests."],
                ["특수 진행 규칙", "ProgressionFlow", "Stage 1 고유 flag 전환과 몬스터 시작/추격 시작 로직."],
                ["몬스터 이동/위험도", "MonsterNodeGraph asset, MonsterAIController", "MonsterPlacementCatalog, DangerSystem, monster QA 표시."],
                ["방 화면 UI", "RoomPresenter, MainScene Canvas", "hitbox 크기, prefab/scene reference, mobile layout."],
                ["타이틀/설정 UI", "TitleSceneController, SettingsAudioPanel", "TitleSceneAssetBuilder와 UI prefab 연결."],
                ["리소스 경로", "ResourcePathCatalog, SoundCatalog", "Resources/EscapeFromNightmares 하위 실제 asset 존재 여부."],
                ["저장 정책", "SettingsSaveService", "settings.json과 clear_records.json 외 새 파일 생성 금지."],
            ],
            [39 * mm, 56 * mm, 70 * mm],
            styles,
        )
    )

    story.append(p("6. 테스트와 검증", styles["KoreanH1"]))
    story.append(
        p(
            "EditMode 테스트는 Stage 데이터, 퍼즐, 상호작용, 몬스터, UI helper, 리소스 로딩 성격을 함께 검증합니다. 최근 리팩토링 후 확인된 기준은 105 total / 103 passed / 0 failed / 2 ignored입니다. ignored 2건은 docs/ROOM_IMAGE_HARNESS.md 누락으로 인한 문서 harness 관련 테스트입니다.",
            styles["KoreanBody"],
        )
    )
    story.append(p("권장 검증 명령", styles["KoreanH2"]))
    story.append(
        code(
            """Unity.exe -batchmode -quit -projectPath . -runTests -testPlatform EditMode -testResults TestResults/EditMode.xml""",
            styles,
        )
    )
    story.extend(
        bullets(
            [
                "StageCatalog.stage1이 null이 아니고 room/item/puzzle/sound reference가 유효한지 확인합니다.",
                "room graph와 door target이 서로 맞는지 확인합니다.",
                "roomId, itemId, puzzleId, interactableId 중복이 없는지 확인합니다.",
                "close-up, puzzle, room, item, sound resource path가 Resources에서 로드되는지 확인합니다.",
                "MainScene의 GameDirector가 Stage 1 asset을 참조하는지 확인합니다.",
                "수동 확인은 MainScene에서 방 이동, 인벤토리, 퍼즐, 은신, 몬스터 QA, 스테이지 클리어 흐름을 순서대로 봅니다.",
            ],
            styles,
        )
    )

    story.append(p("7. 인수인계 주의사항", styles["KoreanH1"]))
    story.extend(
        bullets(
            [
                "현재 git status에는 기존 문서/디자인 파일 삭제 흔적이 섞여 있습니다. 이번 PDF는 현재 프로젝트 구조 기준으로 작성했습니다.",
                "일부 기존 C# XML summary 주석은 인코딩이 깨진 흔적이 있습니다. 게임 규칙의 최종 원천으로 보지 말고 asset/test/runtime 동작을 기준으로 확인하세요.",
                "Stage1DataAssetBuilder는 사라진 ID의 낡은 asset을 자동 삭제하지 않고 경고만 남기는 정책입니다. 삭제는 Unity에서 참조 관계를 확인한 뒤 별도 정리하세요.",
                "플레이어가 보는 기능은 Script only로 끝내지 않습니다. Scene, Prefab, ScriptableObject asset, ResourceCatalog, Editor builder 중 필요한 deliverable을 함께 갱신합니다.",
                "새 이미지/오디오가 필요하면 직접 component reference 대신 ResourcePathCatalog 또는 SoundCatalog 같은 경로 catalog를 통해 연결합니다.",
            ],
            styles,
        )
    )

    story.append(p("부록 A. 주요 asset 경로", styles["KoreanH1"]))
    story.append(
        table(
            [
                ["용도", "경로"],
                ["StageCatalog", "Assets/EscapeFromNightmares/Resources/EscapeFromNightmares/Data/StageCatalog.asset"],
                ["Stage1 root", "Assets/EscapeFromNightmares/ScriptableObjects/Stage1/Stage1.asset"],
                ["Stage1 items", "Assets/EscapeFromNightmares/ScriptableObjects/Stage1/Items"],
                ["Stage1 puzzles", "Assets/EscapeFromNightmares/ScriptableObjects/Stage1/Puzzles"],
                ["Stage1 rooms", "Assets/EscapeFromNightmares/ScriptableObjects/Stage1/Rooms"],
                ["Monster graph", "Assets/EscapeFromNightmares/ScriptableObjects/Stage1/Stage1MonsterNodeGraph.asset"],
                ["Sound catalog", "Assets/EscapeFromNightmares/ScriptableObjects/Stage1/Stage1SoundCatalog.asset"],
                ["Resource catalog", "Assets/EscapeFromNightmares/Resources/EscapeFromNightmares/Data"],
                ["Main scene", "Assets/EscapeFromNightmares/Scenes/MainScene.unity"],
                ["Title scene", "Assets/EscapeFromNightmares/Scenes/TitleScene.unity"],
            ],
            [40 * mm, 125 * mm],
            styles,
        )
    )

    story.append(p("부록 B. 용어", styles["KoreanH1"]))
    story.append(
        table(
            [
                ["용어", "의미"],
                ["StageDefinition", "한 stage의 rooms/items/puzzles/monster/sound를 묶는 최상위 데이터."],
                ["StageCatalog", "Resources에서 stage asset을 찾기 위한 catalog. 현재 stage1 참조를 관리."],
                ["StageLookup", "런타임에서 ID로 방/아이템/퍼즐/상호작용을 빠르게 찾는 helper."],
                ["Presenter", "Unity UI/GameObject 표시를 담당하는 runtime helper. 규칙보다 표시 책임이 중심."],
                ["Service", "세션, 인벤토리, 퍼즐, 몬스터, 저장 등 상태와 규칙을 담당하는 순수 로직 중심 클래스."],
                ["Editor builder", "반복 생성/복구 가능한 scene, asset, catalog를 Unity Editor 메뉴에서 갱신하는 도구."],
            ],
            [38 * mm, 127 * mm],
            styles,
        )
    )

    return story


def main():
    register_fonts()
    styles = make_styles()
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    doc = SimpleDocTemplate(
        str(OUTPUT),
        pagesize=A4,
        rightMargin=15 * mm,
        leftMargin=15 * mm,
        topMargin=15 * mm,
        bottomMargin=16 * mm,
        title="Escape from Nightmares Project Handoff",
        author="OpenAI Codex",
    )
    doc.build(build_story(styles), onFirstPage=draw_footer, onLaterPages=draw_footer)
    print(f"Wrote {OUTPUT}")


if __name__ == "__main__":
    main()
