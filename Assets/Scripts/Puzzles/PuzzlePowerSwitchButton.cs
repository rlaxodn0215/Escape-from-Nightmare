using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    public class PuzzlePowerSwitchButton : MonoBehaviour
    {
        [SerializeField] private string switchId;
        [SerializeField] private Text labelText;
        [SerializeField] private PuzzlePowerDeviceUIBase target;

        private Button button;

        public string SwitchId
        {
            get { return switchId; }
        }

        private void Awake()
        {
            CacheReferences();
        }

        private void OnEnable()
        {
            CacheReferences();

            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
                button.onClick.AddListener(HandleClick);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        private void Reset()
        {
            CacheReferences();
        }

        public void SetTarget(PuzzlePowerDeviceUIBase newTarget)
        {
            target = newTarget;
        }

        public void SetInteractable(bool value)
        {
            CacheReferences();
            if (button != null)
            {
                button.interactable = value;
            }
        }

        private void HandleClick()
        {
            if (string.IsNullOrEmpty(switchId))
            {
                Debug.LogWarning("PuzzlePowerSwitchButton.switchId is empty: " + name, this);
                return;
            }

            if (target == null)
            {
                target = GetComponentInParent<PuzzlePowerDeviceUIBase>();
            }

            if (target == null)
            {
                Debug.LogWarning("PuzzlePowerSwitchButton target is missing: " + name, this);
                return;
            }

            target.InputSwitch(switchId);
        }

        private void CacheReferences()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (target == null)
            {
                target = GetComponentInParent<PuzzlePowerDeviceUIBase>();
            }

            if (labelText != null && !string.IsNullOrEmpty(switchId))
            {
                labelText.text = switchId;
            }
        }
    }
}
