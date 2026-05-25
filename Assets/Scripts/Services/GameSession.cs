using System.Collections.Generic;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// 한 플레이 세션의 현재 방, 방향, 인벤토리, 플래그, 완료 상태를 메모리에 보관합니다.
    /// </summary>
    public sealed class GameSession
    {
        private readonly HashSet<string> inventory = new HashSet<string>();
        private readonly HashSet<string> flags = new HashSet<string>();
        private readonly HashSet<string> solvedPuzzleIds = new HashSet<string>();
        private readonly HashSet<string> usedInteractableIds = new HashSet<string>();

        /// <summary>현재 플레이어가 위치한 방 ID입니다.</summary>
        public string CurrentRoomId { get; private set; }
        /// <summary>현재 방 안에서 플레이어가 바라보는 방향입니다.</summary>
        public RoomFaceDirection CurrentFaceDirection { get; private set; } = RoomFaceDirection.North;
        /// <summary>인벤토리에서 현재 선택한 아이템 ID입니다.</summary>
        public string SelectedItemId { get; private set; }
        /// <summary>스테이지 진행 중 몬스터 시스템이 활성화되었는지 나타냅니다.</summary>
        public bool MonsterEnabled { get; set; }

        /// <summary>현재 보유 중인 아이템 ID 목록입니다.</summary>
        public IReadOnlyCollection<string> InventoryItems => inventory;
        /// <summary>현재 세션에 설정된 진행 플래그 목록입니다.</summary>
        public IReadOnlyCollection<string> Flags => flags;
        /// <summary>해결 완료로 기록된 퍼즐 ID 목록입니다.</summary>
        public IReadOnlyCollection<string> SolvedPuzzleIds => solvedPuzzleIds;
        /// <summary>oneShot 처리로 다시 사용할 수 없게 된 상호작용 ID 목록입니다.</summary>
        public IReadOnlyCollection<string> UsedInteractableIds => usedInteractableIds;

        /// <summary>
        /// 새 스테이지를 시작하며 세션의 모든 진행 상태를 초기화합니다.
        /// </summary>
        public void Start(StageDefinition stage)
        {
            inventory.Clear();
            flags.Clear();
            solvedPuzzleIds.Clear();
            usedInteractableIds.Clear();
            SelectedItemId = string.Empty;
            MonsterEnabled = false;
            CurrentRoomId = stage.startRoomId;
            CurrentFaceDirection = RoomFaceDirection.North;
        }

        /// <summary>
        /// 지정한 방으로 이동하고 시선을 북쪽으로 초기화합니다.
        /// </summary>
        public void MoveTo(string roomId)
        {
            CurrentRoomId = roomId;
            CurrentFaceDirection = RoomFaceDirection.North;
        }

        /// <summary>
        /// 현재 바라보는 방향을 offset만큼 회전합니다. 4방향을 순환합니다.
        /// </summary>
        public void RotateFace(int offset)
        {
            CurrentFaceDirection = (RoomFaceDirection)(((int)CurrentFaceDirection + offset + 4) % 4);
        }

        /// <summary>현재 바라보는 방향을 직접 지정합니다.</summary>
        public void SetFaceDirection(RoomFaceDirection direction)
        {
            CurrentFaceDirection = direction;
        }

        /// <summary>인벤토리에 아이템을 추가합니다.</summary>
        public bool AddItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            return inventory.Add(itemId);
        }

        /// <summary>인벤토리에서 아이템을 제거합니다.</summary>
        public bool RemoveItem(string itemId)
        {
            return !string.IsNullOrWhiteSpace(itemId) && inventory.Remove(itemId);
        }

        /// <summary>
        /// 비어 있는 요구 아이템은 조건 없음으로 보고 true를 반환합니다.
        /// </summary>
        public bool HasItem(string itemId)
        {
            return string.IsNullOrWhiteSpace(itemId) || inventory.Contains(itemId);
        }

        /// <summary>
        /// 보유 중인 아이템만 선택하고, 없으면 선택을 해제합니다.
        /// </summary>
        public void SelectItem(string itemId)
        {
            SelectedItemId = inventory.Contains(itemId) ? itemId : string.Empty;
        }

        /// <summary>진행 플래그를 설정합니다.</summary>
        public void SetFlag(string flagId)
        {
            if (!string.IsNullOrWhiteSpace(flagId))
            {
                flags.Add(flagId);
            }
        }

        /// <summary>진행 플래그를 제거합니다.</summary>
        public void ClearFlag(string flagId)
        {
            if (!string.IsNullOrWhiteSpace(flagId))
            {
                flags.Remove(flagId);
            }
        }

        /// <summary>
        /// 비어 있는 요구 플래그는 조건 없음으로 보고 true를 반환합니다.
        /// </summary>
        public bool HasFlag(string flagId)
        {
            return string.IsNullOrWhiteSpace(flagId) || flags.Contains(flagId);
        }

        /// <summary>퍼즐을 해결 완료 상태로 기록합니다.</summary>
        public void MarkPuzzleSolved(string puzzleId)
        {
            if (!string.IsNullOrWhiteSpace(puzzleId))
            {
                solvedPuzzleIds.Add(puzzleId);
            }
        }

        /// <summary>
        /// 비어 있는 퍼즐 요구 조건은 조건 없음으로 보고 true를 반환합니다.
        /// </summary>
        public bool HasSolvedPuzzle(string puzzleId)
        {
            return string.IsNullOrWhiteSpace(puzzleId) || solvedPuzzleIds.Contains(puzzleId);
        }

        /// <summary>oneShot 상호작용이 다시 실행되지 않도록 사용 완료로 기록합니다.</summary>
        public void MarkInteractableUsed(string interactableId)
        {
            if (!string.IsNullOrWhiteSpace(interactableId))
            {
                usedInteractableIds.Add(interactableId);
            }
        }

        /// <summary>상호작용이 이미 사용 완료로 기록되었는지 확인합니다.</summary>
        public bool HasUsedInteractable(string interactableId)
        {
            return !string.IsNullOrWhiteSpace(interactableId) && usedInteractableIds.Contains(interactableId);
        }
    }

    /// <summary>
    /// GameSession의 플래그와 조건 판정을 도메인 언어에 가깝게 다루는 서비스입니다.
    /// </summary>
    public sealed class FlagService
    {
        private readonly GameSession session;

        public FlagService(GameSession session)
        {
            this.session = session;
        }

        /// <summary>진행 플래그를 설정합니다.</summary>
        public void Set(string flagId)
        {
            session.SetFlag(flagId);
        }

        /// <summary>진행 플래그를 제거합니다.</summary>
        public void Clear(string flagId)
        {
            session.ClearFlag(flagId);
        }

        /// <summary>진행 플래그가 설정되어 있는지 확인합니다.</summary>
        public bool Has(string flagId)
        {
            return session.HasFlag(flagId);
        }

        /// <summary>모든 플래그가 설정되어 있는지 확인합니다.</summary>
        public bool AllFlags(params string[] flagIds)
        {
            return All(flagIds, session.HasFlag);
        }

        /// <summary>
        /// 하나 이상의 플래그가 설정되어 있는지 확인합니다. 목록이 비어 있으면 조건 없음으로 봅니다.
        /// </summary>
        public bool AnyFlag(params string[] flagIds)
        {
            if (flagIds == null || flagIds.Length == 0)
            {
                return true;
            }

            foreach (var flagId in flagIds)
            {
                if (session.HasFlag(flagId))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 아이템, 필수 플래그, 선택 플래그, 금지 플래그, 해결 퍼즐 조건을 모두 판정합니다.
        /// </summary>
        public bool ConditionsMet(ConditionDefinition conditions)
        {
            if (conditions == null)
            {
                return true;
            }

            return All(conditions.requiredItemIds, session.HasItem)
                && All(conditions.requiredFlagIds, session.HasFlag)
                && AnyFlag(conditions.anyFlagIds)
                && None(conditions.forbiddenFlagIds, session.HasFlag)
                && All(conditions.solvedPuzzleIds, session.HasSolvedPuzzle);
        }

        private static bool All(string[] ids, System.Func<string, bool> predicate)
        {
            if (ids == null)
            {
                return true;
            }

            foreach (var id in ids)
            {
                if (!predicate(id))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool None(string[] ids, System.Func<string, bool> predicate)
        {
            if (ids == null)
            {
                return true;
            }

            foreach (var id in ids)
            {
                if (!string.IsNullOrWhiteSpace(id) && predicate(id))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
