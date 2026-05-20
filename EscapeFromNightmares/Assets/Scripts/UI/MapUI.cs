using EscapeFromNightmares.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public sealed class MapUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button[] floorButtons = new Button[0];
        [SerializeField] private Image[] floorImages = new Image[0];
        [SerializeField] private Image currentRoomMarker;
        [SerializeField] private TMP_Text floorLabel;
        [SerializeField] private string[] floorNames = new string[0];
        [SerializeField] private int defaultFloorIndex;

        private int currentFloorIndex;
        private int currentRoomFloorIndex = -1;
        private Vector2 currentRoomMarkerPosition;
        private bool hasCurrentRoom;

        public int CurrentFloorIndex => currentFloorIndex;

        private void Awake()
        {
            canvasGroup ??= GetComponent<CanvasGroup>();
            if (!hasCurrentRoom)
            {
                currentFloorIndex = Mathf.Clamp(defaultFloorIndex, 0, Mathf.Max(0, floorImages.Length - 1));
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            for (int index = 0; index < floorButtons.Length; index++)
            {
                int capturedIndex = index;
                if (floorButtons[index] != null)
                {
                    floorButtons[index].onClick.AddListener(() => ShowFloor(capturedIndex));
                }
            }
        }

        private void OnEnable()
        {
            ShowFloor(currentFloorIndex);
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
            }
        }

        public void Show()
        {
            SetVisible(true);
            ShowFloor(currentFloorIndex);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        public void Toggle()
        {
            bool isVisible = canvasGroup == null ? gameObject.activeSelf : canvasGroup.alpha > 0.5f;
            SetVisible(!isVisible);
        }

        public void ShowFloor(int floorIndex)
        {
            if (floorImages.Length == 0)
            {
                return;
            }

            currentFloorIndex = Mathf.Clamp(floorIndex, 0, floorImages.Length - 1);
            for (int index = 0; index < floorImages.Length; index++)
            {
                if (floorImages[index] != null)
                {
                    floorImages[index].enabled = index == currentFloorIndex;
                }
            }

            ApplyMarkerVisibility();

            if (floorLabel != null)
            {
                floorLabel.text = currentFloorIndex < floorNames.Length ? floorNames[currentFloorIndex] : "";
            }
        }

        public void SetCurrentRoom(FloorId floorId, Vector2 markerAnchoredPosition, string roomLabel)
        {
            hasCurrentRoom = true;
            currentRoomFloorIndex = ToFloorIndex(floorId);
            currentRoomMarkerPosition = markerAnchoredPosition;
            ShowFloor(currentRoomFloorIndex);

            if (currentRoomMarker != null)
            {
                currentRoomMarker.rectTransform.anchoredPosition = markerAnchoredPosition;
                ApplyMarkerVisibility();
            }

            if (floorLabel != null)
            {
                string floorName = currentFloorIndex < floorNames.Length ? floorNames[currentFloorIndex] : "";
                floorLabel.text = string.IsNullOrWhiteSpace(roomLabel) ? floorName : $"{floorName} / {roomLabel}";
            }
        }

        private void SetVisible(bool visible)
        {
            gameObject.SetActive(true);

            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        private void ApplyMarkerVisibility()
        {
            if (currentRoomMarker == null)
            {
                return;
            }

            currentRoomMarker.enabled = hasCurrentRoom && currentFloorIndex == currentRoomFloorIndex;
            currentRoomMarker.rectTransform.anchoredPosition = currentRoomMarkerPosition;
        }

        private static int ToFloorIndex(FloorId floorId)
        {
            return floorId switch
            {
                FloorId.FirstFloor => 0,
                FloorId.SecondFloor => 1,
                FloorId.Basement => 2,
                FloorId.Attic => 3,
                _ => 0
            };
        }
    }
}
